using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contains logic for scenario
/// </summary>
public class Scenario : MonoBehaviour
{
    /// <summary>
    /// List of steps in scenario
    /// </summary>
    [SerializeField]
    private List<Step> steps = new List<Step>();
    /// <summary>
    /// Event called when a module starts
    /// </summary>
    public UnityEvent OnScenarioStart = null;
    /// <summary>
    /// Event called when module ends
    /// Usually used to clean up module objects and elements
    /// </summary>
    public UnityEvent OnScenarioEnd = null;

    /// <summary>
    /// Loads steps into list
    /// </summary>
    [ContextMenu("Load steps")]
    private void LoadSteps()
    {
        steps.Clear();
        var tab = GetComponentsInChildren<Step>(true);
        for(int i = 0; i< tab.Length; ++i)
        {
            steps.Add(tab[i]);
        }
    }

    /// <summary>
    /// Updates state of current scenario
    /// </summary>
    public void UpdateState()
    {
        if (StateModel.OnScenarioChanged != null)
            StateModel.OnScenarioChanged.Invoke(this);
    }

    /// <summary>
    /// Runs next step from list
    /// </summary>
    public void NextStep()
    {
        RunStep(steps.IndexOf(StateModel.currentStep) + 1);
    }

    /// <summary>
    /// Runs step with specified index from list
    /// </summary>
    /// <param name="index">Index of step to run</param>
    public void RunStep(int index)
    {
        if (index >= 0 && index < steps.Count)
        {
            if (StateModel.currentStep)
            {
                StateModel.currentStep.OnStepEnd.Invoke();
                StateModel.currentStep.gameObject.SetActive(false);
            }
            StateModel.currentStep = steps[index];
            StateModel.currentStep.UpdateState();
            StateModel.currentStep.gameObject.SetActive(true);
            StateModel.currentStep.OnStepStart.Invoke();
        }
        else
        {
            var lastStep = StateModel.currentStep;
            StateModel.currentStep = null;
            lastStep.OnStepEnd.Invoke();
        }
    }

    /// <summary>
    /// Runs specified step
    /// </summary>
    /// <param name="step">Step to run</param>
    public void RunStep(Step step)
    {
        if (steps.Contains(step))
        {
            if (StateModel.currentStep)
            {
                StateModel.currentStep.OnStepEnd.Invoke();
                StateModel.currentStep.gameObject.SetActive(false);
            }
            StateModel.currentStep = step;
            StateModel.currentStep.UpdateState();
            StateModel.currentStep.gameObject.SetActive(true);
            StateModel.currentStep.OnStepStart.Invoke();
        }
    }

    /// <summary>
    /// Return currently running step
    /// </summary>
    /// <returns>Index of currently running step plus one (human readable)</returns>
    public int GetCurrentStepNumber()
    {
        return steps.IndexOf(StateModel.currentStep) + 1;
    }
}
