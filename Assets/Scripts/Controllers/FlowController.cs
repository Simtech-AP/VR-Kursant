using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Valve.VR;
using System.Globalization;
using System.Linq;

/// <summary>
/// Main flow controller for application, manages other controllers
/// </summary>
public class FlowController : Controller
{
    /// <summary>
    /// Reference to module controller object
    /// </summary>
    [SerializeField]
    private ModuleController moduleController = default;
    /// <summary>
    /// Reference to settings controller object
    /// </summary>
    [SerializeField]
    private SettingsController settingsController = default;
    /// <summary>
    /// Reference to connection controller object
    /// </summary>
    [SerializeField]
    private ConnectionController connectionController = default;
    /// <summary>
    /// Reference to analytics controller object
    /// </summary>
    [SerializeField]
    private AnalyticsController analyticsController = default;
    /// <summary>
    /// Reference to UI controller object
    /// </summary>
    [SerializeField]
    private UIController uiController = default;
    /// <summary>
    /// Template action for running scenario after loading module
    /// </summary>
    private UnityAction<Module> loadScenario = default;

    public int moduleIndexToRun = 1;

    /// <summary>
    /// Sets up every controller at the start of an application
    /// Binds received messages callback
    /// Loads first module
    /// </summary>
    private void OnApplicationStart()
    {
        connectionController.ConnectToServerApplication();
        settingsController.LoadSettings();
        connectionController.OnMessageRecieved += ProcessMessage;
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
        moduleController.RunModule(moduleIndexToRun);
    }

    /// <summary>
    /// Executes methods when appication should quit
    /// </summary>
    private void OnApplicationExit()
    {
        //analyticsController.SaveData();
        settingsController.SaveSettings();
    }

    /// <summary>
    /// Executes when application starts
    /// </summary>
    private void Start()
    {
        OnApplicationStart();
    }

    /// <summary>
    /// Executes when application quits
    /// </summary>
    private void OnApplicationQuit()
    {
        OnApplicationExit();
    }

    /// <summary>
    /// Allows processing of messages sent from server
    /// </summary>
    /// <param name="data">Message sent by server</param>
    private void ProcessMessage(string data)
    {
        switch (data)
        {
            case string s when s.StartsWith("runScenario"):
                moduleController.RunModule(int.Parse(s.Replace("runScenario", "")[0].ToString()));
                loadScenario = (module) => LoadScenarioInModule(int.Parse(s.Split('-')[1]));
                StateModel.OnModuleChanged.AddListener(loadScenario);
                break;
            case string s when s.StartsWith("module"):
                moduleController.RunModule(int.Parse(s.Replace("module", "")));
                break;
            case string s when s.StartsWith("scenario"):
                StateModel.currentModule.RunScenario(int.Parse(s.Replace("scenario", "")) - 1);
                break;
            case string s when s.StartsWith("step"):
                StateModel.currentScenario.RunStep(int.Parse(s.Replace("step", "")) - 1);
                break;
            case string s when s.StartsWith("next"):
                moduleController.NextModule();
                break;
            case string s when s.StartsWith("watch"):
                uiController.SetWatchingIndicator(true);
                break;
            case string s when s.StartsWith("stop"):
                uiController.SetWatchingIndicator(false);
                break;
            case string s when s.StartsWith("language"):
                if (Enum.TryParse(data.Split(':')[1], out Language language))
                    FindObjectOfType<LanguageController>().ChangeLanguage(language);
                break;
            case string s when s.StartsWith("tracker"):
                if (int.TryParse(data.Split(':')[1], NumberStyles.Any, CultureInfo.InvariantCulture, out int level))
                    ControllersManager.Instance.GetController<TrackerController>().trackerBatteryLevelToCharge = (level);
                break;
            case string s when s.StartsWith("pilotID"):
                if (s.Split(':')[1].Length > 0)
                {
                    FindObjectOfType<TcpReceiver>().pilotIpEnd = int.Parse(s.Split(':')[1]);
                }
                else
                {
                    FindObjectOfType<TcpReceiver>().pilotIpEnd = 0;
                }
                break;
            case string s when s.StartsWith("toggleHints"):
                var message = s.Split(':');
                var aidesController = ControllersManager.Instance.GetController<AidesController>();
                var aidType = (HintType)(Int32.Parse(message[1]));
                if (message[2] == "True")
                {
                    aidesController.SetStateForAidType(aidType, true);
                }
                else
                {
                    aidesController.SetStateForAidType(aidType, false);
                }
                break;
            case string s when s.StartsWith("lectorVolume"):
                var payload = s.Split(':');
                float volumeValue = float.Parse(payload[1]);
                ControllersManager.Instance.GetController<LanguageController>().ChangeLectorVolume(volumeValue);
                break;
        }
    }

    /// <summary>
    /// Loads set scenario in module, used with loading new modules through trainer application with delay
    /// </summary>
    /// <param name="scenarioNumber">Index of scenario to load</param>
    private void LoadScenarioInModule(int scenarioNumber)
    {
        StartCoroutine(LoadScenarioDelayed(scenarioNumber));
    }

    /// <summary>
    /// Loads set scenario with delay
    /// </summary>
    /// <param name="scenarioNumber">Index of scenario to load</param>
    /// <returns>Template Enumerator</returns>
    private IEnumerator LoadScenarioDelayed(int scenarioNumber)
    {
        yield return new WaitForSeconds(0.04f);
        StateModel.currentModule.RunScenario(scenarioNumber - 1, false);
        StateModel.OnModuleChanged.RemoveListener(loadScenario);
    }
}
