using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class JointChangeData
{
    public int joindIndex = 0;
    public float minimumAngleChange = 0f;
}

public class JointAxisMovedCondition : StepEnabler
{
    [SerializeField] private JointChangeData jointChangeData = default;
    private Quaternion initialRotation = default;
    private JointRobot robot = default;

    private MovementType lastMovementType = default;


    protected override void initialize()
    {
        robot = ControllersManager.Instance.GetController<RobotController>().CurrentRobot;
        initialRotation = robot.JointHierarchy[jointChangeData.joindIndex].localRotation;
    }

    public void CheckForAngleDifference()
    {
        if (robot.MovementType == MovementType.Joint)
        {
            var currentRotation = robot.JointHierarchy[jointChangeData.joindIndex].localRotation;
            if (lastMovementType != MovementType.Joint)
            {
                UpdateInitialRotation();
                lastMovementType = MovementType.Joint;
            }
            if (Quaternion.Angle(initialRotation, currentRotation) > jointChangeData.minimumAngleChange)
            {
                Enabled = true;
            }
            else
            {
                Enabled = false;
            }
        }
        else
        {
            lastMovementType = robot.MovementType;
        }
    }

    protected override void onUpdate()
    {
        CheckForAngleDifference();
    }

    public void UpdateInitialRotation()
    {
        initialRotation = robot.JointHierarchy[jointChangeData.joindIndex].localRotation;
    }
}
