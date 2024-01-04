using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPointMovedCondition : StepEnabler
{
    [Header("End Point Moved Condition")]

    [SerializeField] private bool shouldCheckForDistance = default;
    [SerializeField] private float minimumDistanceChange = default;
    [SerializeField] private bool shouldCheckForAngle = default;
    [SerializeField] private float minimumAngleChange = default;

    private Transform robotEndPoint;
    private Target targetOnStart = default;

    protected override void initialize()
    {
        robotEndPoint = ControllersManager.Instance.GetController<RobotController>().CurrentRobot.RobotEndPoint.transform;

        targetOnStart.position = robotEndPoint.position;
        targetOnStart.rotation = robotEndPoint.rotation.eulerAngles;
    }

    public void CheckForEndPointMovement()
    {
        if ((Vector3.Distance(targetOnStart.position, robotEndPoint.position) > minimumDistanceChange && shouldCheckForDistance)
            || (Vector3.Angle(targetOnStart.rotation, robotEndPoint.rotation.eulerAngles) > minimumAngleChange && shouldCheckForAngle))
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
        CheckForEndPointMovement();
    }
}
