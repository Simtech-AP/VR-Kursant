/// <summary>
/// Gazable target for light column
/// </summary>
public class LockBoxGazableTarget : CustomizedGazableTarget
{
    /// <summary>
    /// Sets up references
    /// </summary>
    private void OnEnable()
    {
        objectToGaze = FindObjectOfType<LockBox>().gameObject;
        SetUpGazable();
        SetCustomColliders();
    }
}
