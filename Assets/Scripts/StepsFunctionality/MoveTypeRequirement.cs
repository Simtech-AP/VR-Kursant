using System;
using UnityEngine;

public class MoveTypeRequirement : InstructionSpecificRequirements
{
    private enum RequiredMovementType
    {
        ANY,
        LINEAR,
        JOINT
    }

    [SerializeField]
    private RequiredMovementType requiredMovementType = default;

    protected override bool assertRequirement(Instruction newInstruction)
    {
        switch (requiredMovementType)
        {
            case RequiredMovementType.ANY:
                return true;
            case RequiredMovementType.LINEAR:
                return ((MoveInstruction)newInstruction).movementType == InstructionMovementType.LINEAR;
            case RequiredMovementType.JOINT:
                return ((MoveInstruction)newInstruction).movementType == InstructionMovementType.JOINT;
            default:
                return false;
        }

    }
}