using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls loigc of gazable targets
/// </summary>
public class GazableTargetsController : StepEnabler
{
    [Header("Gazable Targets Controller")]
    /// <summary>
    /// Gazable targets list
    /// </summary>
    [SerializeField]
    private List<GameObject> targets = new List<GameObject>();
    [SerializeField] private float unlockWaitTime = 0f;
    /// <summary>
    /// Timer for unlocking functionality waiting
    /// </summary>
    private WaitForSeconds unlockWaitTimer;

    protected override void initialize()
    {
        unlockWaitTimer = new WaitForSeconds(unlockWaitTime);
    }

    /// <summary>
    /// Check if all the targets were gazed upon
    /// </summary>
    public void CheckTargets()
    {
        foreach (GameObject go in targets)
        {
            if (go.activeInHierarchy) return;
        }
        StartCoroutine(Unlock());
    }

    /// <summary>
    /// Unlocks continuation after a specified time
    /// </summary>
    /// <returns>Handle to coroutine</returns>
    IEnumerator Unlock()
    {
        yield return unlockWaitTimer;
        Enabled = true;
    }
}
