using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class DebugDrawPoints : MonoBehaviour
{
    [Header("Trasnforms")]
    public Transform LinearParent;
    public Transform JointParent;
    public Transform endPoint;

    [Header("Drawing variables")]
    public float trackDuration = 0f;
    public float pointSize = 0f;
    public float directionMarkerSize = 0f;


    private InstructionMovementType moveType = InstructionMovementType.LINEAR;
    private Vector3 prevWorldPosition = Vector3.zero;

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || PendantData.Instance.EditedProgram == null) return;

        try
        {
            MoveInstruction instruction = (MoveInstruction)RobotData.Instance.LoadedProgram.Instructions[RobotData.Instance.CurrentRunningInstructionIndex];
            moveType = instruction.movementType;
        }
        catch { }

        Vector3 endPointLocation = Vector3.zero;

        Gizmos.matrix = LinearParent.transform.localToWorldMatrix;
        Handles.matrix = LinearParent.transform.localToWorldMatrix;

        if ((moveType == InstructionMovementType.JOINT && RobotData.Instance.IsRunning) ||
        (RobotData.Instance.MovementType == MovementType.Joint && !RobotData.Instance.IsRunning))
        {
            endPointLocation = -endPoint.localPosition + JointParent.transform.position - LinearParent.position;
            endPointLocation = new Vector3(-endPointLocation.z, -endPointLocation.x, endPointLocation.y);
        }
        else
        {
            endPointLocation = endPoint.localPosition;
        }

        for (int i = 0; i < PendantData.Instance.EditedProgram.SavedPoints.Count; i++)
        {
            var pos = PendantData.Instance.EditedProgram.SavedPoints[i].position;
            var rot = PendantData.Instance.EditedProgram.SavedPoints[i].rotation;

            DrawPoints(pos, rot);
            DrawLines(endPointLocation, pos);
            DrawLabels(endPointLocation, i, pos);
            DrawTrack();
        }

        prevWorldPosition = endPoint.position;

    }

    private void DrawTrack()
    {
        Debug.DrawLine(prevWorldPosition, endPoint.position, Color.blue, trackDuration);
    }

    private void DrawLabels(Vector3 endPointLocation, int i, Vector3 pos)
    {
        Handles.color = Color.white;
        Handles.Label(pos + new Vector3(0, pointSize + 0.05f, 0), "P" + i);
        Handles.Label(pos + new Vector3(0, 0, pointSize + 0.05f), Vector3.Distance(pos, endPointLocation).ToString("0.000"));
    }

    private static void DrawLines(Vector3 endPointLocation, Vector3 pos)
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(pos, endPointLocation);
    }

    private void DrawPoints(Vector3 pos, Vector3 rot)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, pointSize);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos + Quaternion.Euler(rot) * Vector3.forward * directionMarkerSize);
        Gizmos.DrawLine(pos, pos + Quaternion.Euler(rot) * Vector3.up * directionMarkerSize / 2);
    }
}

#endif