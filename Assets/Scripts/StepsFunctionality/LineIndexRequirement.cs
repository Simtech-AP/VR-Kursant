using System;
using UnityEngine;

public class LineIndexRequirement : InstructionSpecificRequirements
{
    [SerializeField] private int indexRequired;

    protected override bool assertRequirement(Instruction newInstruction)
    {
        return RobotData.Instance.LoadedProgram.Instructions.IndexOf(newInstruction) == indexRequired;
    }
}