using UnityEngine;

/// <summary>
/// Checks if end point of tool is perpendicular to base
/// </summary>
public class PerpendicularityChecker : StepEnabler
{
    /// <summary>
    /// Reference to tool end point
    /// </summary>
    private Transform endPoint;
    /// <summary>
    /// Reference to robot base
    /// </summary>
    private Transform robotBase;

    private RobotController robotController;

    /// <summary>
    /// Sets up references
    /// </summary>
    protected override void initialize()
    {
        robotController = ControllersManager.Instance.GetController<RobotController>();
        endPoint = robotController.CurrentRobot.RobotEndPoint.transform;
        robotBase = robotController.CurrentRobot.GetComponent<RobotVisualAid>().GetBaseAid().transform.parent;
    }

    /// <summary>
    /// Checks if conditions has been met
    /// </summary>
    protected override void onUpdate()
    {
        CheckPerpendicularity();
    }

    private void CheckPerpendicularity()
    {
        float dot = Vector3.Dot(-endPoint.transform.forward.normalized, robotBase.transform.forward.normalized);
        if (dot > -0.3 && dot < 0.3)
            Enabled = true;
        else
            Enabled = false;
    }
}
