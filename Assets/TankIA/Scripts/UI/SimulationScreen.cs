using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class SimulationScreen : MonoBehaviour
{
    public PopulationManager populationManager;
    public Text generationsCountTxt;
    public Text bestFitnessTxt;
    public Text avgFitnessTxt;
    public Text worstFitnessTxt;
    public Text timerTxt;
    public Slider timerSlider;
    public Button pauseBtn;
    public Button stopBtn;
    public Button saveBtn;
    public Button exitBtn;
    public Text redScore;
    public Text greenScore;
    public GameObject startConfigurationScreen;
    public GameplayController gameplayController;

    string generationsCountText;
    string bestFitnessText;
    string avgFitnessText;
    string worstFitnessText;
    string timerText;
    string redText;
    string greenText;
    int lastGeneration = 0;

    // Start is called before the first frame update
    void Start()
    {
        timerSlider.onValueChanged.AddListener(OnTimerChange);
        timerText = timerTxt.text;

        timerTxt.text = string.Format(timerText, PopulationManager.Instance.IterationCount);

        if (string.IsNullOrEmpty(generationsCountText))
            generationsCountText = generationsCountTxt.text;   
        if (string.IsNullOrEmpty(bestFitnessText))
            bestFitnessText = bestFitnessTxt.text;   
        if (string.IsNullOrEmpty(avgFitnessText))
            avgFitnessText = avgFitnessTxt.text;   
        if (string.IsNullOrEmpty(worstFitnessText))
            worstFitnessText = worstFitnessTxt.text;
        if (string.IsNullOrEmpty(redText))
            redText = redScore.text;
        if (string.IsNullOrEmpty(greenText))
            greenText = greenScore.text;

        pauseBtn.onClick.AddListener(OnPauseButtonClick);
        stopBtn.onClick.AddListener(OnStopButtonClick);
        saveBtn.onClick.AddListener(SaveBrainData);
        exitBtn.onClick.AddListener(ExitGame);

        saveBtn.interactable = false;
#if UNITY_EDITOR
        saveBtn.interactable = true;
#endif
    }

    void OnEnable()
    {
        if (string.IsNullOrEmpty(generationsCountText))
            generationsCountText = generationsCountTxt.text;   
        if (string.IsNullOrEmpty(bestFitnessText))
            bestFitnessText = bestFitnessTxt.text;   
        if (string.IsNullOrEmpty(avgFitnessText))
            avgFitnessText = avgFitnessTxt.text;   
        if (string.IsNullOrEmpty(worstFitnessText))
            worstFitnessText = worstFitnessTxt.text;
        if (string.IsNullOrEmpty(redText))
            redText = redScore.text;
        if (string.IsNullOrEmpty(greenText))
            greenText = greenScore.text;

        generationsCountTxt.text = string.Format(generationsCountText, 0);
        bestFitnessTxt.text = string.Format(bestFitnessText, 0);
        avgFitnessTxt.text = string.Format(avgFitnessText, 0);
        worstFitnessTxt.text = string.Format(worstFitnessText, 0);
        redScore.text = string.Format(redText, 0);
        greenScore.text = string.Format(greenText, 0);
    }

    void OnTimerChange(float value)
    {
        PopulationManager.Instance.IterationCount = (int)value;
        timerTxt.text = string.Format(timerText, PopulationManager.Instance.IterationCount);
    }

    void OnPauseButtonClick()
    {
        PopulationManager.Instance.PauseSimulation();
    }

    void OnStopButtonClick()
    {
        PopulationManager.Instance.StopSimulation();
        this.gameObject.SetActive(false);
        startConfigurationScreen.SetActive(true);
        lastGeneration = 0;
    }

    void ExitGame()
    {
        Application.Quit();
    }

    void LateUpdate()
    {
        if (lastGeneration != PopulationManager.Instance.generation)
        {
            lastGeneration = PopulationManager.Instance.generation;
            generationsCountTxt.text = string.Format(generationsCountText, PopulationManager.Instance.generation);
            bestFitnessTxt.text = string.Format(bestFitnessText, PopulationManager.Instance.bestFitness);
            avgFitnessTxt.text = string.Format(avgFitnessText, PopulationManager.Instance.avgFitness);
            worstFitnessTxt.text = string.Format(worstFitnessText, PopulationManager.Instance.worstFitness);
        }

        redScore.text = string.Format(redText, gameplayController.redScore);
        greenScore.text = string.Format(greenText, gameplayController.greenScore);
    }

    void SaveBrainData()
    {
        BrainData data = new BrainData();
        data.genomes = populationManager.population;

        data.GenerationCount = populationManager.generation;
        data.PopulationCount = populationManager.PopulationCount;
        data.MinesCount = populationManager.MinesCount;

        data.GenerationDuration = populationManager.GenerationDuration;
        data.IterationCount = populationManager.IterationCount;

        data.EliteCount = populationManager.EliteCount;
        data.MutationChance = populationManager.MutationChance;
        data.MutationRate = populationManager.MutationRate;

        data.InputsCount = populationManager.InputsCount;
        data.HiddenLayers = populationManager.HiddenLayers;
        data.OutputsCount = populationManager.OutputsCount;
        data.NeuronsCountPerHL = populationManager.NeuronsCountPerHL;
        data.Bias = populationManager.Bias;
        data.P = populationManager.P;

        string dataJson = JsonUtility.ToJson(data, true);

        string path = null;
#if UNITY_EDITOR
        path = EditorUtility.SaveFilePanel("Save Brain Data", "", "brain_data.json", "json");
#endif

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        File.WriteAllText(path, dataJson);
    }
}
