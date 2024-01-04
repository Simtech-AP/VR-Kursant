using UnityEngine;
using Valve.VR.InteractionSystem;

/// <summary>
/// Allows resetting scenarios to specified state
/// </summary>
public class ScenarioReseter : MonoBehaviour
{
    /// <summary>
    /// Reference to cell state resetter
    /// </summary>
    private CellStateReseter cellStateReseter;
    /// <summary>
    /// Should we reset cell?
    /// </summary>
    public bool ResetCell = false;

    /// <summary>
    /// Sets up references
    /// </summary>
    private void Awake()
    {
        cellStateReseter = GetComponent<CellStateReseter>();
    }

    /// <summary>
    /// Resets cell state
    /// </summary>
    public void ResetCellState()
    {
        CleanUpStepState();
        RemoveOutlines();
        RemoveGazables();

        if (ResetCell)
            cellStateReseter.CellReset();
    }

    /// <summary>
    /// Resets objects used in steps
    /// </summary>
    private void CleanUpStepState()
    {
        PressedButtonTarget currentStep = FindObjectOfType<PressedButtonTarget>();
        if (currentStep)
            currentStep.CleanUpButtons();

        GazableTarget gazableTarget = FindObjectOfType<GazableTarget>();
        if (gazableTarget)
            gazableTarget.CleanObjects();

        Hover[] hovers = FindObjectsOfType<Hover>();
        if (hovers != null)
            hovers.ForEach(x => x.gameObject.SetActive(false));

        RobotInteractiveObject pallete = FindObjectOfType<RobotInteractiveObject>();
        if (pallete)
            pallete.gameObject.SetActive(false);

        Conveyor[] conveyors = FindObjectsOfType<Conveyor>();
        if (conveyors != null)
            conveyors.ForEach(x => x.gameObject.SetActive(false));
    }

    /// <summary>
    /// Removes all outlines on scene
    /// </summary>
    private void RemoveOutlines()
    {
        QuickOutline.Outline[] outlines = FindObjectsOfType<QuickOutline.Outline>();
        if (outlines != null)
        {
            for (int i = 0; i < outlines.Length; ++i)
            {
                Destroy(outlines[i]);
            }
        }
    }

    /// <summary>
    /// Removes gazables from scene
    /// </summary>
    private void RemoveGazables()
    {
        Gazable[] gazables = FindObjectsOfType<Gazable>();
        if (gazables != null)
        {
            for (int i = 0; i < gazables.Length; ++i)
            {
                Destroy(gazables[i]);
            }
        }
    }
}
