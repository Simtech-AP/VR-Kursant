using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Task containing logic of cell setup for working with robot
/// </summary>
public class CellTask : ExamTask
{
    [SerializeField] private InputContainer pendantInputContainer;

    /// <summary>
    /// List of all buttons needed for task
    /// </summary>
    private List<GameObject> stopButtons;
    private List<GameObject> eStopButtons;
    private List<GameObject> modeKeys;


    private TeleportCockpitButton teleportButton;
    private Action onUpdate = delegate { };
    private bool wasRobotManuallyStopped = false;

    [SerializeField]
    private ContinueListener continueListener = default;

    /// <summary>
    /// Sets up references to buttons and adds listeners to them
    /// </summary>
    private void OnEnable()
    {
        setupStopButtons();
        setupEStopButtons();
        setupTeleportButton();
        setupModeKeyButton();

        onUpdate += checkGateOpenCondition;
        onUpdate += checkPadlockCondition;
        onUpdate += checkBumperCapCondition;
        onUpdate += disableContinuation;

        continueListener.DisableContinuation();
    }

    private void setupStopButtons()
    {
        stopButtons = InteractablesManager.Instance.GetAllInteractableButton(ButtonType.STOP);
        for (int i = 0; i < stopButtons.Count; ++i)
        {
            stopButtons[i].GetComponent<PhysicalButton>().OnPressed.AddListener(checkStopCondition);
        }
        pendantInputContainer.binds.Find(x => x.inputName == "STOP").OnPress.AddListener(checkStopCondition);
    }

    private void setupEStopButtons()
    {
        eStopButtons = InteractablesManager.Instance.GetAllInteractableButton(ButtonType.ESTOP);
        eStopButtons.Add(InteractablesManager.Instance.GetAllInteractableButton(ButtonType.PENDANT)[0]);
        for (int i = 0; i < eStopButtons.Count; ++i)
        {
            eStopButtons[i].GetComponent<PhysicalButton>().OnPressed.AddListener(checkEStopCondition);
        }
    }

    private void setupTeleportButton()
    {
        teleportButton = InteractablesManager.Instance.GetInteractableButton(ButtonType.TELEPORT).GetComponent<TeleportCockpitButton>();
        teleportButton.OnOuterTeleportActivation.AddListener(checkCellEnteredCondition);
    }

    private void setupModeKeyButton()
    {
        modeKeys = InteractablesManager.Instance.GetAllInteractableButton(ButtonType.MODE_KEY);
        for (int i = 0; i < stopButtons.Count; ++i)
        {
            modeKeys[i].GetComponent<PhysicalButton>().OnPressed.AddListener(onModeKeyStop);
        }
    }

    private void clearStopButtons()
    {
        for (int i = 0; i < stopButtons.Count; ++i)
        {
            stopButtons[i].GetComponent<PhysicalButton>().OnPressed.RemoveListener(checkStopCondition);
        }
        pendantInputContainer.binds.Find(x => x.inputName == "STOP").OnPress.RemoveListener(checkStopCondition);
    }

    private void clearEstopButtons()
    {
        for (int i = 0; i < eStopButtons.Count; ++i)
        {
            eStopButtons[i].GetComponent<PhysicalButton>().OnPressed.RemoveListener(checkEStopCondition);
        }
    }

    private void clearTeleportButton()
    {
        teleportButton.OnOuterTeleportActivation.RemoveListener(checkCellEnteredCondition);
    }

    private void clearModeKeyButton()
    {
        for (int i = 0; i < stopButtons.Count; ++i)
        {
            modeKeys[i].GetComponent<PhysicalButton>().OnPressed.RemoveListener(onModeKeyStop);
        }
    }

    /// <summary>
    /// Finishes task
    /// Removes listeners from events
    /// </summary>
    public override void EndTask()
    {
        clearStopButtons();
        clearEstopButtons();
        clearTeleportButton();
        clearModeKeyButton();

        onUpdate -= checkGateOpenCondition;
        onUpdate -= checkPadlockCondition;
        onUpdate -= checkBumperCapCondition;
        onUpdate -= disableContinuation;
    }

    /// <summary>
    /// Checks if any of points in task has been made
    /// </summary>
    private void Update()
    {
        onUpdate();
    }

    private void checkStopCondition()
    {
        if (!wasRobotManuallyStopped)
        {
            testModule.GetPointByName("Zad.1: Zatrzymanie robota przyciskiem STOP na pulpicie: ").SetPoints(15);
            wasRobotManuallyStopped = true;
            clearStopButtons();
            clearEstopButtons();
        }
    }

    private void checkEStopCondition()
    {
        if (!wasRobotManuallyStopped)
        {
            testModule.GetPointByName("Zad.1: Zatrzymanie robota wyłącznikiem E-STOP: ").SetPoints(5);
            wasRobotManuallyStopped = true;
            clearStopButtons();
            clearEstopButtons();
        }
    }

    private void onModeKeyStop()
    {
        if (!wasRobotManuallyStopped)
        {
            wasRobotManuallyStopped = true;
            clearStopButtons();
            clearEstopButtons();
        }
    }

    private void checkGateOpenCondition()
    {
        if (CellStateData.cellEntranceState != CellEntranceState.Closed)
        {

            if (!wasRobotManuallyStopped)
            {
                testModule.GetPointByName("Zad.1: Otwarcie bramki bez wcześniejszego zatrzymania robota ").SetPoints(-10);
            }
            else
            {
                testModule.GetPointByName("Zad.1: Otwarcie bramki: ").SetPoints(10);
            }

            onUpdate -= checkGateOpenCondition;
        }
    }

    private void checkPadlockCondition()
    {
        if (CellStateData.padLockState == PadLockState.OnDoor)
        {
            testModule.GetPointByName("Zad.1: Założenie kłódki: ").SetPoints(30);
        }
        else
        {
            testModule.GetPointByName("Zad.1: Założenie kłódki: ").SetPoints(0);
        }
    }

    private void checkBumperCapCondition()
    {
        if (CellStateData.bumpCapState == BumperCapState.OnHead)
        {
            testModule.GetPointByName("Zad.1: Założenie czapki: ").SetPoints(30);
        }
        else
        {
            testModule.GetPointByName("Zad.1: Założenie czapki: ").SetPoints(0);
        }
    }

    private void checkCellEnteredCondition()
    {
        if (CellStateData.bumpCapState != BumperCapState.OnHead)
        {
            testModule.GetPointByName("Zad.1: Wejście do celi bez czapki: ").SetPoints(-40);
        }

        if (CellStateData.padLockState != PadLockState.OnDoor)
        {
            testModule.GetPointByName("Zad.1: Wejście do celi bez kłódki: ").SetPoints(-40);
        }

        testModule.GetPointByName("Zad.1: Wejście do celi: ").SetPoints(10);

        onUpdate -= disableContinuation;
        continueListener.EnableContinuation();
    }

    private void disableContinuation()
    {
        continueListener.DisableContinuation();
    }
}