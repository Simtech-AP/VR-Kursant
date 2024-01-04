/// <summary>
/// Gazable target for desktop on scene
/// </summary>
public class DesktopGazableTarget : CustomizedGazableTarget
{
    /// <summary>
    /// Sets up references
    /// </summary>
    protected override void initialize()
    {
        objectToGaze = ControllersManager.Instance.GetController<RobotConsoleController>().gameObject;
        SetUpGazable();
        SetCustomColliders();
    }
}
