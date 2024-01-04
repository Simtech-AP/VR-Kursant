using UnityEngine;

public class ApxValueRequirement : InstructionSpecificRequirements
{
    [SerializeField]
    private float expectedAPXValue = default;
    protected override bool assertRequirement(Instruction newInstruction)
    {
        return ((MoveInstruction)newInstruction).ApproximationAmount == expectedAPXValue;
    }
}