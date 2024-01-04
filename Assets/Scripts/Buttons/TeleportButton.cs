using UnityEngine;

/// <summary>
/// Class allowing player to teleport using a button on scene
/// </summary>
public class TeleportButton : PhysicalButton
{
    /// <summary>
    /// Return button
    /// </summary>
    public GameObject backButton;
    /// <summary>
    /// Is the teleportation possiblity blocked?
    /// </summary>
    public bool blockTeleport = false;
    /// <summary>
    /// Is the teleportation button active?
    /// </summary>
    public bool isActive = false;
    /// <summary>
    /// Current teleportation 
    /// </summary>
    public TeleportType teleportType;
    /// <summary>
    /// Reference to 
    /// </summary>
    [SerializeField]
    private TeleportController teleportController = default;

    private static int defaultInnerModifyCanvasAction = 2;
    private static int defaultOuterModifyCanvasAction = 1;

    private int modifyCanvasAction;

    private void Awake()
    {
        modifyCanvasAction = teleportType == TeleportType.INSIDE ? defaultInnerModifyCanvasAction : defaultOuterModifyCanvasAction;
    }


    /// <summary>
    /// Called then user want to teleport
    /// </summary>
    public void Teleport()
    {
        InvokeTeleport();
    }

    /// <summary>
    /// Presses the button
    /// </summary>
    [ContextMenu("TEST")]
    public void Press()
    {
        OnPressed?.Invoke();
        if (!blockTeleport)
            Teleport();
    }

    /// <summary>
    /// Main teleport couroutine
    /// </summary>
    /// <returns>Handle to coroutine</returns>
    private void InvokeTeleport()
    {
        teleportController.TeleportDestination(teleportType);
        backButton.SetActive(true);
        backButton.GetComponent<TeleportButton>().isActive = true;
        gameObject.SetActive(false);
        isActive = false;
        ModifyCanvasPosition();
    }

    /// <summary>
    /// Set canvas position based on teleport button position
    /// </summary>
    private void ModifyCanvasPosition()
    {
        FindObjectOfType<ContinueFrameSwitcher>().SetCanvasPosition(modifyCanvasAction);
    }

    public void OverwriteModifyCanvasActions(int action)
    {
        modifyCanvasAction = action;
    }

    public void ResetModifyCanvasActions()
    {
        if (teleportType == TeleportType.INSIDE)
        {
            modifyCanvasAction = TeleportButton.defaultInnerModifyCanvasAction;
        }
        else
        {
            modifyCanvasAction = TeleportButton.defaultOuterModifyCanvasAction;
        }
    }

    private void Update()
    {

    }
}
