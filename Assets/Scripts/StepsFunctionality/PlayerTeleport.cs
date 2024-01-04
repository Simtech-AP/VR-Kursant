using UnityEngine;

/// <summary>
/// Teeports player to specified location
/// </summary>
public class PlayerTeleport : MonoBehaviour
{
    /// <summary>
    /// Reference to teleport controller
    /// </summary>
    private TeleportController teleportController;
    private UserCockpitController userCockpitController;

    /// <summary>
    /// Sets up references
    /// </summary>
    private void OnEnable()
    {
        teleportController = ControllersManager.Instance.GetController<TeleportController>();
        userCockpitController = ControllersManager.Instance.GetController<UserCockpitController>();
    }

    /// <summary>
    /// Teleports player inside of cell
    /// </summary>
    public void TeleportInsideCell()
    {
        teleportController.TeleportDestination(TeleportType.INSIDE);
        userCockpitController.Reposition((int)CockpitPosition.INSIDE);
        userCockpitController.SetTeleportType((int)CockpitPosition.INSIDE);
    }

    /// <summary>
    /// Teleports player outside of cell
    /// </summary>
    public void TeleportOutsideCell()
    {
        teleportController.TeleportDestination(TeleportType.OUTSIDE);
        userCockpitController.Reposition((int)CockpitPosition.OUTSIDE);
        userCockpitController.SetTeleportType((int)CockpitPosition.OUTSIDE);
    }
}
