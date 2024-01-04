using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class NewInstructionTarget : StepEnabler
{
    [Header("New Instruction Target")]
    [SerializeField]
    private InstructionType requiredInstructionType = default;

    [SerializeField]
    private List<InstructionSpecificRequirements> additionalRequirements = default;

    private Program loadedProgramCopy = default;
    private Program loadedProgram = default;
    private Instruction newInstruction = default;

    private Action runOnUpdate = delegate { };

    protected override void initialize()
    {
        loadedProgramCopy = new Program(PendantData.Instance.EditedProgram);

        runOnUpdate += LookForNewInstruction;
    }

    protected override void onUpdate()
    {
        runOnUpdate();
    }

    private void LookForNewInstruction()
    {
        CheckForNewProgramLoadedEvent();
        CheckForProgramModifications();
        CheckForNewInstructionOfRequiredType();
    }

    private void CheckForNewProgramLoadedEvent()
    {
        if (wasNewProgramLoaded())
        {
            Debug.Log("New program loaded");
            loadedProgram = PendantData.Instance.EditedProgram;
            loadedProgramCopy = new Program(PendantData.Instance.EditedProgram);
            newInstruction = null;

            runOnUpdate -= AssertInstructionSpecificCondition;
            // runOnUpdate += LookForNewInstruction;
        }
    }

    private void CheckForProgramModifications()
    {
        if (wasInstructionInsertedOrRemoved() && !wasNewInstructionOfRequiredTypeInserted())
        {
            Debug.Log("Progam modified");
            loadedProgramCopy = new Program(PendantData.Instance.EditedProgram);
        }

        if (!isSavedInstructionPresentInProgram() && newInstruction != null)
        {
            Debug.Log("Instruction missing from program");
            newInstruction = null;
            loadedProgramCopy = new Program(PendantData.Instance.EditedProgram);
        }
    }

    private bool isSavedInstructionPresentInProgram()
    {
        return RobotData.Instance.LoadedProgram.Instructions.Contains(newInstruction);
    }

    private void CheckForNewInstructionOfRequiredType()
    {
        if (wasNewInstructionOfRequiredTypeInserted())
        {
            Debug.Log("New instruction of type inserted");

            newInstruction = findNewInstructionInProgram();
            loadedProgramCopy = new Program(PendantData.Instance.EditedProgram);

            // runOnUpdate -= LookForNewInstruction;
            runOnUpdate += AssertInstructionSpecificCondition;
        }
    }

    private void AssertInstructionSpecificCondition()
    {
        CheckForNewProgramLoadedEvent();
        if (newInstruction != null && (additionalRequirements == null || additionalRequirements.Count == 0))
        {
            Enabled = true;
        }
        else if (newInstruction != null)
        {
            var result = true;

            foreach (var req in additionalRequirements)
            {
                result &= req.AssertRequirement(newInstruction);
            }

            Enabled = result;
        }
        else
        {
            additionalRequirements.ForEach(x => x.Enabled = false);
            Enabled = false;
        }
    }

    private bool wasNewInstructionOfRequiredTypeInserted()
    {
        return getInstructionOfRequiredTypeCount(loadedProgramCopy) < getInstructionOfRequiredTypeCount(PendantData.Instance.EditedProgram);
    }

    private bool wasInstructionInsertedOrRemoved()
    {
        return loadedProgramCopy.Instructions.Count != PendantData.Instance.EditedProgram.Instructions.Count;
    }

    bool wasNewProgramLoaded()
    {
        return loadedProgram != PendantData.Instance.EditedProgram;
    }

    private Instruction findNewInstructionInProgram()
    {
        for (int i = 0; i < loadedProgramCopy.Instructions.Count; ++i)
        {
            if (loadedProgramCopy.Instructions[i].GetInstructionText() != PendantData.Instance.EditedProgram.Instructions[i].GetInstructionText())
            {
                return PendantData.Instance.EditedProgram.Instructions[i];
            }
        }

        if (PendantData.Instance.EditedProgram.Instructions.Last().type == requiredInstructionType)
        {
            return PendantData.Instance.EditedProgram.Instructions.Last();
        }

        Debug.LogError("Something went wrong, new instruction wasn't found.");

        return null;
    }


    private int getInstructionOfRequiredTypeCount(Program program)
    {
        return program.Instructions.Where(x => x.type == requiredInstructionType).ToList().Count;
    }

}