using UnityEngine;
using System.Collections.Generic;

public class PopulationManager : MonoBehaviour
{
    public GameplayController gameplayController;
    public bool dataLoaded = false;

    public GameObject TankPrefab;
    public GameObject MinePrefab;

    public int PopulationCount = 40;
    public int MinesCount = 50;

    public Vector3 SceneHalfExtents = new Vector3 (20.0f, 0.0f, 20.0f);

    public float GenerationDuration = 20.0f;
    public int IterationCount = 1;

    public int EliteCount = 4;
    public float MutationChance = 0.10f;
    public float MutationRate = 0.01f;

    public int InputsCount = 6;
    public int HiddenLayers = 1;
    public int OutputsCount = 2;
    public int NeuronsCountPerHL = 7;
    public float Bias = 1f;
    public float P = 0.5f;

    GeneticAlgorithm genAlg;

    List<Tank> populationGOs = new List<Tank>();
    public List<Genome> population = new List<Genome>();
    public List<Genome> savePopulation = new List<Genome>();
    List<NeuralNetwork> brains = new List<NeuralNetwork>();
    List<GameObject> mines = new List<GameObject>();
    List<GameObject> goodMines = new List<GameObject>();
    List<GameObject> badMines = new List<GameObject>();
    
    float accumTime = 0;
    bool isRunning = false;

    public int generation {
        get; set;
    }

    public float bestFitness 
    {
        get; private set;
    }

    public float avgFitness 
    {
        get; private set;
    }

    public float worstFitness 
    {
        get; private set;
    }

    private float getBestFitness()
    {
        float fitness = 0;
        foreach(Genome g in population)
        {
            if (fitness < g.Fitness)
                fitness = g.Fitness;
        }

        return fitness;
    }

    private float getAvgFitness()
    {
        float fitness = 0;
        foreach(Genome g in population)
        {
            fitness += g.Fitness;
        }

        return fitness / population.Count;
    }

    private float getWorstFitness()
    {
        float fitness = float.MaxValue;
        foreach(Genome g in population)
        {
            if (fitness > g.Fitness)
                fitness = g.Fitness;
        }

        return fitness;
    }

    static PopulationManager instance = null;

    public static PopulationManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PopulationManager>();

            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
    }

    public void StartSimulation()
    {
        // Create and confiugre the Genetic Algorithm
        genAlg = new GeneticAlgorithm(EliteCount, MutationChance, MutationRate);

        GenerateInitialPopulation();
        CreateMines();

        isRunning = true;
    }

    public void PauseSimulation()
    {
        isRunning = !isRunning;
    }

    public void StopSimulation()
    {
        isRunning = false;

        generation = 0;

        // Destroy previous tanks (if there are any)
        DestroyTanks();

        // Destroy all mines
        DestroyMines();
    }

    // Generate the random initial population
    void GenerateInitialPopulation()
    {
        if (!dataLoaded)
        {
            generation = 0;
        }

        // Destroy previous tanks (if there are any)
        DestroyTanks();

        for (int i = 0; i < PopulationCount; i++)
        {
            NeuralNetwork brain = CreateBrain();
            Genome genome = null;

            if (dataLoaded)
            {
                genome = savePopulation[i];
            }
            else
            {
                genome = new Genome(brain.GetTotalWeightsCount());
            }

            brain.SetWeights(genome.genome);
            brains.Add(brain);
            population.Add(genome);

            Tank tank = CreateTank(genome, brain);
            tank.SetColor(i % 2 == 0 ? Color.green : Color.red);
            tank.team = i % 2 == 0 ? TEAM.B : TEAM.A;

            populationGOs.Add(tank);
        }

        accumTime = 0.0f;
    }

    // Creates a new NeuralNetwork
    NeuralNetwork CreateBrain()
    {
        NeuralNetwork brain = new NeuralNetwork();

        // Add first neuron layer that has as many neurons as inputs
        brain.AddFirstNeuronLayer(InputsCount, Bias, P);

        for (int i = 0; i < HiddenLayers; i++)
        {
            // Add each hidden layer with custom neurons count
            brain.AddNeuronLayer(NeuronsCountPerHL, Bias, P);
        }

        // Add the output layer with as many neurons as outputs
        brain.AddNeuronLayer(OutputsCount, Bias, P);

        return brain;
    }

    // Evolve!!!
    void Epoch(TEAM loserTeam)
    {
        // Increment generation counter
        generation++;

        // Calculate best, average and worst fitness
        bestFitness = getBestFitness();
        avgFitness = getAvgFitness();
        worstFitness = getWorstFitness();

        // Evolve each genome and create a new array of genomes
        Genome[] newGenomes = genAlg.Epoch(population.ToArray());

        // Clear current population
        population.Clear();

        // Add new population
        population.AddRange(newGenomes);

        // Set the new genomes as each NeuralNetwork weights
        for (int i = 0; i < PopulationCount; i++)
        {
            if (populationGOs[i].team == loserTeam || loserTeam == TEAM.NONE)
            {
                NeuralNetwork brain = brains[i];
                brain.SetWeights(newGenomes[i].genome);
                populationGOs[i].SetBrain(newGenomes[i], brain);
            }

            populationGOs[i].transform.position = GetRandomPos();
            populationGOs[i].transform.rotation = GetRandomRot();
        }
    }

    // Update is called once per frame
    void FixedUpdate () 
	{
        if (!isRunning)
            return;
        
        float dt = Time.fixedDeltaTime;

        for (int i = 0; i < Mathf.Clamp((float)(IterationCount / 100.0f) * 50, 1, 50); i++)
        {
            foreach (Tank t in populationGOs)
            {
                // Get the nearest mine
                GameObject nearMine = GetNearestMine(t.transform.position);
                t.SetNearestMine(nearMine);

                GameObject goodMine = null;
                GameObject badMine = null;

                //Set Good Near Mine For Team
                if (t.team == TEAM.B)
                {
                    goodMine = GetNearestGoodMine(t.transform.position);
                    badMine = GetNearestBadMine(t.transform.position);
                }
                else if (t.team == TEAM.A)
                {
                    goodMine = GetNearestBadMine(t.transform.position);
                    badMine = GetNearestGoodMine(t.transform.position);
                }

                t.SetGoodNearestMine(goodMine != null ? goodMine : nearMine);
                t.SetBadNearestMine(badMine != null ? badMine : nearMine);

                //Set the nearest tank in to near mine
                Tank nearTank = GetNearTank(nearMine.transform.position);
                t.SetNearTankInNearMine(nearTank);

                // Think!! 
                t.Think(dt);

                // Just adjust tank position when reaching world extents
                Vector3 pos = t.transform.position;
                if (pos.x > SceneHalfExtents.x)
                    pos.x -= SceneHalfExtents.x * 2;
                else if (pos.x < -SceneHalfExtents.x)
                    pos.x += SceneHalfExtents.x * 2;

                if (pos.z > SceneHalfExtents.z)
                    pos.z -= SceneHalfExtents.z * 2;
                else if (pos.z < -SceneHalfExtents.z)
                    pos.z += SceneHalfExtents.z * 2;

                // Set tank position
                t.transform.position = pos;
            }

            // Check the time to evolve
            accumTime += dt;
            if (accumTime >= GenerationDuration && !dataLoaded)
            {
                accumTime -= GenerationDuration;

                TEAM loserTeam = GetLoserTeam();
                Epoch(loserTeam);

                gameplayController.ResetScore();

                break;
            }
        }
	}

#region Helpers
    private TEAM GetLoserTeam()
    {
        float redFitness = 0f;
        float greenFitness = 0f;

        for (int i = 0; i < populationGOs.Count; i++)
        {
            switch (populationGOs[i].team)
            {
                case TEAM.A:
                    redFitness += populationGOs[i].Fitness;
                    break;
                case TEAM.B:
                    greenFitness += populationGOs[i].Fitness;
                    break;
            }
        }

        if (redFitness == greenFitness)
        {
            return TEAM.NONE;
        }

        return redFitness < greenFitness ? TEAM.A : TEAM.B;
    }

    Tank CreateTank(Genome genome, NeuralNetwork brain)
    {
        Vector3 position = GetRandomPos();
        GameObject go = Instantiate(TankPrefab, position, GetRandomRot());
        Tank t = go.GetComponent<Tank>();
        t.SetBrain(genome, brain);
        t.onUpdateScore = gameplayController.UpdateScore;
        return t;
    }

    void DestroyMines()
    {
        foreach (GameObject go in mines)
            Destroy(go);

        mines.Clear();
        goodMines.Clear();
        badMines.Clear();
    }

    void DestroyTanks()
    {
        foreach (Tank go in populationGOs)
            Destroy(go.gameObject);

        populationGOs.Clear();
        population.Clear();
        brains.Clear();
    }

    void CreateMines()
    {
        // Destroy previous created mines
        DestroyMines();

        for (int i = 0; i < MinesCount; i++)
        {
            Vector3 position = GetRandomPos();
            GameObject go = Instantiate<GameObject>(MinePrefab, position, Quaternion.identity);

            bool good = Random.Range(-1.0f, 1.0f) >= 0;

            SetMineGood(good, go);

            mines.Add(go);
        }
    }

    void SetMineGood(bool good, GameObject go)
    {
        if (good)
        {
            go.GetComponent<Renderer>().material.color = Color.green;
            go.GetComponent<Mine>().team = TEAM.B;
            goodMines.Add(go);
        }
        else
        {
            go.GetComponent<Renderer>().material.color = Color.red;
            go.GetComponent<Mine>().team = TEAM.A;
            badMines.Add(go);
        }

    }

    public void RelocateMine(GameObject mine)
    {
        if (goodMines.Contains(mine))
            goodMines.Remove(mine);
        else
            badMines.Remove(mine);

        bool good = Random.Range(-1.0f, 1.0f) >= 0;

        SetMineGood(good, mine);

        mine.transform.position = GetRandomPos();
    }

    Vector3 GetRandomPos()
    {
        return new Vector3(Random.value * SceneHalfExtents.x * 2.0f - SceneHalfExtents.x, 0.0f, Random.value * SceneHalfExtents.z * 2.0f - SceneHalfExtents.z); 
    }

    Quaternion GetRandomRot()
    {
        return Quaternion.AngleAxis(Random.value * 360.0f, Vector3.up);
    }

    GameObject GetNearestMine(Vector3 pos)
    {
        GameObject nearest = mines[0];
        float distance = (pos - nearest.transform.position).sqrMagnitude;

        foreach (GameObject go in mines)
        {
            float newDist = (go.transform.position - pos).sqrMagnitude;
            if (newDist < distance)
            {
                nearest = go;
                distance = newDist;
            }
        }

        return nearest;
    }   

    GameObject GetNearestGoodMine(Vector3 pos)
    {
        GameObject nearest = mines[0];
        float distance = (pos - nearest.transform.position).sqrMagnitude;

        foreach (GameObject go in goodMines)
        {
            float newDist = (go.transform.position - pos).sqrMagnitude;
            if (newDist < distance)
            {
                nearest = go;
                distance = newDist;
            }
        }

        return nearest;
    }   

    GameObject GetNearestBadMine(Vector3 pos)
    {
        GameObject nearest = mines[0];
        float distance = (pos - nearest.transform.position).sqrMagnitude;

        foreach (GameObject go in badMines)
        {
            float newDist = (go.transform.position - pos).sqrMagnitude;
            if (newDist < distance)
            {
                nearest = go;
                distance = newDist;
            }
        }

        return nearest;
    }   

    Tank GetNearTank(Vector3 pos)
    {
        Tank nearest = populationGOs[0];
        float distance = (pos - nearest.transform.position).sqrMagnitude;

        foreach (Tank go in populationGOs)
        {
            float newDist = (go.transform.position - pos).sqrMagnitude;
            if (newDist < distance)
            {
                nearest = go;
                distance = newDist;
            }
        }

        return nearest;
    }

#endregion

}
