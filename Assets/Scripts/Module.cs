using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Main class for a single application module
/// </summary>
public class Module : MonoBehaviour
{
    /// <summary>
    /// List of steps inside a module
    /// </summary>
    [SerializeField]
    private List<Scenario> scenarios = new List<Scenario>();
    /// <summary>
    /// Event called when a module starts
    /// </summary>
    public UnityEvent OnModuleStart = null;
    /// <summary>
    /// Event called when module ends
    /// Usually used to clean up module objects and elements
    /// </summary>
    public UnityEvent OnModuleEnd = null;

    /// <summary>
    /// Runs event updating all listeners with new data
    /// </summary>
    public void UpdateState()
    {
        if (StateModel.OnModuleChanged != null)
            StateModel.OnModuleChanged.Invoke(this);
    }

    /// <summary>
    /// Runs next step if available
    /// </summary>
    public void NextScenario()
    {
        RunScenario(scenarios.IndexOf(StateModel.currentScenario) + 1);
    }

    /// <summary>
    /// Runs step based on provided index
    /// </summary>
    /// <param name="index">Index of a step </param>
    /// <param name="runScenarioEnd">Should we run ending scenario event?</param>
    public void RunScenario(int index, bool runScenarioEnd = true)
    {
        if (index >= 0 && index < scenarios.Count)
        {
            if (StateModel.currentScenario && runScenarioEnd)
                StateModel.currentScenario.OnScenarioEnd.Invoke();
            StateModel.currentScenario = scenarios[index];
            StateModel.currentScenario.UpdateState();
            StateModel.currentScenario.OnScenarioStart.Invoke();
        }
        else
        {
            var lastScenario = StateModel.currentScenario;
            StateModel.currentScenario = null;
            lastScenario.OnScenarioEnd.Invoke();
        }
    }

    /// <summary>
    /// Runs scenario using provided Scenario object
    /// </summary>
    /// <param name="scenario">Scenario object to run</param>
    public void RunScenario(Scenario scenario)
    {
        if (scenarios.Contains(scenario))
        {
            if (StateModel.currentScenario)
                StateModel.currentScenario.OnScenarioEnd.Invoke();
            StateModel.currentScenario = scenario;
            StateModel.currentScenario.UpdateState();
            StateModel.currentScenario.OnScenarioStart.Invoke();
        }
        else if (scenario == null)
        {
            var lastScenario = StateModel.currentScenario;
            StateModel.currentScenario = null;
            lastScenario.OnScenarioEnd.Invoke();
        }
    }

    /// <summary>
    /// Get currently loaded scenario number
    /// </summary>
    /// <returns>Currently loaded scenario number</returns>
    public int GetCurrentScenarioNumber()
    {
        return scenarios.IndexOf(StateModel.currentScenario) + 1;
    }
}
