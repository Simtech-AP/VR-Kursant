using UnityEngine;

public class ToolOperationTypeRequirement : InstructionSpecificRequirements
{
    private enum ToolOperationType
    {
        ANY,
        ON,
        OFF
    }

    [SerializeField]
    private ToolOperationType requiredOperationType = default;
    protected override bool assertRequirement(Instruction newInstruction)
    {
        var newToolInstruction = (UseToolInstruction)newInstruction;

        switch (requiredOperationType)
        {
            case ToolOperationType.ANY:
                return true;
            case ToolOperationType.ON:
                return newToolInstruction.actionType == InstructionToolAction.TOOLON;
            case ToolOperationType.OFF:
                return newToolInstruction.actionType == InstructionToolAction.TOOLOFF;
            default:
                return false;
        }
    }
}