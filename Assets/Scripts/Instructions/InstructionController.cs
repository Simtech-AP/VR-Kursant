using TMPro;
using UnityEngine;

/// <summary>
/// Controller in charge of loading and showing instructions to user
/// </summary>
public class InstructionController : Controller
{
    /// <summary>
    /// Reference to text on which to show instruction
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI text = default;
    /// <summary>
    /// Reference to title of instruction
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI title = default;
    /// <summary>
    /// Reference to audio source playing instruction audio
    /// </summary>
    [SerializeField]
    private AudioSource audioSource = default;

    public UIObjectiveController uiObjectiveController;

    /// <summary>
    /// Reference to language controller
    /// </summary>
    private LanguageController languageController = default;
    /// <summary>
    /// Reference to debug text showing current Module, Scenario and Step indexes
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI debugIDText = default;
    /// <summary>
    /// Module index
    /// </summary>
    private int moduleNumber = 0;
    /// <summary>
    /// Scenario index
    /// </summary>
    private int scenarioNumber = 0;
    /// <summary>
    /// Step index
    /// </summary>
    private int stepNumber = 0;
    /// <summary>
    /// Temporary string containing previous text of instruction
    /// </summary>
    private string prevText;

    /// <summary>
    /// Binds methods to events
    /// Gets reference to language controller
    /// Shows and hides debug text if we are in production
    /// </summary>
    private void Start()
    {
        StateModel.OnModuleChanged.AddListener(ModuleChanged);
        StateModel.OnScenarioChanged.AddListener(ScenarioChanged);
        StateModel.OnStepChanged.AddListener(StepChanged);
        languageController = FindObjectOfType<LanguageController>();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        debugIDText.gameObject.SetActive(true);
#else
        debugIDText.gameObject.SetActive(false);
#endif
    }

    /// <summary>
    /// Method called when Module has changed
    /// </summary>
    /// <param name="module">Current Module</param>
    private void ModuleChanged(Module module)
    {
        moduleNumber = FindObjectOfType<ModuleController>().GetCurrentModuleNumber();
    }

    /// <summary>
    /// Method called when Scenario has changed
    /// </summary>
    /// <param name="scenario">Current Scenario</param>
    private void ScenarioChanged(Scenario scenario)
    {
        scenarioNumber = StateModel.currentModule.GetCurrentScenarioNumber();
    }

    /// <summary>
    /// Method called when Step has changed
    /// </summary>
    /// <param name="step">Current Step</param>
    private void StepChanged(Step step)
    {
        stepNumber = StateModel.currentScenario.GetCurrentStepNumber();
        LoadInstruction();
    }

    /// <summary>
    /// Loads instruction into text and setups debug text if needed
    /// </summary>
    public void LoadInstruction(bool forNextStep = true)
    {
        // text.fontSize = 34;
        string scenario = scenarioNumber >= 10 ? scenarioNumber.ToString() : "0" + scenarioNumber;
        string step = stepNumber >= 10 ? stepNumber.ToString() : "0" + stepNumber;
        if (languageController)
        {
            var element = languageController.GetByName("M0" + moduleNumber + "S" + scenario + "S" + step);
            // text.text = element.text;
            if (forNextStep)
            {
                uiObjectiveController.SetText(element.text);
            }
            else
            {
                uiObjectiveController.UpdateTextLanguage(element.text);
            }
            prevText = element.text;
            title.text = element.title;
            FindObjectOfType<MediaController>().LoadMediaStream("M0" + moduleNumber + "S" + scenario + "S" + step);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            debugIDText.text = "M0" + moduleNumber + "S" + scenario + "S" + step;
#endif
        }
    }

    /// <summary>
    /// Manauly set text of step
    /// </summary>
    /// <param name="txt">Text to set</param>
    /// <param name="v">Size of font</param>
    public void SetText(string txt, float v)
    {
        // text.text = txt;
        // text.fontSize = v;
        uiObjectiveController.SetText(txt, 0);
    }

    /// <summary>
    /// Loads and plays audio attached to instruction 
    /// </summary>
    public void LoadAndPlayAudio()
    {
        audioSource.Play();
    }

    /// <summary>
    /// Adds text to time amount
    /// </summary>
    /// <param name="v">Time in a formatted string</param>
    public void AddText(string v)
    {
        string txt = prevText;
        txt += "Twój czas: \n" + v;
        uiObjectiveController.SetText(txt, 0);
    }
}
