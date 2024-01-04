using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentProgramLineIndexTarget : StepEnabler
{
    [Header("Current Program Line Index Target")]
    [SerializeField] private int lineIndexTarget;
    [SerializeField] private bool countBackwards;

    protected override void onUpdate()
    {
        AssertIndex();
    }
    public void AssertIndex()
    {
        if (!countBackwards && PendantData.Instance.CurrentInstructionIndex == lineIndexTarget)
        {
            Enabled = true;
        }
        else if (countBackwards && (PendantData.Instance.EditedProgram.Instructions.Count - 1 - PendantData.Instance.CurrentInstructionIndex) == lineIndexTarget)
        {
            Enabled = true;
        }
        else
        {
            Enabled = false;
        }
    }
}
