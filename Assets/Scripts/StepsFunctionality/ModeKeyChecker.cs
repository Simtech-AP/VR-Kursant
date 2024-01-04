using UnityEngine;

/// <summary>
/// Allows checking for specified mode key state
/// </summary>
public class ModeKeyChecker : StepEnabler
{
    /// <summary>
    /// Reference to outline 
    /// </summary>
    private QuickOutline.Outline objectToHighlightOutline;
    /// <summary>
    /// Reference to mode key
    /// </summary>
    private ModeKey modeKey;
    /// <summary>
    /// Specified movement mode
    /// </summary>
    [Header("Mode Key Checker")]
    [SerializeField]
    private MovementMode targetMode = default;

    /// <summary>
    /// Sets up references
    /// </summary>
    protected override void initialize()
    {
        modeKey = InteractablesManager.Instance.GetInteractableBehaviour<ModeKey>();
        SetUpModeKey();
    }

    /// <summary>
    /// Sets up start state
    /// </summary>
    public void SetUpModeKey()
    {
        if (modeKey.gameObject.GetComponent<QuickOutline.Outline>() == null)
            objectToHighlightOutline = modeKey.gameObject.AddComponent<QuickOutline.Outline>();
        Enabled = false;
    }

    /// <summary>
    /// Cleans up outline
    /// </summary>
    public void CleanUpModeKey()
    {
        Destroy(objectToHighlightOutline);
    }

    /// <summary>
    /// Finishes step
    /// </summary>
    public void EndStep()
    {
        Enabled = true;
        CleanUpModeKey();
    }

    /// <summary>
    /// Checks for specified condition
    /// </summary>
    protected override void onUpdate()
    {
        CheckMovementModeCondition();
    }

    private void CheckMovementModeCondition()
    {
        if (RobotData.Instance.MovementMode == targetMode)
        {
            EndStep();
        }
        else
        {
            SetUpModeKey();
        }
    }

    protected override void cleanUp()
    {
        CleanUpModeKey();
    }
}
