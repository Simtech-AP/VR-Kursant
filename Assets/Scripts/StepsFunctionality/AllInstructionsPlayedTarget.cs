using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllInstructionsPlayedTarget : StepEnabler
{

    private ProgramController programController;

    protected override void initialize()
    {
        programController = ControllersManager.Instance.GetController<ProgramController>();
    }


    protected override void onUpdate()
    {
        CheckForLastInstruction();
    }

    public void CheckForLastInstruction()
    {
        int currentInstructionIndex = RobotData.Instance.CurrentRunningInstructionIndex;
        int totalExecutableInstructionsCount = RobotData.Instance.LoadedProgram.Instructions.Count;
        if (currentInstructionIndex == totalExecutableInstructionsCount - 1)
        {
            Enabled = true;
        }
    }
}
