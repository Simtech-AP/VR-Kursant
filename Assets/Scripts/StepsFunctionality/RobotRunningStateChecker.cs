using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotRunningStateChecker : StepEnabler
{
    [Header("Robot Running State Checker")]
    [SerializeField] private bool targetRunningState = false;

    protected override void onUpdate()
    {
        CheckState();
    }

    public void CheckState()
    {

        Enabled = targetRunningState ? RobotData.Instance.IsRunning : !RobotData.Instance.IsRunning;
    }
}
