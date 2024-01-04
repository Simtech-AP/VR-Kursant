using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class SpeedValueRequirement : InstructionSpecificRequirements
{

    [Serializable]
    private class ValueForMovementType
    {
        public InstructionMovementType movementType;
        public int value;
    }

    [SerializeField]
    private List<ValueForMovementType> validValues = default;


    protected override bool assertRequirement(Instruction newInstruction)
    {
        var moveInstruction = (MoveInstruction)newInstruction;
        return validValues.Any(x => x.movementType == moveInstruction.movementType && x.value == moveInstruction.GetDisplaySpeedValue());
    }
}