using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Enumeration for state of EStop buttons
/// </summary>
public enum EStopState
{
    PRESSED,
    RELEASED
}

/// <summary>
/// Allows toggling the EStops on scene
/// </summary>
public class ToggleEStop : StepEnabler
{
    /// <summary>
    /// List of EStops on scene
    /// </summary>
    private EStop[] eStops;
    /// <summary>
    /// List of EStops to toggle next time
    /// </summary>
    private List<EStop> togleEStops = new List<EStop>();
    /// <summary>
    /// List of outlines for EStops
    /// </summary>
    private List<QuickOutline.Outline> objectToHighlightOutlineList = new List<QuickOutline.Outline>();
    /// <summary>
    /// Are we preparing EStops for toggling?
    /// </summary>
    private bool prepareEstop = false;
    /// <summary>
    /// State of EStop to check
    /// </summary>
    [Header("Toggle Estop")]
    [SerializeField]
    private EStopState checkState = default;

    /// <summary>
    /// Sets up references
    /// </summary>
    protected override void initialize()
    {
        eStops = InteractablesManager.Instance.GetAllInteractableBehaviour<EStop>();
        PrepareEStop();
    }

    /// <summary>
    /// Prepares EStops for next step
    /// </summary>
    public void PrepareEStop()
    {
        CleanEStops();
        Enabled = false;
        foreach (var eStop in eStops)
        {
            switch (checkState)
            {
                case EStopState.RELEASED:
                    if (CellStateData.EStopStates[eStop.EStopIndex] == EStopButtonState.Pressed)
                    {
                        AddEStop(eStop);
                        eStop.OnReleased.AddListener(CheckEndStep);
                    }
                    break;
                case EStopState.PRESSED:
                    if (CellStateData.EStopStates[eStop.EStopIndex] == EStopButtonState.Released)
                    {
                        AddEStop(eStop);
                        eStop.OnPressed.AddListener(CheckEndStep);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Adds EStop for toggling
    /// </summary>
    /// <param name="eStop"></param>
    private void AddEStop(EStop eStop)
    {
        objectToHighlightOutlineList.Add(eStop.gameObject.AddComponent<QuickOutline.Outline>());
        togleEStops.Add(eStop);
    }

    /// <summary>
    /// Cleans up listeners and outlines
    /// </summary>
    public void CleanEStops()
    {
        foreach (var eStop in togleEStops)
        {
            switch (checkState)
            {
                case EStopState.PRESSED:
                    eStop.OnPressed.RemoveListener(CheckEndStep);
                    break;
                case EStopState.RELEASED:
                    eStop.OnReleased.RemoveListener(CheckEndStep);
                    break;
            }
        }
        foreach (var objectToHighlightOutline in objectToHighlightOutlineList)
        {
            Destroy(objectToHighlightOutline);
        }

        togleEStops.Clear();
        objectToHighlightOutlineList.Clear();
    }

    /// <summary>
    /// Checks if EStops are in desired state
    /// </summary>
    private void CheckEndStep()
    {
        switch (checkState)
        {
            case EStopState.PRESSED:
                if (CellStateData.EStopStates.Any(x => x == EStopButtonState.Pressed))
                {
                    CleanEStops();
                    Enabled = true;
                    prepareEstop = false;
                }
                break;
            case EStopState.RELEASED:
                if (CellStateData.EStopStates.All(x => x == EStopButtonState.Released))
                {
                    CleanEStops();
                    Enabled = true;
                    prepareEstop = false;
                }
                break;
        }
    }

    /// <summary>
    /// Checks state of EStops continously
    /// </summary>
    protected override void onUpdate()
    {
        CheckEstopCondition();
    }

    public void CheckEstopCondition()
    {
        switch (checkState)
        {
            case EStopState.PRESSED:
                if (CellStateData.EStopStates.All(x => x == EStopButtonState.Released) && !prepareEstop)
                {
                    PrepareEStop();
                    prepareEstop = true;
                }
                break;
            case EStopState.RELEASED:
                if (CellStateData.EStopStates.Any(x => x == EStopButtonState.Pressed) && !prepareEstop)
                {
                    PrepareEStop();
                    prepareEstop = true;
                }
                break;
        }
        CheckEndStep();
    }

    protected override void cleanUp()
    {
        CleanEStops();
    }
}
