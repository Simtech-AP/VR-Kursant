using UnityEngine;

/// <summary>
/// Allows checking for cell entrance and lock interaction
/// </summary>
public class LockEntranceChecker : StepEnabler
{
    /// <summary>
    /// Outline of padlock
    /// </summary>
    private QuickOutline.Outline padLockToHighliteOutline;
    /// <summary>
    /// Outline of cell entrance
    /// </summary>
    private QuickOutline.Outline cellLockToHighliteOutline;
    /// <summary>
    /// Reference to lock of cell entrance
    /// </summary>
    private CellLock cellLock;
    /// <summary>
    /// Reference to padlock
    /// </summary>
    private PadLock padLock;

    [Header("Lock Entrance Checker")]
    [SerializeField] private int targetState = PadLockState.OnDoor;


    /// <summary>
    /// Sets up references
    /// </summary>
    protected override void initialize()
    {
        padLock = InteractablesManager.Instance.GetInteractableBehaviour<PadLock>();
        cellLock = FindObjectOfType<CellLock>();
        SetUpLockEntrance();
    }

    /// <summary>
    /// Sets up outlines
    /// </summary>
    public void SetUpLockEntrance()
    {
        if (padLock.gameObject.GetComponent<QuickOutline.Outline>() == null)
            padLockToHighliteOutline = padLock.gameObject.AddComponent<QuickOutline.Outline>();
        if (cellLock.gameObject.GetComponent<QuickOutline.Outline>() == null)
            cellLockToHighliteOutline = cellLock.gameObject.AddComponent<QuickOutline.Outline>();
    }

    /// <summary>
    /// Checks for padlock state
    /// </summary>
    protected override void onUpdate()
    {
        LockConditionChecker();
    }

    private void LockConditionChecker()
    {
        if (CellStateData.padLockState == targetState)
        {
            Enabled = true;
        }
        else
        {
            Enabled = false;
        }
    }

    /// <summary>
    /// Cleans up outlines
    /// </summary>
    public void CleanUpLockEntrance()
    {
        Destroy(padLockToHighliteOutline);
        Destroy(cellLockToHighliteOutline);
    }

    protected override void cleanUp()
    {
        CleanUpLockEntrance();
    }
}
