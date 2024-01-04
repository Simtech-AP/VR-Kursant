using UnityEngine;

/// <summary>
/// Helper class for checking if axe has reached specified rotation
/// </summary>
public class AxisAngleChecker : StepEnabler
{
    /// <summary>
    /// Maximum angle of selected axis
    /// </summary>
    public float maxAxisAngle = 2f;
    /// <summary>
    /// Reference to visual axis helper
    /// </summary>
    private RobotVisualAxis robotVisualAxis;
    /// <summary>
    /// Reference to first axis of robot
    /// </summary>
    private Transform firstAxis;
    /// <summary>
    /// Current axis index of robot
    /// </summary>
    public int AxisIndex;
    /// <summary>
    /// Are we in range of movement of axis?
    /// </summary>
    public bool inRange = true;

    /// <summary>
    /// Sets up references
    /// </summary>
    private void Awake()
    {
        robotVisualAxis = FindObjectOfType<RobotController>().CurrentRobot.GetComponent<RobotVisualAxis>();
    }

    /// <summary>
    /// Sets up reference to first robot axis
    /// </summary>
    public void SetUpAxis()
    {
        firstAxis = robotVisualAxis.GetAxis(AxisIndex).transform.parent;
    }

    /// <summary>
    /// Checks if we reached axis limit
    /// </summary>
    private void Update()
    {
        Vector3 angles = firstAxis.transform.localRotation.eulerAngles;
        float y = ((angles.z + 540) % 360) - 180;

        if (inRange)
        {
            if (y < maxAxisAngle && y > -maxAxisAngle)
            {
                EndStep();
            }
            else
            {
                Enabled = false;
            }
        }
        else
        {          
            if ((y > maxAxisAngle || y < -maxAxisAngle) && !(y < maxAxisAngle && y > -maxAxisAngle))
            {
                EndStep();
            }
            else if(y < maxAxisAngle && y > -maxAxisAngle)
            {
                Enabled = false;
            }
        }
    }

    /// <summary>
    /// Finishes step
    /// </summary>
    private void EndStep()
    {
        Enabled = true;
    }
}
