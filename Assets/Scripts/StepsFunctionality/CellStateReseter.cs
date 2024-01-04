using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains state of cell entrance for reset
/// </summary>
[System.Serializable]
public class CellEntranceReset
{
    public bool Opened;
    public bool Closed;
    public int State;
}

/// <summary>
/// Contains state of bumper cap for reset
/// </summary>
[System.Serializable]
public class BumperCapReset
{
    public bool OnHead;
    public bool OnDoor;
    public int State;
}

/// <summary>
/// Contains pad lock state for reset
/// </summary>
[System.Serializable]
public class PadLockReset
{
    public bool OnDoor;
    public bool InLockBox;
    public int State;
}

/// <summary>
/// Contains state of robot for reset
/// </summary>
[System.Serializable]
public class RobotReset
{
    public bool Stopped;
    public bool HomePosition;
}

/// <summary>
/// Contains movement type for reset
/// </summary>
[System.Serializable]
public class RobotMovementType
{
    public MovementType MovementType;
    public bool Switch = false;
}

[System.Serializable]
public class EStopsState
{
    public bool OnCasette = false;
    public bool OnLocker = false;
}

public enum ExecutionModeReset
{
    NO_CHANGE,
    NORMAL,
    STEP
}

/// <summary>
/// Allows resetting state of cell
/// </summary>
[AddComponentMenu("Cell Reseter")]
public class CellStateReseter : MonoBehaviour
{
    /// <summary>
    /// Errors to raise when resetting
    /// </summary>
    public List<string> ErrorsToRaise;
    /// <summary>
    /// Movement mode to set after reest
    /// </summary>
    public MovementMode MovementMode;
    /// <summary>
    /// Reference to set robot movement mode after reset
    /// </summary>
    public RobotMovementType RobotMovementType;
    /// <summary>
    /// Reference to set cell state after reset
    /// </summary>
    [HideInInspector]
    public CellEntranceReset cellEntranceReset;
    /// <summary>
    /// Reference to set bumper cap state after reset
    /// </summary>
    [HideInInspector]
    public BumperCapReset bumperCapReset;
    /// <summary>
    /// Reference to set pad lock state after reset
    /// </summary>
    [HideInInspector]
    public PadLockReset padLockReset;
    [HideInInspector]
    public EStopsState eStopsState;
    /// <summary>
    /// Reference to set robot state after reset
    /// </summary>
    [HideInInspector]
    public RobotReset robotReset;
    public bool resetEndPoint = false;
    public DirectionType endPointOrientation = DirectionType.FORWARD;

    public ExecutionModeReset ExecutionModeReset = ExecutionModeReset.NO_CHANGE;

    /// <summary>
    /// Reference to mode key on scene
    /// </summary>
    private ModeKey modeKey;
    /// <summary>
    /// Reference to cell entrance on scene
    /// </summary>
    private CellEntrance cellEntrance;
    /// <summary>
    /// Reference to bumper cap on scene
    /// </summary>
    private BumperCap bumperCap;
    /// <summary>
    /// Reference to pad lock on scene
    /// </summary>
    private PadLock padLock;
    /// <summary>
    /// Reference to program controller
    /// </summary>
    private ProgramController programController;
    /// <summary>
    /// Reference to robot controller
    /// </summary>
    private RobotController robotController;

    private List<EStop> eStopButtons = default;

    /// <summary>
    /// Sets up references
    /// </summary>
    private void OnEnable()
    {
        modeKey = InteractablesManager.Instance.GetInteractableBehaviour<ModeKey>();
        bumperCap = InteractablesManager.Instance.GetInteractableBehaviour<BumperCap>();
        cellEntrance = InteractablesManager.Instance.GetInteractableBehaviour<CellEntrance>();
        padLock = InteractablesManager.Instance.GetInteractableBehaviour<PadLock>();
        eStopButtons = InteractablesManager.Instance.GetAllInteractableBehaviour<EStop>().ToList();
        programController = ControllersManager.Instance.GetController<ProgramController>();
        robotController = ControllersManager.Instance.GetController<RobotController>();
    }

    /// <summary>
    /// Resets cell to specified state
    /// </summary>
    public void CellReset()
    {
        CellStateData cellStateData = FindObjectOfType<CellStateData>();
        cellStateData.ResetCellState();

        bumperCap.SetState(bumperCapReset.State);
        cellEntrance.SetState(cellEntranceReset.State);
        padLock.SetState(padLockReset.State);
        modeKey.SetMode(MovementMode);

        if (ExecutionModeReset != ExecutionModeReset.NO_CHANGE)
        {
            var targetMode = ExecutionModeReset == ExecutionModeReset.STEP ? StepMode.STEP : StepMode.NORMAL;
            robotController.SwitchStepMode(targetMode);
        }

        if (robotReset.Stopped)
        {
            robotController.SoftStop();
        }

        if (robotReset.HomePosition)
        {
            robotController.CurrentRobot.MoveToHomePosition(true);
        }
        else if (!robotReset.Stopped)
        {
            programController.RunProgram();
        }

        for (int i = 0; i < ErrorsToRaise.Count; ++i)
        {
            ErrorRequester.RaiseError(ErrorsToRaise[i]);
        }

        if (RobotMovementType.Switch)
        {
            RobotData.Instance.MovementType = RobotMovementType.MovementType;
            // robotController.CurrentRobot.SwitchMovementType(RobotMovementType.MovementType);     //moved to "MoveToHomePosition()" to be called after MoveToHomePosition coroutine
        }

        if (eStopsState.OnLocker)
        {
            eStopButtons.Find(x => x.name == "szafa_robota_EStop").InvokeEStopPressed();
        }
        else
        {
            eStopButtons.Find(x => x.name == "szafa_robota_EStop").InvokeEStopRelesed();
        }

        if (eStopsState.OnCasette)
        {
            eStopButtons.Find(x => x.name == "kaseta_sterownicza_przycisk4").InvokeEStopPressed();
        }
        else
        {
            eStopButtons.Find(x => x.name == "kaseta_sterownicza_przycisk4").InvokeEStopRelesed();
        }

        if (resetEndPoint)
        {
            robotController.CurrentRobot.SetEndPointOrientation(endPointOrientation);
        }

    }
}
