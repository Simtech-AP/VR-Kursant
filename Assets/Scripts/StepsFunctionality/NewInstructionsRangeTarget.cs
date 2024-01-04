using UnityEngine;
using System;
public class NewInstructionsRangeTarget : StepEnabler
{

    [Serializable]
    private class IndexRange
    {
        public int min = default;
        public int max = default;
    }

    [Header("New Instructions Range Target")]
    [SerializeField]
    private IndexRange indexRange = default;

    [SerializeField]
    private InstructionType requiredType = default;
    private Program loadedProgramCopy = default;

    protected override void initialize()
    {
        loadedProgramCopy = new Program(RobotData.Instance.LoadedProgram);
    }

    protected override void onUpdate()
    {
        assertCondition();
    }

    private void assertCondition()
    {
        var result = true;

        for (int i = indexRange.min; i <= indexRange.max; ++i)
        {
            try
            {
                result &= RobotData.Instance.LoadedProgram.Instructions[i].type == requiredType;
            }
            catch
            {
                result = false;
            }
        }

        Enabled = result;
    }
}