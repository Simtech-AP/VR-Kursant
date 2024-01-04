using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LinearMovementCondition : StepEnabler
{
    private enum LinearMoveOption
    {
        X,
        Y,
        Z,
        Xrot,
        Yrot,
        Zrot
    }

    [Header("Linear Movement Condition")]
    [SerializeField] private LinearMoveOption option;
    [SerializeField] private float minimalChange;
    private Target targetOnStart;

    private MovementType lastMovementType = default;

    protected override void initialize()
    {
        ControllersManager.Instance.GetController<RobotController>().CurrentRobot.UpdateRobotTarget();
        SetInitialTarget();
    }

    public void SetInitialTarget()
    {
        targetOnStart = RobotData.Instance.CurrentTarget;
    }

    public void CheckForEndPointMovement()
    {
        if (RobotData.Instance.MovementType == MovementType.Base)
        {
            if (lastMovementType != MovementType.Base)
            {
                UpdateTargetOnStart();
                lastMovementType = MovementType.Base;
            }
            bool fulfilled = false;
            var currentTarget = RobotData.Instance.CurrentTarget;

            switch (option)
            {
                case LinearMoveOption.X:
                    fulfilled = Mathf.Abs(currentTarget.position.x - targetOnStart.position.x) >= minimalChange;
                    break;
                case LinearMoveOption.Y:
                    fulfilled = Mathf.Abs(currentTarget.position.y - targetOnStart.position.y) >= minimalChange;
                    break;
                case LinearMoveOption.Z:
                    fulfilled = Mathf.Abs(currentTarget.position.z - targetOnStart.position.z) >= minimalChange;
                    break;
                case LinearMoveOption.Xrot:
                    fulfilled = Mathf.Abs(currentTarget.rotation.z - targetOnStart.rotation.z) >= minimalChange;
                    break;
                case LinearMoveOption.Yrot:
                    fulfilled = Mathf.Abs(currentTarget.rotation.y - targetOnStart.rotation.y) >= minimalChange;
                    break;
                case LinearMoveOption.Zrot:
                    fulfilled = Mathf.Abs(currentTarget.rotation.x - targetOnStart.rotation.x) >= minimalChange;
                    break;
                default:
                    break;

            }

            Enabled = fulfilled;
        }
        else
        {
            lastMovementType = RobotData.Instance.MovementType;
        }
    }

    protected override void onUpdate()
    {
        CheckForEndPointMovement();
    }

    public void UpdateTargetOnStart()
    {
        targetOnStart = RobotData.Instance.CurrentTarget;
    }
}
