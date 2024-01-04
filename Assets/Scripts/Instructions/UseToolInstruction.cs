using System;
using UnityEngine;

public enum InstructionToolAction
{
    TOOLON,
    TOOLOFF
}

[CreateAssetMenu(menuName = "Instructions/ToolInstruction")]
public class UseToolInstruction : Instruction
{
    public InstructionToolAction actionType = default;

    public UseToolInstruction()
    {
        type = InstructionType.USE_TOOL;
        maxSelectablePartIndex = 1;
    }

    public UseToolInstruction(UseToolInstruction other) : base(other)
    {
        this.actionType = other.actionType;
    }

    protected override string generateInstructionSpecificText()
    {

        var _text = "TOOL ";
        _text += actionType == InstructionToolAction.TOOLOFF ? "OFF" : "ON";
        return _text;
    }
    public override void ProcessInput(string input)
    {
        if (selectedPart == 1 && input.Equals("Enter"))
        {
            toggleActionType();
            PendantData.OnUpdatedData?.Invoke(PendantData.Instance);
        }

    }

    protected override Tuple<int, int> selectPart(int part)
    {
        selectedPart = part;
        if (part == 1)
        {
            int endIndexOffset = actionType == InstructionToolAction.TOOLOFF ? 3 : 2;
            return new Tuple<int, int>(5, 5 + endIndexOffset);
        }
        return new Tuple<int, int>(0, 5);
    }

    private void toggleActionType()
    {
        if (actionType == InstructionToolAction.TOOLOFF)
        {
            actionType = InstructionToolAction.TOOLON;
        }
        else
        {
            actionType = InstructionToolAction.TOOLOFF;
        }
    }

}
