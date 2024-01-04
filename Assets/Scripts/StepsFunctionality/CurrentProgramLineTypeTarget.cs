using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentProgramLineTypeTarget : StepEnabler
{
    [Header("Current Program Line Index Target")]
    [SerializeField] private bool negateCondition = false;
    [SerializeField] private InstructionType instructionTypeTarget;

    protected override void onUpdate()
    {
        AssertlineType();
    }

    public void AssertlineType()
    {
        Debug.Log(PendantData.Instance.CurrentInstructionIndex);
        var result = PendantData.Instance.CurrentInstruction.type == instructionTypeTarget;
        Enabled = negateCondition ? !result : result;
    }
}
