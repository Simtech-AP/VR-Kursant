using System;
using UnityEngine;

/// <summary>
/// Limits enumeration
/// </summary>
public enum Limit
{
    UPPER,
    LOWER
}

/// <summary>
/// Class for logic of clamping rotations of axes of robot 
/// </summary>
public class ClampRobotLimitAngle : MonoBehaviour
{
    /// <summary>
    /// Reference to visual aides object
    /// </summary>
    [SerializeField]
    private RobotVisualAxis robotVisualAxis = default;
    /// <summary>
    /// Event fired when one of limits are reached
    /// </summary>
    public static Action<Transform, Limit> LimitReached = default;
    /// <summary>
    /// Is robot in limit?
    /// </summary>
    private bool robotInLimit = false;
    public int PivotErrorIndex;

    public static Action<int> newLimitReached = default;
    public static Action<int> newLimitReset = default;

    /// <summary>
    /// Sets limit and raises error for limit reached
    /// </summary>
    /// <param name="index">Index of axis that reached limit</param>
    /// <param name="limitFlag">Is the limit upper (false) or lower (true)?</param>
    public void SetLimit(int index, bool limitFlag)
    {
        if (!robotInLimit)
        {
            robotInLimit = true;
            PivotErrorIndex = index;
            LimitReached?.Invoke(robotVisualAxis.GetAxis(index + 1).transform, limitFlag == true ? Limit.LOWER : Limit.UPPER);
            newLimitReached?.Invoke(index);
            ErrorRequester.RaiseError("R-1003");
        }
    }

    /// <summary>
    /// Resets reached limit status
    /// </summary>
    public void ResetLimit()
    {
        robotInLimit = false;
    }

    public void ResetLimit(int index)
    {
        robotInLimit = false;
        newLimitReset?.Invoke(index);
    }

    public void ResetOnlyLimitFlag()
    {
        robotInLimit = false;
    }

    public Transform GetAxisTransform(int axisIndex)
    {
        return robotVisualAxis.GetAxis(axisIndex + 1).transform;
    }
}
