using UnityEngine;

/// <summary>
/// Allows binding keyboard buttons
/// </summary>
public class KeybindsInput : MonoBehaviour
{
    /// <summary>
    /// Reference to targets controller
    /// </summary>
    [SerializeField]
    public KeybindsTargetsController keybindsTargetsController = default;
    /// <summary>
    /// Reference to input controller on scene
    /// </summary>
    private InputController inputController;

    /// <summary>
    /// Initialize base controllers for all steps
    /// </summary>
    private void Awake()
    {
        inputController = ControllersManager.Instance.GetController<InputController>();
    }
}
