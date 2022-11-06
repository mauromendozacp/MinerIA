using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StartConfigurationScreen : MonoBehaviour
{
    public PopulationManager populationManager;
    public Text populationCountTxt;
    public Slider populationCountSlider;
    public Text minesCountTxt;
    public Slider minesCountSlider;
    public Text generationDurationTxt;
    public Slider generationDurationSlider;
    public Text eliteCountTxt;
    public Slider eliteCountSlider;
    public Text mutationChanceTxt;
    public Slider mutationChanceSlider;
    public Text mutationRateTxt;
    public Slider mutationRateSlider;
    public Text hiddenLayersCountTxt;
    public Slider hiddenLayersCountSlider;
    public Text neuronsPerHLCountTxt;
    public Slider neuronsPerHLSlider;
    public Text biasTxt;
    public Slider biasSlider;
    public Text sigmoidSlopeTxt;
    public Slider sigmoidSlopeSlider;
    public Button startButton;
    public Button loadButton;
    public GameObject simulationScreen;
    public TextAsset brainDataJson;

    string populationText;
    string minesText;
    string generationDurationText;
    string elitesText;
    string mutationChanceText;
    string mutationRateText;
    string hiddenLayersCountText;
    string biasText;
    string sigmoidSlopeText;
    string neuronsPerHLCountText;

    void Start()
    {   
        populationCountSlider.onValueChanged.AddListener(OnPopulationCountChange);
        minesCountSlider.onValueChanged.AddListener(OnMinesCountChange);
        generationDurationSlider.onValueChanged.AddListener(OnGenerationDurationChange);
        eliteCountSlider.onValueChanged.AddListener(OnEliteCountChange);
        mutationChanceSlider.onValueChanged.AddListener(OnMutationChanceChange);
        mutationRateSlider.onValueChanged.AddListener(OnMutationRateChange);
        hiddenLayersCountSlider.onValueChanged.AddListener(OnHiddenLayersCountChange);
        neuronsPerHLSlider.onValueChanged.AddListener(OnNeuronsPerHLChange);
        biasSlider.onValueChanged.AddListener(OnBiasChange);
        sigmoidSlopeSlider.onValueChanged.AddListener(OnSigmoidSlopeChange);

        populationText = populationCountTxt.text;
        minesText = minesCountTxt.text;
        generationDurationText = generationDurationTxt.text;
        elitesText = eliteCountTxt.text;
        mutationChanceText = mutationChanceTxt.text;
        mutationRateText = mutationRateTxt.text;
        hiddenLayersCountText = hiddenLayersCountTxt.text;
        neuronsPerHLCountText = neuronsPerHLCountTxt.text;
        biasText = biasTxt.text;
        sigmoidSlopeText = sigmoidSlopeTxt.text;

        populationCountSlider.value = PopulationManager.Instance.PopulationCount;
        minesCountSlider.value = PopulationManager.Instance.MinesCount;
        generationDurationSlider.value = PopulationManager.Instance.GenerationDuration;
        eliteCountSlider.value = PopulationManager.Instance.EliteCount;
        mutationChanceSlider.value = PopulationManager.Instance.MutationChance * 100.0f;
        mutationRateSlider.value = PopulationManager.Instance.MutationRate * 100.0f;
        hiddenLayersCountSlider.value = PopulationManager.Instance.HiddenLayers;
        neuronsPerHLSlider.value = PopulationManager.Instance.NeuronsCountPerHL;
        biasSlider.value = -PopulationManager.Instance.Bias;
        sigmoidSlopeSlider.value = PopulationManager.Instance.P;

        startButton.onClick.AddListener(OnStartButtonClick);
        loadButton.onClick.AddListener(LoadBrainData);
    }

    void OnPopulationCountChange(float value)
    {
        PopulationManager.Instance.PopulationCount = (int)value;
        
        populationCountTxt.text = string.Format(populationText, PopulationManager.Instance.PopulationCount);
    }

    void OnMinesCountChange(float value)
    {
        PopulationManager.Instance.MinesCount = (int)value;        

        minesCountTxt.text = string.Format(minesText, PopulationManager.Instance.MinesCount);
    }

    void OnGenerationDurationChange(float value)
    {
        PopulationManager.Instance.GenerationDuration = (int)value;
        
        generationDurationTxt.text = string.Format(generationDurationText, PopulationManager.Instance.GenerationDuration);
    }

    void OnEliteCountChange(float value)
    {
        PopulationManager.Instance.EliteCount = (int)value;

        eliteCountTxt.text = string.Format(elitesText, PopulationManager.Instance.EliteCount);
    }

    void OnMutationChanceChange(float value)
    {
        PopulationManager.Instance.MutationChance = value / 100.0f;

        mutationChanceTxt.text = string.Format(mutationChanceText, (int)(PopulationManager.Instance.MutationChance * 100));
    }

    void OnMutationRateChange(float value)
    {
        PopulationManager.Instance.MutationRate = value / 100.0f;

        mutationRateTxt.text = string.Format(mutationRateText, (int)(PopulationManager.Instance.MutationRate * 100));
    }

    void OnHiddenLayersCountChange(float value)
    {
        PopulationManager.Instance.HiddenLayers = (int)value;
        

        hiddenLayersCountTxt.text = string.Format(hiddenLayersCountText, PopulationManager.Instance.HiddenLayers);
    }

    void OnNeuronsPerHLChange(float value)
    {
        PopulationManager.Instance.NeuronsCountPerHL = (int)value;

        neuronsPerHLCountTxt.text = string.Format(neuronsPerHLCountText, PopulationManager.Instance.NeuronsCountPerHL);
    }

    void OnBiasChange(float value)
    {
        PopulationManager.Instance.Bias = -value;

        biasTxt.text = string.Format(biasText, PopulationManager.Instance.Bias.ToString("0.00"));
    }

    void OnSigmoidSlopeChange(float value)
    {
        PopulationManager.Instance.P = value;

        sigmoidSlopeTxt.text = string.Format(sigmoidSlopeText, PopulationManager.Instance.P.ToString("0.00"));
    }


    void OnStartButtonClick()
    {
        PopulationManager.Instance.StartSimulation();
        this.gameObject.SetActive(false);
        simulationScreen.SetActive(true);
    }

    void LoadBrainData()
    {
        string path = null;

#if UNITY_EDITOR
        path = EditorUtility.OpenFilePanel("Select Brain Data", "", "json");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
#endif

        BrainData data = null;
        if (path == null)
        {
            data = JsonUtility.FromJson<BrainData>(brainDataJson.text);
        }
        else
        {
            string dataJson = File.ReadAllText(path);
            data = JsonUtility.FromJson<BrainData>(dataJson);
        }

        if (data != null)
        {
            populationManager.savePopulation = data.genomes;

            populationManager.generation = data.GenerationCount;
            populationManager.PopulationCount = data.PopulationCount;
            populationManager.MinesCount = data.MinesCount;
            
            populationManager.GenerationDuration = data.GenerationDuration;
            populationManager.IterationCount = data.IterationCount;

            populationManager.EliteCount = data.EliteCount;
            populationManager.MutationChance = data.MutationChance;
            populationManager.MutationRate = data.MutationRate;

            populationManager.InputsCount = data.InputsCount;
            populationManager.HiddenLayers = data.HiddenLayers;
            populationManager.OutputsCount = data.OutputsCount;
            populationManager.NeuronsCountPerHL = data.NeuronsCountPerHL;
            populationManager.Bias = data.Bias;
            populationManager.P = data.P;

            populationManager.dataLoaded = true;

            OnStartButtonClick();
        }
    }
}
