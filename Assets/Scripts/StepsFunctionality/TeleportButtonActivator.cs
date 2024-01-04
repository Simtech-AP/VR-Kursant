using UnityEngine;

/// <summary>
/// Allows activation of teleport buttons
/// </summary>
public class TeleportButtonActivator : MonoBehaviour
{
    /// <summary>
    /// Reference to teleport button controller
    /// </summary>
    private TeleportButtonController teleportButtonController;

    /// <summary>
    /// Sets up references
    /// </summary>
    private void Awake()
    {
        teleportButtonController = ControllersManager.Instance.GetController<TeleportButtonController>();
    }

    public void OverwriteCanvasBehaviour(int action)
    {
        teleportButtonController.OverwriteCanvasBehaviour(action);
    }

    public void ResetCanvasBahaviour()
    {
        teleportButtonController.ResetModifyCanvasActions();
    }
}
