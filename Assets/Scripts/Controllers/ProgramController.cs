using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Controls programmin of a robot and running a program
/// </summary>
public class ProgramController : Controller
{
    /// <summary>
    /// Reference to robot controller
    /// </summary>
    [SerializeField]
    private RobotController robotController = default;
    /// <summary>
    /// Reference to pendant data object
    /// </summary>
    private PendantData pendantData = default;
    /// <summary>
    /// Reference to robot data object
    /// </summary>
    private RobotData robotData = default;

    /// <summary>
    /// Action invoked when program starts (e.g. after pressing START button while robot had been stopped)
    /// </summary>
    public Action OnProgramRun = delegate { };

    /// <summary>
    /// Sets references to pendatn and robot data
    /// </summary>
    private void OnEnable()
    {
        pendantData = PendantData.Instance;
        robotData = RobotData.Instance;
    }

    /// <summary>
    /// Adds instruction to current program
    /// </summary>
    /// <param name="instruction">Instruction to add</param>
    public void AddInstruction(Instruction instruction)
    {
        pendantData.EditedProgram.Instructions.Add(instruction);
        PendantData.OnUpdatedData.Invoke(pendantData);
    }

    public void ReplaceInstruction(int index, Instruction instruction)
    {
        if (index < pendantData.EditedProgram.Instructions.Count)
        {
            pendantData.EditedProgram.Instructions[index] = instruction;
        }
    }

    /// <summary>
    /// Changes point on specified index
    /// </summary>
    /// <param name="point">Point to change to</param>
    /// <param name="index">Index of a point in a list</param>
    public void ChangePoint(int index, Point point)
    {
        pendantData.EditedProgram.ChangePoint(index, point);
        PendantData.OnUpdatedData.Invoke(pendantData);
    }

    /// <summary>
    /// Removes selected instruction from a program at a given index
    /// </summary>
    /// <param name="index">Index of an instruction</param>
    public void RemoveInstruction(int index)
    {
        if (pendantData.EditedProgram.Instructions.Count >= index - 1)
            pendantData.EditedProgram.Instructions.RemoveAt(index);
        PendantData.OnUpdatedData.Invoke(pendantData);
    }

    public void ToggleInstructionCommentStatus()
    {
        var currentInstruction = pendantData.EditedProgram.Instructions[pendantData.CurrentInstructionIndex];
        currentInstruction.isCommented = !currentInstruction.isCommented;
        PendantData.OnUpdatedData.Invoke(pendantData);
    }

    /// <summary>
    /// Inserts instruction at specified index in a program
    /// </summary>
    /// <param name="instruction">Instruction to insert</param>
    /// <param name="index">Index of inserted instruction</param>
    public void InsertInstruction(Instruction instruction, int index, EditInstructionType insertType)
    {
        if (pendantData.EditedProgram.Instructions.Count > index - 1)
        {
            if (pendantData.EditedProgram.Instructions[index].type == InstructionType.EMPTY && instruction.type != InstructionType.EMPTY)
            {
                ReplaceInstruction(index, instruction);
            }
            else
            {

                switch (insertType)
                {
                    case EditInstructionType.INSERT_ABOVE:
                        pendantData.EditedProgram.Instructions.Insert(index, instruction);
                        break;
                    case EditInstructionType.INSERT_UNDER:
                        pendantData.EditedProgram.Instructions.Insert(index + 1, instruction);
                        break;

                }
            }
            if (instruction.type == InstructionType.MOVE)
            {
                robotController.CurrentRobot.UpdateRobotTarget();

                ((MoveInstruction)instruction).pointNumber = pendantData.EditedProgram.SavedPoints.Count;
                pendantData.EditedProgram.SavedPoints.Add(new Point(robotData.CurrentTarget));
            }
        }
        PendantData.OnUpdatedData?.Invoke(pendantData);
    }

    /// <summary>
    /// Is the program currently running?
    /// </summary>
    /// <returns>Currently running flag</returns>
    public bool IsRunning()
    {
        return robotData.IsRunning;
    }

    /// <summary>
    /// Allows to load and run program using current robot
    /// </summary>
    /// <param name="program">Program to run</param>
    public void RunProgram(Program program)
    {
        if (!ErrorRequester.HasAnyErrors() &&
            ErrorRequester.HasAllErrorsReset() &&
            ErrorRequester.HasResetAlarmErrors())
        {
            if (robotData.MovementMode == MovementMode.AUTO)
            {
                robotController.ShouldStopProgram = false;
                if (robotData.IsRunning)
                    return;
                robotController.LoadProgram(program);
                robotController.RunProgram();
                OnProgramRun?.Invoke();
            }
            else if (robotData.MovementMode == MovementMode.T1 || robotData.MovementMode == MovementMode.T2)
            {
                if (robotController.DeadmanLeftPushed || robotController.DeadmanRightPushed)
                {
                    robotController.ShouldStopProgram = false;
                    if (robotData.IsRunning)
                        return;
                    robotController.LoadProgram(program);
                    robotController.RunProgram();
                    OnProgramRun?.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// Runs currently loaded program
    /// </summary>
    public void RunProgram()
    {
        if (!ErrorRequester.HasAnyErrors() &&
            ErrorRequester.HasAllErrorsReset() &&
            ErrorRequester.HasResetAlarmErrors())
        {
            if (robotData.MovementMode == MovementMode.AUTO)
            {
                robotController.ShouldStopProgram = false;
                if (robotData.IsRunning)
                    return;
                robotController.RunProgram();
                OnProgramRun?.Invoke();
            }
            else if (robotData.MovementMode == MovementMode.T1 || robotData.MovementMode == MovementMode.T2)
            {
                if (robotController.DeadmanLeftPushed || robotController.DeadmanRightPushed)
                {
                    robotController.ShouldStopProgram = false;
                    if (robotData.IsRunning)
                        return;
                    robotController.RunProgram();
                    OnProgramRun?.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// Stops the program
    /// </summary>
    /// <param name="instant">Stop robot instant flag</param>
    public void StopProgram(bool instant)
    {
        robotController.ShouldStopProgram = true;
        if (instant)
            robotController.EmergencyStop();
    }

    public void RenameProgram(string name, string description)
    {
        pendantData.EditedProgram.Name = name;
        pendantData.EditedProgram.Description = description;
        PendantData.OnUpdatedData?.Invoke(pendantData);
    }

    /// <summary>
    /// Adds program to programs list
    /// </summary>
    /// <param name="program">Program to add</param>
    public void AddProgram(Program program)
    {
        List<Program> currentPrograms = pendantData.SavedPrograms;
        currentPrograms.Add(program);
        pendantData.SavedPrograms = currentPrograms;
    }

    /// <summary>
    /// Removes program from saved programs list
    /// </summary>
    /// <param name="program">Program to remove</param>
    /// <returns>Was the program removed?</returns>
    public bool RemoveProgram(Program program)
    {
        if (pendantData.SavedPrograms.Contains(program))
        {
            if (program == pendantData.EditedProgram)
            {
                return false;
            }
            else if (program == robotData.LoadedProgram)
            {
                return false;
            }
        }
        List<Program> currentPrograms = pendantData.SavedPrograms;
        currentPrograms.Remove(program);
        pendantData.SavedPrograms = currentPrograms;
        PendantData.OnUpdatedData(PendantData.Instance);

        return true;
    }

    /// <summary>
    /// Load program to edit it on pendant
    /// </summary>
    /// <param name="program"></param>
    public void EditProgram(ref Program program)
    {
        pendantData.EditedProgram = program;
        robotController.LoadProgram(pendantData.EditedProgram);
    }

    /// <summary>
    /// Starts robot in default mode with default program
    /// </summary>
    public void StartRobotDefault()
    {
        RunProgram(pendantData.SavedPrograms[0]);
    }

    public void MoveTo()
    {
        if (!ErrorRequester.HasAnyErrors() &&
            ErrorRequester.HasAllErrorsReset() &&
            ErrorRequester.HasResetAlarmErrors())
        {
            int index = pendantData.CurrentInstructionIndex;
            var instruction = pendantData.EditedProgram.Instructions[index];

            if (instruction.type == InstructionType.MOVE)
            {
                robotController.MoveTo((MoveInstruction)instruction);
            }

            RobotData.Instance.CurrentRunningInstructionIndex = index;
        }
    }

    public int GetExecutableLinesCount()
    {

        int count = 0;

        foreach (var instruction in robotData.LoadedProgram.Instructions)
        {
            count += !instruction.isCommented ? 1 : 0;
        }

        return count;
    }

    public int GetMoveInstructionLinesCount()
    {
        return robotData.LoadedProgram.Instructions.Where(x => x.type == InstructionType.MOVE && !x.isCommented).ToList().Count;
    }
}


