using System;
using UnityEngine;

/// <summary>
/// Class for checking if robot reached singularity
/// </summary>
public class ClampRobotSingularityAngle : MonoBehaviour
{
    /// <summary>
    /// Event called when singularity is reached
    /// </summary>
    public static Action SingularityReached;
    /// <summary>
    /// Is robot in axes angle limit?
    /// </summary>
    private bool isRobotInLimit = false;

    /// <summary>
    /// Sets robot in singualrity and raises error
    /// </summary>
    public void SetSingularity()
    {
        if (!isRobotInLimit)
        {
            isRobotInLimit = true;
            SingularityReached?.Invoke();
            ErrorRequester.RaiseError("R-1004");
        }
    }

    /// <summary>
    /// Resets robot singularity flag
    /// </summary>
    public void ResetSingularity()
    {
        isRobotInLimit = false;
    }
}
