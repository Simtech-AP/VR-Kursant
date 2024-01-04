/// <summary>
/// Gazable target for control cabinet
/// </summary>
public class ControlCabinetGazableTarget : CustomizedGazableTarget
{
    /// <summary>
    /// Sets up references
    /// </summary>
    protected override void initialize()
    {
        objectToGaze = FindObjectOfType<ControlCabinet>().gameObject;
        SetUpGazable();
        SetCustomColliders();
    }
}
