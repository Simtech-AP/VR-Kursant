using System;
using UnityEngine;
using UnityEngine.Events;

public enum TeleportType
{
    INSIDE = 1,
    OUTSIDE = 2
}

public enum OnTeleportCanvasBehaviour
{
    CHANGABLE = 0,
    INSIDE = 1,
    OUTSIDE = 2
}

public class TeleportCockpitButton : CockpitButton
{
    [SerializeField]
    private TeleportType teleportType = default;

    private TeleportController teleportController = default;

    private static OnTeleportCanvasBehaviour defaultModifyCanvasBehaviour = OnTeleportCanvasBehaviour.CHANGABLE;
    private OnTeleportCanvasBehaviour modifyCanvasBehaviour = OnTeleportCanvasBehaviour.CHANGABLE;

    public UnityEvent OnInnerTeleportActivation = default;
    public UnityEvent OnOuterTeleportActivation = default;

    [SerializeField]
    private Material activeMaterialIn = default;

    [SerializeField]
    private Material activeMaterialOut = default;

    private void Awake()
    {
        teleportController = ControllersManager.Instance.GetController<TeleportController>();
        if (CellStateData.cellEntranceState == CellEntranceState.Closed)
        {
            DisableButton();
        }
    }

    private void OnEnable()
    {
        teleportController.OnTeleportInvoked += ModifyCanvasPosition;
    }

    protected override void OnPress()
    {
        InvokeTeleport();
    }

    public void InvokeTeleport()
    {
        Action actionToInvoke = delegate { };

        if (teleportType == TeleportType.INSIDE)
        {
            SetType((int)TeleportType.OUTSIDE);
            actionToInvoke = OnInnerTeleportActivation.Invoke;
        }
        else
        {
            SetType((int)TeleportType.INSIDE);
            actionToInvoke = OnOuterTeleportActivation.Invoke;
        }

        teleportController.TeleportDestination(teleportType);
        ModifyCanvasPosition();
        actionToInvoke();
    }

    public void SetType(int targetType)
    {
        teleportType = (TeleportType)targetType;

        if (allowInteraction)
        {
            var newMaterial = (TeleportType)targetType == TeleportType.INSIDE ? activeMaterialIn : activeMaterialOut;
            SetMaterial(newMaterial, true);
        }
    }

    private void ModifyCanvasPosition()
    {
        var newCanvasPosition = (int)modifyCanvasBehaviour;

        if (modifyCanvasBehaviour == 0)
        {
            newCanvasPosition = (int)teleportType;
        }

        FindObjectOfType<ContinueFrameSwitcher>().SetCanvasPosition(newCanvasPosition);
    }

    public void SetCanvasBehaviour(int action)
    {
        modifyCanvasBehaviour = (OnTeleportCanvasBehaviour)action;

        ModifyCanvasPosition();
    }

    public void ResetModifyCanvasActions()
    {
        modifyCanvasBehaviour = TeleportCockpitButton.defaultModifyCanvasBehaviour;
    }

    private void OnDisable()
    {
        teleportController.OnTeleportInvoked -= ModifyCanvasPosition;
    }

}