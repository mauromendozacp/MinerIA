using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Genome
{
	public float[] genome;
	public float fitness = 0;

	public float Fitness { get => fitness; set => fitness = value; }

	public Genome(float[] genes)
	{
		this.genome = genes;
		Fitness = 0;
	}

	public Genome(int genesCount)
	{
        genome = new float[genesCount];

        for (int j = 0; j < genesCount; j++)
            genome[j] = Random.Range(-1.0f, 1.0f);

        Fitness = 0;
	}

    public Genome()
    {
        Fitness = 0;
    }

}

public class GeneticAlgorithm 
{
	List<Genome> population = new List<Genome>();
	List<Genome> newPopulation = new List<Genome>();

	float totalFitness;

	int eliteCount = 0;
	float mutationChance = 0.0f;
	float mutationRate = 0.0f;

	public GeneticAlgorithm(int eliteCount, float mutationChance, float mutationRate)
	{
		this.eliteCount = eliteCount;
		this.mutationChance = mutationChance;
		this.mutationRate = mutationRate;
	}

    public Genome[] GetRandomGenomes(int count, int genesCount)
    {
        Genome[] genomes = new Genome[count];

        for (int i = 0; i < count; i++)
        {
            genomes[i] = new Genome(genesCount);
        }

        return genomes;
    }


	public Genome[] Epoch(Genome[] oldGenomes)
	{
		totalFitness = 0;

		population.Clear();
		newPopulation.Clear();

		population.AddRange(oldGenomes);
		population.Sort(HandleComparison);

		foreach (Genome g in population)
		{
			totalFitness += g.Fitness;
		}

		SelectElite();

		while (newPopulation.Count < population.Count)
		{
			Crossover();
		}

		return newPopulation.ToArray();
	}

	void SelectElite()
	{
		for (int i = 0; i < eliteCount && newPopulation.Count < population.Count; i++)
		{
			newPopulation.Add(population[i]);
		}
	}

	void Crossover()
	{
		Genome mom = RouletteSelection();
		Genome dad = RouletteSelection();

		Genome child1;
		Genome child2;

		Crossover(mom, dad, out child1, out child2);

		newPopulation.Add(child1);
		newPopulation.Add(child2);
	}

	void Crossover(Genome mom, Genome dad, out Genome child1, out Genome child2)
	{
		child1 = new Genome();
		child2 = new Genome();

		child1.genome = new float[mom.genome.Length];
		child2.genome = new float[mom.genome.Length];

		int pivot = Random.Range(0, mom.genome.Length);

		for (int i = 0; i < pivot; i++)
		{
			child1.genome[i] = mom.genome[i];

			if (ShouldMutate())
				child1.genome[i] += Random.Range(-mutationRate, mutationRate);

			child2.genome[i] = dad.genome[i];

			if (ShouldMutate())
				child2.genome[i] += Random.Range(-mutationRate, mutationRate);
		}

		for (int i = pivot; i < mom.genome.Length; i++)
		{
			child2.genome[i] = mom.genome[i];

			if (ShouldMutate())
				child2.genome[i] += Random.Range(-mutationRate, mutationRate);
			
			child1.genome[i] = dad.genome[i];

			if (ShouldMutate())
				child1.genome[i] += Random.Range(-mutationRate, mutationRate);
		}
	}

	bool ShouldMutate()
	{
		return Random.Range(0.0f, 1.0f) < mutationChance;
	}

	int HandleComparison(Genome x, Genome y)
	{
		return x.Fitness > y.Fitness ? 1 : x.Fitness < y.Fitness ? -1 : 0;
	}


	public Genome RouletteSelection()
	{
		float rnd = Random.Range(0, Mathf.Max(totalFitness, 0));

		float fitness = 0;

		for (int i = 0; i < population.Count; i++)
		{
			fitness += Mathf.Max(population[i].Fitness, 0);
			if (fitness >= rnd)
				return population[i];
		}

		return null;
	}

}
