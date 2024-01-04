using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls logic for controlling a robot as a single entity
/// </summary>
public class RobotController : Controller
{
    [SerializeField]
    private List<JointRobot> allRobots = default;
    /// <summary>
    /// Reference to currently controlled robot
    /// </summary>
    [SerializeField]
    private JointRobot currentRobot = default;
    /// <summary>
    /// Reference to program controller
    /// </summary>
    [SerializeField]
    private ProgramController program = default;
    /// <summary>
    /// Reference to robot data object
    /// </summary>
    private RobotData data = default;
    /// <summary>
    /// Should the program be stopped after completing instruction?
    /// </summary>
    public bool ShouldStopProgram { get; set; }
    /// <summary>
    /// Should the program be stopped after completing program?
    /// </summary>
    public bool ShouldStopAfterProgram { get; set; }
    /// <summary>
    /// Is left deadman pushed in?
    /// </summary>
    public bool DeadmanLeftPushed { get; set; }
    /// <summary>
    /// Is right deadman pushed in?
    /// </summary>
    public bool DeadmanRightPushed { get; set; }
    public List<JointRobot> AllRobots { get => allRobots; }
    public JointRobot CurrentRobot { get => currentRobot; }

    public Action OnRobotSpeedChanged = delegate { };

    /// <summary>
    /// Action invoked at the ending of the current program (after last instruction of the program)
    /// </summary>
    public Action OnProgramEnding = delegate { };

    private bool shouldCountinueFromLastInstruction = false;
    private int moveToInstructionIndex = -1;

    /// <summary>
    /// Sets robot data object to variable
    /// Adds callback to clamp method for changed data
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        data = RobotData.Instance;
        RobotData.OnUpdatedData += ClampRobotSpeed;
        OnRobotSpeedChanged += ApplySpeedChangeToCurrentMovement;
    }

    public void Start()
    {
        LinkRobot();
    }

    public void SetRobot(string name)
    {
        if (currentRobot)
            currentRobot.activeIndicator.SetActive(false);
        currentRobot = AllRobots.FindAll(x => x.name.Equals(name))[0];
        LinkRobot();
    }

    /// <summary>
    /// Links robot t objects available on scene
    /// </summary>
    public void LinkRobot()
    {
        data = RobotData.Instance;
        FindObjectOfType<RobotErrorStatus>().LinkRobot(CurrentRobot);
        FindObjectOfType<TCPPendant>().LinkRobot(CurrentRobot);
        FindObjectOfType<RobotErrorController>().LinkRobot(CurrentRobot);
        FindObjectOfType<FollowPivot6>().LinkRobot(CurrentRobot);
        currentRobot.activeIndicator.SetActive(true);
    }

    /// <summary>
    /// Changes robot speed up or down
    /// </summary>
    /// <param name="up">Is the speed changing up?</param>
    public void ChangeRobotSpeed(bool up)
    {
        if (!up)
        {
            if (data.RobotSpeed <= 0f) return;
            data.RobotSpeed -= 0.1f;
            OnRobotSpeedChanged.Invoke();
        }
        else
        {
            if (data.RobotSpeed >= 1f) return;
            data.RobotSpeed += 0.1f;
            OnRobotSpeedChanged.Invoke();
        }
        data.RobotSpeed = Mathf.Clamp(data.RobotSpeed, 0f, 1f);
    }

    /// <summary>
    /// Clamps robot speed according to movement mode
    /// </summary>
    private void ClampRobotSpeed(RobotData data)
    {
        //float newSpeed;
        //if (data.MovementMode == MovementMode.T1)
        //{
        //    newSpeed = Mathf.Clamp(data.RobotSpeed, 0f, 0.3f);
        //    if (newSpeed != data.RobotSpeed)
        //        data.RobotSpeed = newSpeed;
        //}
    }

    /// <summary>
    /// Moves robot end point in specified direction
    /// </summary>
    /// <param name="direction">Integer number for direction (see MovementDirection)</param>
    [EnumAction(typeof(MovementDirection))]
    public void MovePosition(int direction)
    {
        //TODO: robot.IsInError should not be used by Robot, should be in some ErrorController
        if (!CurrentRobot || program.IsRunning() || CurrentRobot.ErrorDetected || RobotData.Instance.MovementMode == MovementMode.AUTO)
        {
            CurrentRobot.StopPositionMovement();
            return;
        }
        if (data.MovementMode == MovementMode.T2 || data.MovementMode == MovementMode.AUTO)
        {
            CurrentRobot.MovePosition(data.MovementType, (MovementDirection)direction, data.MovementType == MovementType.Joint ? data.RobotSpeed * data.MaxRobotJointSpeed : data.RobotSpeed * data.MaxRobotSpeed);
        }
        else
        {
            CurrentRobot.MovePosition(data.MovementType, (MovementDirection)direction, data.MovementType == MovementType.Joint ? data.RobotSpeed * data.MaxRobotJointSpeed * 0.3f : data.RobotSpeed * data.MaxRobotSpeed * 0.3f);
        }
    }

    /// <summary>
    /// Rotates robot end point in specified direction
    /// </summary>
    /// <param name="direction">Integer number for rotation direction (see MovementDirection)</param>
    [EnumAction(typeof(MovementDirection))]
    public void MoveRotation(int direction)
    {
        //TODO: robot.IsInError should not be used by Robot, should be in some ErrorController
        if (!CurrentRobot || program.IsRunning() || CurrentRobot.ErrorDetected || RobotData.Instance.MovementMode == MovementMode.AUTO)
        {
            CurrentRobot.StopRotationMovement();
            return;
        }
        if (data.MovementMode == MovementMode.T2 || data.MovementMode == MovementMode.AUTO)
        {
            CurrentRobot.MoveRotation(data.MovementType, (MovementDirection)direction, data.RobotSpeed * data.MaxRobotJointSpeed);
        }
        else
        {
            CurrentRobot.MoveRotation(data.MovementType, (MovementDirection)direction, data.RobotSpeed * data.MaxRobotJointSpeed * 0.3f);
        }
    }

    /// <summary>
    /// Stops moving position of a robot end point
    /// </summary>
    public void StopMovingPosition()
    {
        if (!CurrentRobot) return;
        CurrentRobot.StopPositionMovement();
    }

    /// <summary>
    /// Stops rotation of a robot end point
    /// </summary>
    public void StopMovingRotation()
    {
        if (!CurrentRobot) return;
        CurrentRobot.StopRotationMovement();
    }


    private void ApplySpeedChangeToCurrentMovement()
    {
        if (currentRobot.IsMovedManually)
        {
            var pendantManualSpeed = data.RobotSpeed;
            var movementModeModiffier = data.MovementMode == MovementMode.T1 ? 0.3f : 1f;
            var movementTypeModifier = currentRobot.IsMovingRotation || data.MovementType == MovementType.Joint ? data.MaxRobotJointSpeed : data.MaxRobotSpeed;

            currentRobot.SetManualSpeed(pendantManualSpeed * movementModeModiffier * movementTypeModifier);
        }
        else if (currentRobot.IsExecutingMoveTo)
        {
            shouldCountinueFromLastInstruction = true;
            StopAllCoroutines();
            data.IsRunning = false;
            currentRobot.KillMovement();
            try
            {
                MoveTo((MoveInstruction)RobotData.Instance.LoadedProgram.Instructions[moveToInstructionIndex]);
            }
            catch { }
        }
        else if (currentRobot.IsExecutingInstructions)
        {
            shouldCountinueFromLastInstruction = true;
            StopAllCoroutines();
            data.IsRunning = false;
            currentRobot.KillMovement();
            RunProgram();
        }
    }

    /// <summary>
    /// Run currently loaded program
    /// </summary>
    public void RunProgram()
    {
        if (data.LoadedProgram != null && data.LoadedProgram.Instructions != null && data.LoadedProgram.Instructions.Count > 0)
        {
            StopAllCoroutines();
            CurrentRobot.ErrorDetected = false;

            switch (data.StepMode)
            {
                case StepMode.NORMAL:
                    StartCoroutine(RunningProgram());
                    break;
                case StepMode.STEP:
                    StartCoroutine(ContinueProgram());
                    break;
            }
        }
    }

    public void SwitchStepMode()
    {
        data.StepMode = (StepMode)((int)(data.StepMode + 1) % 2);
    }

    public void SwitchStepMode(StepMode targetMode)
    {
        data.StepMode = targetMode;
    }

    /// <summary>
    /// Switches state of a left deadman
    /// </summary>
    /// <param name="isOn">Is the daedman switch on?</param>
    public void SwitchLeftDeadman(bool isOn)
    {
        DeadmanLeftPushed = isOn;
    }

    /// <summary>
    /// Switches state of a right deadman
    /// </summary>
    /// <param name="isOn">Is the daedman switch on?</param>
    public void SwitchRightDeadman(bool isOn)
    {
        DeadmanRightPushed = isOn;
    }

    /// <summary>
    /// Changes coordinate system between all the options in MovementType
    /// </summary>
    public void CycleCoordinateSystem()
    {
        if (RobotData.Instance.MovementMode == MovementMode.AUTO || CurrentRobot.IsMoving)
        {
            return;
        }
        data.MovementType = data.MovementType + 1;
        if ((int)data.MovementType > 3)
        {
            data.MovementType = MovementType.Base;
        }

        CurrentRobot.SwitchMovementType(data.MovementType);
    }

    /// <summary>
    /// Used to cut off power to the robot, stops all movement in place
    /// </summary>
    public void EmergencyStop()
    {
        shouldCountinueFromLastInstruction = true;
        StopAllCoroutines();
        data.IsRunning = false;
        CurrentRobot.EmergencyStop();
    }

    /// <summary>
    /// Stops movement of robot without error
    /// </summary>
    public void SoftStop()
    {
        StopAllCoroutines();
        data.IsRunning = false;
        CurrentRobot.SoftStop();
    }

    /// <summary>
    /// Moves robot to home position
    /// </summary>
    public void GoToHomePosition()
    {
        CurrentRobot.MoveToPoint(data.HomePoint, data.MaxRobotSpeed * data.RobotSpeed, InstructionMovementType.LINEAR);
    }

    public void MoveTo(MoveInstruction instruction)
    {
        if (!currentRobot.IsMoving)
        {
            StopAllCoroutines();
            StartCoroutine(moveTo(instruction));
        }
    }

    private IEnumerator moveTo(MoveInstruction instruction)
    {
        moveToInstructionIndex = RobotData.Instance.LoadedProgram.Instructions.IndexOf(instruction);
        data.IsRunning = true;
        currentRobot.IsExecutingMoveTo = true;

        yield return ExecuteInstruction(instruction);

        data.IsRunning = false;
        currentRobot.IsExecutingMoveTo = false;
    }

    /// <summary>
    /// Switches current movement mode to specified one
    /// </summary>
    /// <param name="mode">Specified movement mode</param>
    public void SwitchMovementMode(MovementMode mode)
    {
        if (mode == MovementMode.AUTO)
            CurrentRobot.ErrorDetected = false;
        if (data.MovementMode == MovementMode.AUTO && mode != MovementMode.AUTO)
            program.StopProgram(true);
        data.MovementMode = mode;
    }

    /// <summary>
    /// Load program into robot data
    /// </summary>
    /// <param name="program">Progrma to load</param>
    public void LoadProgram(Program program)
    {
        if (RobotData.Instance.LoadedProgram != program)
        {
            RobotData.Instance.CurrentRunningInstructionIndex = 0;
            data.LoadedProgram = program;
        }
    }

    /// <summary>
    /// Coroutine allowing pogram to run 
    /// </summary>
    /// <returns>Waiting for next frame or timing the next instruction</returns>
    private IEnumerator RunningProgram()
    {
        data.IsRunning = true;
        int currentInstruction = shouldCountinueFromLastInstruction ? data.CurrentRunningInstructionIndex : 0;
        shouldCountinueFromLastInstruction = false;
        while (currentInstruction < data.LoadedProgram.Instructions.Count && !ShouldStopProgram)
        {
            data.CurrentRunningInstructionIndex = currentInstruction;

            if (data.StepMode == StepMode.STEP)
            {
                shouldCountinueFromLastInstruction = true;
                break;
            }

            if (!data.LoadedProgram.Instructions[currentInstruction].isCommented)
            {
                yield return new WaitForEndOfFrame();
                yield return ExecuteInstruction(data.LoadedProgram.Instructions[currentInstruction]);
            }

            currentInstruction = (currentInstruction + 1);
            if (currentInstruction >= data.LoadedProgram.Instructions.Count)
            {
                if (!ShouldStopAfterProgram)
                {
                    if (data.MovementMode == MovementMode.AUTO)
                    {
                        currentInstruction %= data.LoadedProgram.Instructions.Count;
                    }
                }
                OnProgramEnding?.Invoke();
            }
        }
        data.IsRunning = false;
    }

    private IEnumerator ExecuteInstruction(Instruction instruction)
    {
        switch (instruction)
        {
            case DelayInstruction delayInstruction:
                float delayTime = delayInstruction.delayTime;
                yield return new WaitForSeconds(delayTime);
                break;
            case MoveInstruction moveInstruction:
                if (moveInstruction.isCommented) break;
                InstructionMovementType movementType = moveInstruction.movementType;
                float approx = moveInstruction.ApproximationAmount;
                //
                // Process approximation
                //
                Point point = data.LoadedProgram.SavedPoints[moveInstruction.pointNumber];
                float speed = moveInstruction.Speed;
                if (movementType == InstructionMovementType.LINEAR)
                {
                    yield return CurrentRobot.MoveToPointLinear(point.position, point.rotation, speed);
                }
                else
                {
                    yield return CurrentRobot.MoveToPointJoint(point.jointAngles, speed);
                }
                break;
            case UseToolInstruction useToolInstruction:
                var actionType = useToolInstruction.actionType;
                if (actionType == InstructionToolAction.TOOLON)
                {
                    ToolOn();
                }
                else
                {
                    ToolOff();
                }
                float toolDelay = 0.5f;
                yield return new WaitForSeconds(toolDelay);
                break;
            case DigitalInInstruction dinInstruction:
                if (dinInstruction.targetState == true)
                {
                    DigitalInput.Instance.array.SetBit(dinInstruction.bitIndex);
                }
                else
                {
                    DigitalInput.Instance.array.ResetBit(dinInstruction.bitIndex);
                }
                break;
            case IfBlockInstruction ifBlockInstruction:
                //
                // //
                //
                break;
            case EmptyInstruction _:
                break;
            default:
                break;
        }
    }

    private IEnumerator ContinueProgram()
    {
        data.IsRunning = true;
        yield return new WaitForEndOfFrame();
        if (data.LoadedProgram.Instructions[data.CurrentRunningInstructionIndex].isCommented)
        {
            data.CurrentRunningInstructionIndex = (data.CurrentRunningInstructionIndex + 1) % data.LoadedProgram.Instructions.Count;
        }

        yield return ExecuteInstruction(data.LoadedProgram.Instructions[data.CurrentRunningInstructionIndex]);
        data.CurrentRunningInstructionIndex = (data.CurrentRunningInstructionIndex + 1) % data.LoadedProgram.Instructions.Count;
        data.IsRunning = false;
        shouldCountinueFromLastInstruction = true;

        if (data.StepMode == StepMode.NORMAL)
        {
            RunProgram();
            yield break;
        }
    }

    /// <summary>
    /// Activates current tool
    /// </summary>
    public void ToolOn()
    {
        if (ErrorRequester.HasAllErrorsReset() && ErrorRequester.HasResetAlarmErrors())
        {
            CurrentRobot.RobotGrasper.ToolOn();
        }

    }

    /// <summary>
    /// Deactivates current tool
    /// </summary>
    public void ToolOff()
    {
        if (ErrorRequester.HasAllErrorsReset() && ErrorRequester.HasResetAlarmErrors())
        {
            CurrentRobot.RobotGrasper.ToolOff();
        }
    }
}
