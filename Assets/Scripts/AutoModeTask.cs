using System;
using System.Collections.Generic;
using UnityEngine;

public class AutoModeTask : ExamTask
{
    [SerializeField]
    private ContinueListener continueListener = default;

    private TeleportCockpitButton teleportButton;
    private List<GameObject> startButtons;
    private List<GameObject> resetButtons;

    private bool wasCellEntranceRepelled = false;
    private bool wasStartAttempted = false;
    private bool wasResetAttempted = false;

    private Action onUpdate = delegate { };

    public async void Initialize(int withDelay = 0)
    {
        await System.Threading.Tasks.Task.Delay(withDelay * 1000);
        setupTeleportButton();
        setupStartButtons();
        setupResetButtons();

        onUpdate += checkPadlockBeforeGateCondition;
        onUpdate += checkCellEntranceClosedCondition;
        onUpdate += checkModeCondition;
        onUpdate += disableContinuation;

    }

    private void setupResetButtons()
    {
        resetButtons = InteractablesManager.Instance.GetAllInteractableButton(ButtonType.SECUIRTY_RESET);
        resetButtons.AddRange(InteractablesManager.Instance.GetAllInteractableButton(ButtonType.ALARM_RESET));

        foreach (var button in resetButtons)
        {
            button.GetComponent<PhysicalButton>().OnPressed.AddListener(checkErrorResetCondition);
        }
    }

    private void clearResetButtons()
    {
        foreach (var button in resetButtons)
        {
            button.GetComponent<PhysicalButton>().OnPressed.RemoveListener(checkErrorResetCondition);
        }
    }

    private void setupStartButtons()
    {
        startButtons = InteractablesManager.Instance.GetAllInteractableButton(ButtonType.START);

        foreach (var button in startButtons)
        {
            button.GetComponent<PhysicalButton>().OnPressed.AddListener(checkStartButtonCondition);
        }
    }

    private void clearStartButtons()
    {
        foreach (var button in startButtons)
        {
            button.GetComponent<PhysicalButton>().OnPressed.RemoveListener(checkStartButtonCondition);
        }
    }

    private void setupTeleportButton()
    {
        teleportButton = InteractablesManager.Instance.GetInteractableButton(ButtonType.TELEPORT).GetComponent<TeleportCockpitButton>();

        teleportButton.OnInnerTeleportActivation.AddListener(checkCellExitCondition);
        teleportButton.OnOuterTeleportActivation.AddListener(checkCellEnteredCondition);
    }

    private void clearTeleportButtons()
    {
        teleportButton.OnInnerTeleportActivation.RemoveListener(checkCellExitCondition);
        teleportButton.OnOuterTeleportActivation.RemoveListener(checkCellEnteredCondition);
    }

    private void checkCellExitCondition()
    {
        testModule.GetPointByName("Zad.2: Wyjście z celi: ").SetPoints(5);

        onUpdate -= disableContinuation;
    }

    private void checkCellEnteredCondition()
    {
        if (CellStateData.padLockState != PadLockState.OnDoor)
        {
            testModule.GetPointByName("Zad.2: Wejście do celi, gdy kłódka jest zdjęta: ").SetPoints(-50);
        }
    }

    private void checkPadlockBeforeGateCondition()
    {
        if (CellStateData.cellEntranceState == CellEntranceState.Repealed)
        {
            testModule.GetPointByName("Zad.2: Próba zamknięcia bramki przed zdjęciem kłódki: ").SetPoints(-15);
            wasCellEntranceRepelled = true;
        }

        if (CellStateData.cellEntranceState == CellEntranceState.Closed && !wasCellEntranceRepelled)
        {
            testModule.GetPointByName("Zad.2: Zdjęcie kłódki przed próbą zamknięcia bramki: ").SetPoints(15);
        }
    }

    private void checkCellEntranceClosedCondition()
    {
        if (CellStateData.cellEntranceState == CellEntranceState.Closed)
        {
            testModule.GetPointByName("Zad.2: Zamknięcie bramki: ").SetPoints(10);
        }
    }

    private void checkModeCondition()
    {
        if (RobotData.Instance.MovementMode == MovementMode.AUTO
            && !wasResetAttempted
            && !wasStartAttempted)
        {
            testModule.GetPointByName("Zad.2: Przełączenie robota w AUTO przed próbą skasowania błędów i naciśnięciem START: ").SetPoints(20);
        }
    }

    private void checkErrorResetCondition()
    {
        wasResetAttempted = true;

        if (CellStateData.cellEntranceState == CellEntranceState.Closed
            && RobotData.Instance.MovementMode == MovementMode.AUTO
            && ErrorRequester.HasAllErrorsReset()
            && ErrorRequester.HasResetAlarmErrors())
        {
            testModule.GetPointByName("Zad.2: Skasowanie błędów, gdy bramka jest zamknięta i robot jest w AUTO: ").SetPoints(20);
        }
    }

    private void checkStartButtonCondition()
    {
        wasStartAttempted = true;

        if (CellStateData.cellEntranceState != CellEntranceState.Closed)
        {
            testModule.GetPointByName("Zad.2: Naciśnięcie START podczas, gdy bramka jest otwarta: ").SetPoints(-30);
        }

        if (RobotData.Instance.MovementMode != MovementMode.AUTO)
        {
            testModule.GetPointByName("Zad.2: Naciśnięcie START podczas, gdy robot nie jest w AUTO: ").SetPoints(-25);
        }

        if (CellStateData.cellEntranceState == CellEntranceState.Closed
            && RobotData.Instance.MovementMode == MovementMode.AUTO
            && ErrorRequester.HasAllErrorsReset()
            && ErrorRequester.HasResetAlarmErrors())
        {
            testModule.GetPointByName("Zad.2: Uruchomienie produkcji przyciskiem START po spełnieniu warunków: ").SetPoints(30);
        }
    }

    private void disableContinuation()
    {
        continueListener.DisableContinuation();
    }

    private void Update()
    {
        onUpdate();
    }

    public override void EndTask()
    {
        clearTeleportButtons();
        clearStartButtons();
        clearResetButtons();

        onUpdate -= checkPadlockBeforeGateCondition;
        onUpdate -= checkCellEntranceClosedCondition;
        onUpdate -= checkModeCondition;
        onUpdate -= disableContinuation;
    }
}