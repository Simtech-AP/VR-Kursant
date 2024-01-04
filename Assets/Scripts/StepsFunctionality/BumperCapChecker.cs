using UnityEngine;

/// <summary>
/// Allows checking of bumper cap state
/// </summary>
public class BumperCapChecker : StepEnabler
{
    /// <summary>
    /// Reference to outline
    /// </summary>
    private QuickOutline.Outline objectToHighlightOutline;
    /// <summary>
    /// Reference to bumper cap object
    /// </summary>
    private BumperCap bumperCap;

    private bool enabledOnce = false;

    [Header("Bumper Cap Checker")]
    [SerializeField] private int targetState = BumperCapState.OnHead;

    /// <summary>
    /// Sets up references
    /// </summary>
    protected override void initialize()
    {
        bumperCap = InteractablesManager.Instance.GetInteractableBehaviour<BumperCap>();
        SetUpBumperCap();
    }

    /// <summary>
    /// Highlights bumper cap and disables continue button
    /// </summary>
    public void SetUpBumperCap()
    {
        if (bumperCap.gameObject.GetComponent<QuickOutline.Outline>() == null)
            objectToHighlightOutline = bumperCap.gameObject.AddComponent<QuickOutline.Outline>();
        Enabled = false;
    }

    /// <summary>
    /// Destroys outline
    /// </summary>
    public void CleanUpBumperCap()
    {
        Destroy(objectToHighlightOutline);
    }

    /// <summary>
    /// Checks for bumper cap state
    /// </summary>
    protected override void onUpdate()
    {
        CheckBumperCapState();
    }

    private void CheckBumperCapState()
    {
        if (CellStateData.bumpCapState == targetState)
        {
            Enabled = true;
            enabledOnce = true;
            objectToHighlightOutline.enabled = true;
        }
        else
        {
            Enabled = false;
            if (enabledOnce)
            {
                objectToHighlightOutline.enabled = false;
            }
        }
    }

    protected override void cleanUp()
    {
        CleanUpBumperCap();
    }
}
