using UnityEngine;
using System;
using UnityEngine.Events;

[HidePropertiesInInspector("OnInitialize", "OnCleanup", "OnEnabledStateReached", "OnEnabledStateLost", "statePersistent", "checkCountinously")]
public abstract class InstructionSpecificRequirements : StepEnabler
{

    protected abstract bool assertRequirement(Instruction newInstruction);

    public bool AssertRequirement(Instruction newInstruction)
    {
        bool result = false;

        try
        {
            result = assertRequirement(newInstruction);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occurred while asserting condition, defaulting to false. Message payload: " + e.Message);
        }

        Enabled = result;

        return result;
    }
}