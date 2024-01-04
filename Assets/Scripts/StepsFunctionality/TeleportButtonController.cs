using System;
using UnityEngine;

/// <summary>
/// Controls state and position of teleport button
/// </summary>
public class TeleportButtonController : Controller
{
    /// <summary>
    /// Teleport button reference 
    /// </summary>
    [SerializeField]
    private TeleportCockpitButton teleportButton = default;


    /// <summary>
    /// Sets up events to listeners
    /// </summary>
    private void OnEnable()
    {
        CellEntrance.CellClosed += ToggleOffTeleport;
        CellEntrance.CellOpened += ToggleOnTeleport;
    }

    /// <summary>
    /// Removes events from listeners
    /// </summary>
    private void OnDisable()
    {
        CellEntrance.CellClosed -= ToggleOffTeleport;
        CellEntrance.CellOpened -= ToggleOnTeleport;
    }

    /// <summary>
    /// Hides al teleports
    /// </summary>
    public void ToggleOffTeleport()
    {
        teleportButton.DisableButton();
    }

    /// <summary>
    /// Toggles on teleport buttons according to their state
    /// </summary>
    public void ToggleOnTeleport()
    {
        teleportButton.EnableButton();
    }

    /// <summary>
    /// Allows teleporting using pendant button
    /// </summary>
    public void PendantTeleport()
    {
        teleportButton.InvokeTeleport();
    }

    public void OverwriteCanvasBehaviour(int action)
    {
        teleportButton.SetCanvasBehaviour(action);
    }

    public void ResetModifyCanvasActions()
    {
        teleportButton.ResetModifyCanvasActions();
    }
}
