/// <summary>
/// Gazable target for light column
/// </summary>
public class LightColumnGazableTarget : CustomizedGazableTarget
{
    /// <summary>
    /// Sets up references
    /// </summary>
    protected override void initialize()
    {
        objectToGaze = FindObjectOfType<LightColumn>().gameObject;
        SetUpGazable();
        SetCustomColliders();
    }
}
