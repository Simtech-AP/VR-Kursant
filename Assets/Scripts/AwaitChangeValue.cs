using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChangeValueType
{
    APX,
    SPD
}


[System.Serializable]
public class MoveTypeSpeed
{
    public InstructionMovementType movementType;
    public float ChangeToValue;
}

public class AwaitChangeValue : StepEnabler
{
    public ChangeValueType Change;
    public List<MoveTypeSpeed> MoveTypeSpeed;
    public float ChangeToValue;

    [SerializeField] private bool requireSpecificLine = false;
    [SerializeField] private int lineIndexLequired = 0;

    protected override void onUpdate()
    {
        CheckForChange();
    }

    public void CheckForChange()
    {
        if (requireSpecificLine && lineIndexLequired != PendantData.Instance.CurrentInstructionIndex) { return; }

        switch (Change)
        {
            case ChangeValueType.APX:
                AproxChange();
                break;
            case ChangeValueType.SPD:
                SpdChange();
                break;
        }
    }

    private void SpdChange()
    {
        try
        {
            var instruction = (PendantData.Instance.EditedProgram.Instructions[PendantData.Instance.CurrentInstructionIndex] as MoveInstruction);
            var targetSpeed = instruction.GetDisplaySpeedValue();

            if (targetSpeed == MoveTypeSpeed.Find(x => x.movementType == instruction.movementType).ChangeToValue)
            {
                Enabled = true;
            }
            else
            {
                Enabled = false;
            }

        }
        catch { }
    }

    private void AproxChange()
    {
        try
        {
            if ((PendantData.Instance.EditedProgram.Instructions[PendantData.Instance.CurrentInstructionIndex] as MoveInstruction).ApproximationAmount == ChangeToValue)
            {
                Enabled = true;
            }
            else
            {
                Enabled = false;
            }

        }
        catch { }
    }
}
