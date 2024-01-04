using UnityEngine;

/// <summary>
/// Conatins information for detecting limits
/// </summary>
[System.Serializable]
public class LimitInfo
{
    /// <summary>
    /// Axis that has limits
    /// </summary>
    public Transform limitAxis;
    /// <summary>
    /// Limit of an axis
    /// </summary>
    public Limit limit;
}

/// <summary>
/// Helper class for determining if any axis has reached its limit
/// </summary>
public class RobotAngleLimitChecker : StepEnabler
{
    /// <summary>
    /// Tested limit information
    /// </summary>
    public LimitInfo limitInfo = default;
    /// <summary>
    /// Refrence to material switcher
    /// </summary>
    [SerializeField]
    private MaterialSwitcher materialSwitcher = default;

    /// <summary>
    /// Attaches check method to an event
    /// </summary>
    protected override void initialize()
    {
        ClampRobotLimitAngle.LimitReached += LimitReached;
    }

    /// <summary>
    /// Signifies that limit has been reached
    /// </summary>
    /// <param name="axis">Axis that reached the limit</param>
    /// <param name="limit">Limit type</param>
    private void LimitReached(Transform axis, Limit limit)
    {
        limitInfo.limitAxis = axis;
        limitInfo.limit = limit;
        Debug.Log("Unhandled material switch");
        materialSwitcher.SwitchMaterial(limitInfo.limitAxis, Color.red);
        Enabled = true;
    }

    /// <summary>
    /// Removes checking method from event
    /// </summary>
    protected override void cleanUp()
    {
        ClampRobotLimitAngle.LimitReached -= LimitReached;
    }

}
