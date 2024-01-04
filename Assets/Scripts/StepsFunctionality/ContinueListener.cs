using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Enables and disables continuing to next step
/// </summary>
public class ContinueListener : MonoBehaviour
{
    /// <summary>
    /// Reference to confinue frame
    /// </summary>
    private ContinueFrameController continueFrame;
    /// <summary>
    /// Reference to next step button
    /// </summary>
    private NextStepButtonController nextStepButton;

    private bool isEnabled = true;

    [SerializeField] private UnityEvent onContinuationEnabled;
    [SerializeField] private UnityEvent onContinuationDisabled;


    /// <summary>
    /// Sets up references
    /// </summary>
    private void Awake()
    {
        continueFrame = ControllersManager.Instance.GetController<ContinueFrameController>();
        nextStepButton = ControllersManager.Instance.GetController<NextStepButtonController>();

        DisableContinuation();
    }

    /// <summary>
    /// Disables continuing to next step
    /// </summary>
    public void DisableContinuation()
    {
        if (isEnabled)
        {
            continueFrame.DisableNextStep();
            nextStepButton.DisableButton();
            onContinuationDisabled.Invoke();
            isEnabled = false;
        }
    }

    /// <summary>
    /// Enables continuing to next step
    /// </summary>
    public void EnableContinuation()
    {
        if (!isEnabled)
        {
            continueFrame.EnableNextStep();
            nextStepButton.EnableButton();
            onContinuationEnabled.Invoke();
            isEnabled = true;
        }
    }
}
