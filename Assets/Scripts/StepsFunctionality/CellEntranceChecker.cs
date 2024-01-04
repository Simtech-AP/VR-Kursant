using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows checking of cell entrance state
/// </summary>
public class CellEntranceChecker : StepEnabler
{
    /// <summary>
    /// Reference to outline
    /// </summary>
    private QuickOutline.Outline objectToHighlightOutline;
    /// <summary>
    /// Reference to cell entrance on scene
    /// </summary>
    private CellEntrance cellEntance;

    [Header("Cell Entrance Checker")]
    [SerializeField] private List<int> targetStates = new List<int>();

    /// <summary>
    /// Sets up references
    /// </summary>
    protected override void initialize()
    {
        cellEntance = InteractablesManager.Instance.GetInteractableBehaviour<CellEntrance>();
        SetUpCellEntrance();
    }

    /// <summary>
    /// Adds outline to entrance
    /// </summary>
    public void SetUpCellEntrance()
    {
        objectToHighlightOutline = cellEntance.gameObject.AddComponent<QuickOutline.Outline>();
    }

    /// <summary>
    /// Destroys outline 
    /// </summary>
    public void CleanUpCellEntrance()
    {
        Destroy(objectToHighlightOutline);
    }

    /// <summary>
    /// Checks state of cell entrance
    /// </summary>
    protected override void onUpdate()
    {
        CheckCellEntranceCondition();
    }

    private void CheckCellEntranceCondition()
    {
        if (targetStates.Exists(x => x == CellStateData.cellEntranceState))
        {
            Enabled = true;
        }
        else
        {
            Enabled = false;
        }
    }

    protected override void cleanUp()
    {
        CleanUpCellEntrance();
    }
}
