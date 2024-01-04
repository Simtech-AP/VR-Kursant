/// <summary>
/// Gazable target for all estops
/// </summary>
public class EStopsGazableTarget : CustomizedGazableTarget
{
    /// <summary>
    /// Sets up references
    /// </summary>
    protected override void initialize()
    {
        var estops = InteractablesManager.Instance.GetAllInteractableBehaviour<EStop>();
        for (int i = 0; i < estops.Length; ++i)
        {
            objectsToGaze.Add(estops[i].gameObject);
        }
        SetUpGazables();
        SetCustomColliders();
    }
}
