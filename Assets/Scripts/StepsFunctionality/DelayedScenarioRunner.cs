using System.Collections;
using UnityEngine;

/// <summary>
/// Allows running scenario after a delay
/// </summary>
public class DelayedScenarioRunner : MonoBehaviour
{
    /// <summary>
    /// Runs specified scenario after a delay
    /// </summary>
    /// <param name="index">Index of scenario to run</param>
    public void RunDelayedScenario(int index)
    {
        StartCoroutine(DelayScenario(index));
    }

    /// <summary>
    /// Run scenario after delay
    /// </summary>
    /// <param name="index">Index of scenario to run</param>
    /// <returns>Handle to coroutine</returns>
    private IEnumerator DelayScenario(int index)
    {
        yield return new WaitForSeconds(0.2f);
        StateModel.currentModule.RunScenario(index);
    }
}
