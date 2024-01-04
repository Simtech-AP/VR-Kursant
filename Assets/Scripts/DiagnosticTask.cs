using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Task containing logic of robot diagnostics
/// </summary>
public class DiagnosticTask : ExamTask
{
    /// <summary>
    /// List of all buttons needed for task
    /// </summary>
    private List<GameObject> eStopButtons = new List<GameObject>();
    /// <summary>
    /// List of all security related buttons needed for task
    /// </summary>
    private List<GameObject> securityResetButtons = new List<GameObject>();
    /// <summary>
    /// List of all alarm related buttons needed for task
    /// </summary>
    private List<GameObject> alarmResetButtons = new List<GameObject>();

    private bool wasMovementModeScored = false;
    private bool deadmanLeftPressed = false;
    private bool deadmanRightPressed = false;

    private TeleportCockpitButton teleportButton;

    /// <summary>
    /// Sets up references to buttons and adds listeners to them
    /// </summary>
    public async void Initialize(int withDelay = 0)
    {
        var gc = ControllersManager.Instance.GetController<GazeController>();
        gc.DisableGazerGuiCircle();

        await System.Threading.Tasks.Task.Delay(withDelay * 1000);
        setupEStopButtons();
        setupSecurityResetButtons();
        setupAlarmResetButtons();
        setupTeleportButton();

    }

    private void setupTeleportButton()
    {
        teleportButton = InteractablesManager.Instance.GetInteractableButton(ButtonType.TELEPORT).GetComponent<TeleportCockpitButton>();

        teleportButton.OnOuterTeleportActivation.AddListener(checkCellEnteredCondition);

    }

    private void setupAlarmResetButtons()
    {
        alarmResetButtons = InteractablesManager.Instance.GetAllInteractableButton(ButtonType.ALARM_RESET);

        foreach (var button in alarmResetButtons)
        {
            button.GetComponent<PhysicalButton>().OnPressed.AddListener(checkResetAlarmsCondition);
        }
    }

    private void setupSecurityResetButtons()
    {
        securityResetButtons = InteractablesManager.Instance.GetAllInteractableButton(ButtonType.SECUIRTY_RESET);

        foreach (var button in securityResetButtons)
        {
            button.GetComponent<PhysicalButton>().OnPressed.AddListener(checkSecurityResetCondition);
        }
    }

    private void setupEStopButtons()
    {
        eStopButtons = InteractablesManager.Instance.GetAllInteractableButton(ButtonType.ESTOP);

        foreach (var button in eStopButtons)
        {
            button.GetComponent<PhysicalButton>().OnReleased.AddListener(checkEStopCondition);
        }
    }

    private void clearAlarmResetButtons()
    {
        foreach (var button in alarmResetButtons)
        {
            button.GetComponent<PhysicalButton>().OnPressed.RemoveListener(checkResetAlarmsCondition);
        }
    }

    private void clearSecurityResetButtons()
    {
        foreach (var button in securityResetButtons)
        {
            button.GetComponent<PhysicalButton>().OnPressed.RemoveListener(checkSecurityResetCondition);
        }
    }

    private void clearEStopButtons()
    {
        foreach (var button in eStopButtons)
        {
            button.GetComponent<PhysicalButton>().OnReleased.RemoveListener(checkEStopCondition);
        }
    }

    private void clearTeleportButtons()
    {
        teleportButton.OnOuterTeleportActivation.RemoveListener(checkCellEnteredCondition);
    }

    /// <summary>
    /// Adds points for resetting alarms
    /// </summary>
    private void checkResetAlarmsCondition()
    {
        if (ErrorRequester.HasResetAlarmErrors())
        {
            testModule.GetPointByName("Zad.4: Skasowanie pozostałych błędów na pulpicie: ").SetPoints(15);
        }
    }

    /// <summary>
    /// Adds points for resetting security errors
    /// </summary>
    private void checkSecurityResetCondition()
    {
        /// !?
        if (ErrorRequester.HasResetSecurityErrors())
        {
            testModule.GetPointByName("Zad.4: Skasowanie błędów bezpieczeństwa na pulpicie: ").SetPoints(15);
        }
    }

    /// <summary>
    /// Adds points for releasing EStop switch
    /// </summary>
    private void checkEStopCondition()
    {
        if (CellStateData.EStopStates.All(x => x == 0))
        {
            testModule.GetPointByName("Zad.4: Wyciągnięcie przycisku E-STOP na szafie robota: ").SetPoints(30);
        }
    }


    private void checkMovementModeCondition()
    {
        if (deadmanLeftPressed || deadmanRightPressed)
        {
            if (RobotData.Instance.MovementMode == MovementMode.T1)
            {
                testModule.GetPointByName("Zad.4: Przełączenie robota w T1: ").SetPoints(10);
                wasMovementModeScored = true;
            }
            else if (RobotData.Instance.MovementMode == MovementMode.T2)
            {
                testModule.GetPointByName("Zad.4: Przełączenie robota w T2: ").SetPoints(5);
                wasMovementModeScored = true;
            }
        }
    }

    public void checkGazableTargetCondition(string objectName)
    {
        if (!ErrorRequester.HasResetAlarmErrors())
        {
            if (objectName == "ScreenTarget")
            {
                testModule.GetPointByName("Zad.4: Zwrócenie uwagi na listę alarmową, gdy alarm aktywny: ").SetPoints(10);
            }
            else if (objectName == "LampTarget")
            {
                testModule.GetPointByName("Zad.4: Zwrócenie uwagi na kolumnę świetlną, gdy alarm aktywny: ").SetPoints(10);

            }
        }
    }

    public void OnDeadmanPressed(string name)
    {
        if (name == "DeadmanR")
        {
            deadmanRightPressed = true;
        }
        else if (name == "DeadmanL")
        {
            deadmanLeftPressed = true;
        }
    }

    public void OnDeadmanReleased(string name)
    {
        if (name == "DeadmanR")
        {
            deadmanRightPressed = false;
        }
        else if (name == "DeadmanL")
        {
            deadmanLeftPressed = false;
        }
    }

    public void checkConditionsOnMoveAttempt()
    {
        if (!wasMovementModeScored)
        {
            checkMovementModeCondition();
        }

        if (ErrorRequester.HasResetAlarmErrors() && ErrorRequester.HasAllErrorsReset())
        {
            testModule.GetPointByName("Zad.4: Zazbrojenie robota i poruszanie w dowolnym układzie po usunięciu błędu: ").SetPoints(20);
        }
        else
        {
            testModule.GetPointByName("Zad.4: Zazbrojenie robota i próba poruszania bez wcześniejszego usunięcia błędu: ").SetPoints(-30);
        }


    }

    private void checkCellEnteredCondition()
    {
        if (CellStateData.bumpCapState != BumperCapState.OnHead)
        {
            testModule.GetPointByName("Zad.4: Wejście do celi bez czapki: ").SetPoints(-50);
        }

        if (CellStateData.padLockState != PadLockState.OnDoor)
        {
            testModule.GetPointByName("Zad.4: Wejście do celi bez kłódki: ").SetPoints(-50);
        }
    }

    /// <summary>
    /// Finishes task
    /// Removes listeners from events
    /// </summary>
    public override void EndTask()
    {
        clearEStopButtons();
        clearSecurityResetButtons();
        clearAlarmResetButtons();
        clearTeleportButtons();

        var gc = ControllersManager.Instance.GetController<GazeController>();
        gc.EnableGazerGuiCircle();
    }

}
