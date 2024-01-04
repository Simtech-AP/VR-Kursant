using UnityEngine;

/// <summary>
/// Allows chcking for key presses on pendant
/// </summary>
public class KeyDownListener : MonoBehaviour
{
    /// <summary>
    /// Reference to current step
    /// </summary>
    private Step currentStep;
    /// <summary>
    /// Reference to input controller
    /// </summary>
    private InputController inputController;
    /// <summary>
    /// Reference to continue enebler
    /// </summary>
    [SerializeField]
    private ContinueListener continueListener = default;

    /// <summary>
    /// Sets up references
    /// </summary>
    private void Awake()
    {
        currentStep = GetComponentInParent<Step>();
        inputController = ControllersManager.Instance.GetController<InputController>();
    }

    /// <summary>
    /// If any key was pressed allow changing step
    /// </summary>
    private void Update()
    {
        if (inputController.isAnyKeyDown)
        {
            continueListener.EnableContinuation();
        }
    }
}
