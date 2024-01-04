using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// State holder of current training
/// </summary>
public class StateModel : MonoBehaviour
{
    /// <summary>
    /// Custom event taking Module as a parameter
    /// </summary>
    public sealed class ModuleEvent : UnityEvent<Module>
    {

    }

    /// <summary>
    /// Event run when module changes
    /// </summary>
    public static ModuleEvent OnModuleChanged = new ModuleEvent();

    /// <summary>
    /// Custom event taking Scenario as a parameter
    /// </summary>
    public sealed class ScenarioEvent : UnityEvent<Scenario>
    {

    }

    /// <summary>
    /// Event run when scenario changes
    /// </summary>
    public static ScenarioEvent OnScenarioChanged = new ScenarioEvent();

    /// <summary>
    /// Custom event taking Step as a parameter
    /// </summary>
    public sealed class StepEvent : UnityEvent<Step>
    {

    }

    /// <summary>
    /// Event run when step changes
    /// </summary>
    public static StepEvent OnStepChanged = new StepEvent();

    /// <summary>
    /// Currently running module
    /// </summary>
    public static Module currentModule = null;
    /// <summary>
    /// Currently running scenario
    /// </summary>
    public static Scenario currentScenario = null;
    /// <summary>
    /// Currently running step
    /// </summary>
    public static Step currentStep = null;
}
