using UnityEngine;
/// <summary>
/// Allows for robot visual detection
/// </summary>
public class RobotGazableTarget : CustomizedGazableTarget
{
    [Header("Robot Gazable Target")]
    [SerializeField] private bool SetUpOnInit = false;
    /// <summary>
    /// Finds robot on scene
    /// </summary>
    protected override void initialize()
    {
        if (SetUpOnInit == true)
        {
            SetUpGazableRobot();
        }
    }

    public void SetUpGazableRobot()
    {
        objectToGaze = FindObjectOfType<RobotController>().CurrentRobot.gameObject;
        SetUpGazable();
        SetCustomColliders();
    }
}
