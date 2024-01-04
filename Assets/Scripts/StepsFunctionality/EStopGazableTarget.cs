using UnityEngine;

/// <summary>
/// Gazable target for single Estop
/// </summary>
public class EStopGazableTarget : CustomizedGazableTarget
{
    [Header("EStop Gazable Target")]
    [SerializeField] private int EstopId;

    /// <summary>
    /// Sets up references
    /// </summary>
    protected override void initialize()
    {
        var estops = InteractablesManager.Instance.GetAllInteractableBehaviour<EStop>();

        objectsToGaze.Add(estops[EstopId].transform.Find("Visual").gameObject);

        SetUpGazables();
        SetCustomColliders();
    }
}
