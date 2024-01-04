using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramLineStateChangeCondition : StepEnabler
{
    [Header("Program Line State Change Condition")]
    [SerializeField] private int lineIndex = 0;
    [SerializeField] private InstructionType fromType;
    [SerializeField] private InstructionType toType;
    private InstructionType prevType;

    protected override void initialize()
    {
        prevType = PendantData.Instance.EditedProgram.Instructions[lineIndex].type;
    }

    protected override void onUpdate()
    {
        ComparePrevStateWithCurrent();
    }

    public void ComparePrevStateWithCurrent()
    {
        if (prevType == fromType && PendantData.Instance.EditedProgram.Instructions[lineIndex].type == toType)
        {
            Enabled = true;
        }

        prevType = PendantData.Instance.EditedProgram.Instructions[lineIndex].type;
    }
}
