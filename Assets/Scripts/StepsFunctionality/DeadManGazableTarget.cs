/// <summary>
/// Gazable target for dead man on pilot
/// </summary>
public class DeadManGazableTarget : GazableTarget
{
    /// <summary>
    /// Enable going to next step
    /// </summary>
    private void OnEnable()
    {
        Enabled = true;
    }

    /// <summary>
    /// Sets up target
    /// </summary>
    public void EnableGazableTarget()
    {
        SetUpGazable();
    }
}
