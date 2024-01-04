using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Enables continuation for next step
/// </summary>
public class StepSwitcher : MonoBehaviour
{
    /// <summary>
    /// List of all step enabler in a step
    /// </summary>
    private List<StepEnabler> stepEnablers;
    /// <summary>
    /// Reference to continue enabler
    /// </summary>
    [SerializeField]
    private ContinueListener continueListener = default;

    /// <summary>
    /// Finds all step enablers
    /// </summary>
    public void ProcessStepEnablers()
    {
        Step step = FindObjectOfType<Step>();
        stepEnablers = step.GetComponentsInChildren<StepEnabler>().ToList();

        foreach (var enabler in stepEnablers)
        {
            enabler.Initialize();
        }
    }

    /// <summary>
    /// Cleans step enablers from list
    /// </summary>
    public void CleanStepEnablers()
    {
        foreach (var enabler in stepEnablers)
        {
            enabler.CleanUp();
        }
        stepEnablers.Clear();
    }

    /// <summary>
    /// Checks if we completed all step enablers tasks
    /// </summary>
    private void Update()
    {
        if (stepEnablers != null)
        {
            if (stepEnablers.Count > 0)
            {
                if (stepEnablers.Any(x => x.Enabled == false))
                {
                    continueListener.DisableContinuation();
                }
                else
                {
                    continueListener.EnableContinuation();
                }
            }
        }
    }
}
