using UnityEngine;
using UnityEngine.Events;

public class NextStepCockpitButton : CockpitButton
{
    private ContinueFrameController continueFrameController = default;

    [SerializeField]
    private InteractGlove interactGlove = default;

    [SerializeField]
    private UnityEvent onNextStep = default;

    private void Awake()
    {
        continueFrameController = ControllersManager.Instance.GetController<ContinueFrameController>();
    }

    protected override void OnPress()
    {
        continueFrameController.GoToNextStep();
        interactGlove.ReleaseObject();
        onNextStep?.Invoke();
    }
}