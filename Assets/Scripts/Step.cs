using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base class for single step in module
/// </summary>
public class Step : MonoBehaviour
{
    /// <summary>
    /// Events called at the start of a step
    /// </summary>
    public UnityEvent OnStepStart;
    /// <summary>
    /// Events called ath the end of the step
    /// </summary>
    public UnityEvent OnStepEnd;

    /// <summary>
    /// Updates data with new step information
    /// </summary>
    public void UpdateState()
    {
        if (StateModel.OnStepChanged != null)
            StateModel.OnStepChanged.Invoke(this);
    }
}
