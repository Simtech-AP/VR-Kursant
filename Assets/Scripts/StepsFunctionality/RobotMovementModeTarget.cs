using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMovementModeTarget : StepEnabler
{
    public MovementMode TargetMode;

    protected override void onUpdate()
    {
        CheckTargetMode();
    }

    public void CheckTargetMode()
    {

        if (TargetMode == RobotData.Instance.MovementMode)
        {
            Enabled = true;
        }
        else
        {
            Enabled = false;
        }
    }
}
