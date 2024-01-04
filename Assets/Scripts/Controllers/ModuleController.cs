using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controller in charge of module management
/// </summary>
public class ModuleController : Controller
{
    /// <summary>
    /// List of all of the modules
    /// </summary>
    [SerializeField]
    private List<string> modules = new List<string>();
    /// <summary>
    /// Current module name
    /// </summary>
    private string currentModule = "";

    /// <summary>
    /// Runs next module if available
    /// </summary>
    public void NextModule()
    {
        RunModule(modules.IndexOf(currentModule) + 1);
    }

    /// <summary>
    /// Runs module based on provided index
    /// </summary>
    /// <param name="index">Index of a module</param>
    public void RunModule(int index)
    {
        if (StateModel.currentModule)
        {
            StateModel.currentModule.OnModuleEnd.Invoke();
            SceneManager.UnloadSceneAsync(currentModule);
        }
        if (index >= 0 && index < modules.Count)
        {
            SceneManager.LoadSceneAsync(modules[index], LoadSceneMode.Additive).completed += (AsyncOperation a) =>
            {
                StartCoroutine(SetModuleState());
                currentModule = modules[index];
            };
        }
    }

    /// <summary>
    /// Updates module satate after loading a scene containing module
    /// </summary>
    /// <returns>Coroutine Enumerator</returns>
    private IEnumerator SetModuleState()
    {
        yield return new WaitForSeconds(0.1f);
        var loadedModules = FindObjectsOfType<Module>();
        StateModel.currentModule = loadedModules[loadedModules.Length - 1];
        StateModel.currentModule.UpdateState();
        StateModel.currentModule.OnModuleStart.Invoke();
    }

    /// <summary>
    /// Runs module based on provided module name
    /// </summary>
    /// <param name="module">Module name</param>
    public void RunModule(string module)
    {
        if (StateModel.currentModule)
        {
            StateModel.currentModule.OnModuleEnd.Invoke();
            SceneManager.UnloadSceneAsync(currentModule);
        }
        SceneManager.LoadSceneAsync(module, LoadSceneMode.Additive).completed += (AsyncOperation a) =>
        {
            var loadedModules = FindObjectsOfType<Module>();
            if (loadedModules.Length <= 0) return;
            StateModel.currentModule = loadedModules[loadedModules.Length - 1];
            StateModel.currentModule.UpdateState();
            StateModel.currentModule.OnModuleStart.Invoke();
            currentModule = module;
        };
    }

    /// <summary>
    /// Returns current module index
    /// </summary>
    /// <returns>Module index in list</returns>
    public int GetCurrentModuleNumber()
    {
        return modules.IndexOf(currentModule) != -1 ? modules.IndexOf(currentModule) : 1;
    }

    //DEBUG------------------------------------------------------------------------------------------
#if UNITY_EDITOR
    /// <summary>
    /// Debug keypresses to check going to next step and next scenario/module
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) FindObjectOfType<ContinueFrameController>().GoToNextStep();
        if (Input.GetKeyDown(KeyCode.AltGr)) StateModel.currentStep.OnStepEnd.Invoke();
    }
#endif
}
