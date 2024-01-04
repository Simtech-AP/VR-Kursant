using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepModeTarget : StepEnabler
{
    public StepMode TargetMode;

    protected override void onUpdate()
    {
        CheckTargetMode();
    }

    public void CheckTargetMode()
    {

        if (TargetMode == RobotData.Instance.StepMode)
        {
            Enabled = true;
        }
        else
        {
            Enabled = false;
        }
    }
}
