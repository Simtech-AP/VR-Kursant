using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTypeChangeChecker : StepEnabler
{
    private InstructionMovementType previousMovementType;
    private int previousLineIndex = 0;
    private MoveInstruction currentInstruction = null;

    protected override void initialize()
    {
        var instruction = ((MoveInstruction)(PendantData.Instance.EditedProgram.Instructions[PendantData.Instance.CurrentInstructionIndex]));

        foreach (var i in PendantData.Instance.EditedProgram.Instructions)
        {
            if (instruction != null)
            {
                break;
            }
            else
            {
                instruction = (MoveInstruction)i;
            }
        }

        if (instruction != null)
        {
            currentInstruction = instruction;
            previousMovementType = instruction.movementType;
            previousLineIndex = PendantData.Instance.CurrentInstructionIndex;
        }
        else
        {
            Enabled = true;
        }
    }

    protected override void onUpdate()
    {
        CheckForInstructionTypeChange();
    }

    private void CheckForInstructionTypeChange()
    {
        var currentIndex = PendantData.Instance.CurrentInstructionIndex;

        if (currentIndex != previousLineIndex)
        {
            previousLineIndex = currentIndex;

            try
            {
                currentInstruction = ((MoveInstruction)(PendantData.Instance.EditedProgram.Instructions[PendantData.Instance.CurrentInstructionIndex]));
                previousMovementType = currentInstruction.movementType;
            }
            catch
            {
                currentInstruction = null;
            }
        }
        else
        {
            if (currentInstruction != null)
            {
                var currentType = currentInstruction.movementType;

                if (currentType != previousMovementType)
                {
                    Enabled = true;
                }
            }
        }

    }
}
