using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JointRobotAngularSpeedLimiter : MonoBehaviour
{
    [Serializable]
    private class LimitedAxis
    {
        private static float ignoreAbove = 10000;
        [SerializeField] private Transform axis;
        [SerializeField] private float speedLimit;
        private Vector3 prevFrameRotation = default;

        public bool AssertSpeedExceedLimit()
        {
            float angularAxisVelocity = (prevFrameRotation - axis.localEulerAngles).magnitude / Time.fixedDeltaTime;

            // Debug.Log(angularAxisVelocity.ToString("0"));

            prevFrameRotation = axis.localEulerAngles;

            return angularAxisVelocity > speedLimit && angularAxisVelocity < LimitedAxis.ignoreAbove;
        }
    }


    [SerializeField] private List<LimitedAxis> limitedAxes = default;

    [SerializeField] private RobotController robotController;

    private void FixedUpdate()
    {
        var isRobotMoving = robotController.CurrentRobot.IsMoving;

        if (isRobotMoving && RobotData.Instance.MovementMode != MovementMode.AUTO)
        {
            if (RobotData.Instance.CurrentRunningInstruction.type == InstructionType.MOVE && ((MoveInstruction)RobotData.Instance.CurrentRunningInstruction).movementType != InstructionMovementType.JOINT)
            {
                foreach (var limAxis in limitedAxes)
                {
                    if (limAxis.AssertSpeedExceedLimit())
                    {
                        ErrorRequester.RaiseError("R-1005");
                    }
                }
            }
        }
    }
}
