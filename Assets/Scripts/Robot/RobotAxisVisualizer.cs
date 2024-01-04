using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows for a visualization of axes helpers
/// </summary>
public class RobotAxisVisualizer : MonoBehaviour
{
    /// <summary>
    /// Reference to visual aid object
    /// </summary>
    private RobotVisualAid robotVisualAid;
    /// <summary>
    /// Movement type the robot is currently in
    /// </summary>
    [SerializeField]
    private MovementType movementType = default;
    /// <summary>
    /// All axes and buttons for them in order
    /// </summary>
    private Dictionary<Tuple<string, string>, int> axisAid = new Dictionary<Tuple<string, string>, int>()
    {
        { new Tuple<string, string>("+X", "-X"), 1 },
        { new Tuple<string, string>("+Y", "-Y"), 2 },
        { new Tuple<string, string>("+Z", "-Z"), 3 },
        { new Tuple<string, string>("+X rot", "-X rot"), 4 },
        { new Tuple<string, string>("+Y rot", "-Y rot"), 5 },
        { new Tuple<string, string>("+Z rot", "-Z rot"), 6 }
    };

    /// <summary>
    /// Sets up references
    /// </summary>
    public void Awake()
    {
        robotVisualAid = FindObjectOfType<RobotController>().CurrentRobot.GetComponent<RobotVisualAid>();
    }

    /// <summary>
    /// Enables visualization of axis helper
    /// </summary>
    /// <param name="axisIndex">Index of axis we want to show</param>
    public void VisualizeAxis(int axisIndex)
    {
        CleanUpAxis();
        if (movementType == MovementType.Joint)
        {
            robotVisualAid.ShowJointAid(axisIndex);
        }
        else if (movementType == MovementType.Base)
        {
            robotVisualAid.ShowBaseAid();
        }
        else if (movementType == MovementType.Tool)
        {
            robotVisualAid.ShowToolAid();
        }
    }

    /// <summary>
    /// Shows all axis helpers
    /// </summary>
    public void ShowAllAxis()
    {
        robotVisualAid.ShowAllJointAides();
        robotVisualAid.HideAid(1);
    }

    /// <summary>
    /// Hides all axis helpers
    /// </summary>
    public void CleanUpAxis()
    {
        robotVisualAid.HideAllAides();
    }

    public void HideAxis(int axisIndex)
    {
        robotVisualAid.HideAid(axisIndex);
    }

    /// <summary>
    /// Hides axis
    /// </summary>
    /// <param name="axis">Tuple containing information about which axis we want to hide</param>
    public void HideAxis(Tuple<string, string> axis)
    {
        robotVisualAid.HideAid(axisAid[axis]);
    }
}
