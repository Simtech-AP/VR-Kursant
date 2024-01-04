/// <summary>
/// Gazable target for cell entrance
/// </summary>
public class CellEntranceGazableTarget : CustomizedGazableTarget
{
    /// <summary>
    /// Gets reference to cell entrance
    /// </summary>
    protected override void initialize()
    {
        objectToGaze = InteractablesManager.Instance.GetInteractableBehaviour<CellEntrance>().transform.GetChild(0).gameObject;
        SetUpGazable();
        SetCustomColliders();
    }

}
