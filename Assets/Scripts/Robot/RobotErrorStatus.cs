using UnityEngine;

/// <summary>
/// Contains status of error in robot
/// </summary>
public class RobotErrorStatus : MonoBehaviour
{
    /// <summary>
    /// Reference to linked robot
    /// </summary>
    private Robot robot = default;

    /// <summary>
    /// Links robot to this object
    /// </summary>
    /// <param name="robot">Robot to link to</param>
    public void LinkRobot(Robot robot)
    {
        this.robot = robot;
    }

    /// <summary>
    /// Sets up listeners
    /// </summary>
    private void OnEnable()
    {
        ErrorController.OnErrorReset += () => SetError(false);
        ErrorController.OnErrorOccured += (ErrorController controller, Error er) => { SetError(true); };
    }

    /// <summary>
    /// Removes listeners
    /// </summary>
    private void OnDisable()
    {
        ErrorController.OnErrorReset -= () => SetError(false);
        ErrorController.OnErrorOccured -= (ErrorController controller, Error er) => { SetError(true); };
    }

    /// <summary>
    /// Sets flag to specified status
    /// </summary>
    /// <param name="flag">Flag status</param>
    public void SetError(bool flag)
    {
        robot.ErrorDetected = flag;
    }
}
