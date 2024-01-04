using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSpeedTarget : StepEnabler
{
    [Header("Robot Speed Target")]
    [SerializeField] private int targetSpeedPercentage;
    [SerializeField] private int targetMargin;

    private Robot robot;

    protected override void initialize()
    {
        robot = ControllersManager.Instance.GetController<RobotController>().CurrentRobot;

    }

    public void CheckRobotSpeed()
    {
        var speedPercentage = (int)(robot.data.RobotSpeed * 100);

        var upperBound = targetSpeedPercentage + targetMargin;
        var bottomBound = targetSpeedPercentage - targetMargin;

        if (speedPercentage >= bottomBound && speedPercentage <= upperBound)
        {
            Enabled = true;
        }
        else
        {
            Enabled = false;
        }
    }

    protected override void onUpdate()
    {
        CheckRobotSpeed();
    }
}
