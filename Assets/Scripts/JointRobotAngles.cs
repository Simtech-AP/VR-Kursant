using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains limit and flag if the limit has been reached
/// </summary>
[System.Serializable]
public class AngleData
{
    /// <summary>
    /// Current angle of that pivot
    /// </summary>
    public float Angle;
    /// <summary>
    /// Did we reach limit angle?
    /// </summary>
    public bool Limit;

    public Vector3 ClampRot;

    public bool LowerLimit;

    public AngleData(float v1, bool v2)
    {
        Angle = v1;
        Limit = v2;
        ClampRot = new Vector3(0, 0, 0);
        LowerLimit = false;
    }

    public AngleData(float v1, bool v2, Vector3 rot, bool lowLimit)
    {
        Angle = v1;
        Limit = v2;
        ClampRot = new Vector3(rot.x, rot.y, rot.z);
        LowerLimit = lowLimit;
    }
}

/// <summary>
/// Contains data regarding limits of an axis
/// </summary>
[System.Serializable]
public class Pivot
{
    /// <summary>
    /// Reference to origin point of axis
    /// </summary>
    public Transform Transform;
    /// <summary>
    /// Axis of a robot pivot
    /// </summary>
    public Vector3 Axis;
    /// <summary>
    /// Maximal axis rotation
    /// </summary>
    public float MaxAxisLimit;
    /// <summary>
    /// Minimal axis rotation
    /// </summary>
    public float MinAxisLimit;
}

/// <summary>
/// Contains logic for clampng rotation of axes of robot
/// </summary>
public class JointRobotAngles : MonoBehaviour
{
    public bool VisualizeLimits = false;
    /// <summary>
    /// List of robot pivots
    /// </summary>
    [SerializeField]
    private List<Pivot> pivots;
    /// <summary>
    /// Reference to clamping rotations object
    /// </summary>
    [SerializeField]
    private ClampRobotLimitAngle clampRobot;
    /// <summary>
    /// Robot end point
    /// </summary>
    [SerializeField]
    private Transform endPoint;
    /// <summary>
    /// List of limits infos for each pivot
    /// </summary>
    private List<AngleData> angles = new List<AngleData>();
    /// <summary>
    /// List of current angles of pivots
    /// </summary>
    private List<float> ang = new List<float>();

    private Vector3 prevEndPointPosition = Vector3.zero;
    private Vector3 prevEndPointDirection = Vector3.zero;
    private float timer = 0;
    private static float movementDetectedThreshold = 0.01f;

    /// <summary>
    /// Sets up default values
    /// </summary>
    private void Awake()
    {
        for (int i = 0; i < pivots.Count; ++i)
        {
            angles.Add(new AngleData(0f, false));
        }

        ang.Clear();
        for (int i = 0; i < angles.Count; ++i)
        {
            ang.Add(angles[i].Angle);
        }

        prevEndPointPosition = endPoint.localPosition;
    }

    /// <summary>
    /// Adds methods to events
    /// </summary>
    private void OnEnable()
    {

    }

    /// <summary>
    /// Adds methods to events
    /// </summary>
    private void OnDisable()
    {

    }

    /// <summary>
    /// Contiously checks if we have reached limit on any axis
    /// </summary>
    private void Update()
    {
        if (RobotData.Instance.MovementMode != MovementMode.AUTO && !RobotData.Instance.IsRunning)
        {
            for (int i = 0; i < pivots.Count; ++i)
            {
                Vector3 rot;
                PrepareAngles(pivots[i], out rot);

                if (RobotData.Instance.MovementType != MovementType.Joint)
                    rot = CalucalteLocalRotation(i).eulerAngles;

                ClampAxisLimit(i, rot);
                SetAngles(i, rot);

            }
        }

        timer += Time.deltaTime;
        if (ErrorRequester.HasAllErrorsReset() && ErrorRequester.HasResetAlarmErrors())
        {
            if (timer > 0.5f)
            {
                timer = 0;
            }
            prevEndPointDirection = (endPoint.localPosition - prevEndPointPosition).normalized;
            prevEndPointPosition = endPoint.localPosition;
        }
    }

    /// <summary>
    /// Sets axis rotation to set value
    /// </summary>
    /// <param name="i">Index of angle</param>
    /// <param name="rot">Rotation of axis to set</param>
    private void SetAngles(int i, Vector3 rot)
    {
        if (angles[i].Angle != GetValue(rot) && !angles[i].Limit)
        {
            angles[i].Angle = GetValue(rot);
            SetAnglesTarget();
        }
    }

    /// <summary>
    /// Calculates local rotation of chosen axis
    /// </summary>
    /// <param name="i">Index of axis</param>
    /// <returns>Rotation of axis in quaterion</returns>
    private Quaternion CalucalteLocalRotation(int i)
    {
        return i > 0 ? Quaternion.Inverse(pivots[i - 1].Transform.localRotation) * pivots[i].Transform.localRotation : pivots[i].Transform.localRotation;
    }

    /// <summary>
    /// When we reach limit detects which pivot has to be clamped
    /// </summary>
    /// <param name="i">Index of pivot</param>
    /// <param name="rot">Current rotation of pivot</param>
    private void ClampAxisLimit(int i, Vector3 rot)
    {
        if (angles[i].Limit == false && (GetValue(rot) > pivots[i].MaxAxisLimit || GetValue(rot) < pivots[i].MinAxisLimit))
        {
            SetLimit(i, (GetValue(rot) < pivots[i].MinAxisLimit));  //drugi parametr określa czy osiągnęliśmy dolny (true), czy górny (false) limit
        }
        else if (angles[i].Limit == true)
        {
            if (angles[i].LowerLimit)
            {
                if (GetValue(rot) > GetValue(angles[i].ClampRot) + movementDetectedThreshold && GetValue(rot) > pivots[i].MinAxisLimit)
                {
                    ResetLimit(i);
                }
                else if (GetValue(rot) < GetValue(angles[i].ClampRot) - movementDetectedThreshold && GetValue(rot) < pivots[i].MinAxisLimit)
                {
                    clampRobot.ResetOnlyLimitFlag();
                    SetLimit(i, true);
                }
            }
            else
            {
                if (GetValue(rot) < GetValue(angles[i].ClampRot) - movementDetectedThreshold && GetValue(rot) < pivots[i].MaxAxisLimit)
                {
                    ResetLimit(i);
                }
                else if (GetValue(rot) > GetValue(angles[i].ClampRot) + movementDetectedThreshold && GetValue(rot) > pivots[i].MaxAxisLimit)
                {
                    clampRobot.ResetOnlyLimitFlag();
                    SetLimit(i, false);
                }
            }
        }
    }

    /// <summary>
    /// Resets limit of specified pivot
    /// </summary>
    /// <param name="i">Index of pivot</param>
    private void ResetLimit(int i)
    {
        angles[i].Limit = false;
        clampRobot.ResetLimit(i);
    }

    /// <summary>
    /// Sets limit of pivot
    /// </summary>
    /// <param name="i">Index of pivot</param>
    private void SetLimit(int i, bool limitFlag)
    {
        Vector3 rot;
        clampRobot.SetLimit(i, limitFlag);
        angles[i].Limit = true;
        switch (RobotData.Instance.MovementType)
        {
            case MovementType.Joint:
                pivots[i].Transform.Rotate(pivots[i].Axis, -1.5f * Mathf.Sign(angles[i].Angle));
                break;
            case MovementType.User:
            case MovementType.Tool:
            case MovementType.Base:
                if (i <= 2)
                {
                    endPoint.transform.localPosition -= prevEndPointDirection * 0.01f;
                }
                else
                {
                    var robot = ControllersManager.Instance.GetController<RobotController>().CurrentRobot;
                    robot.SwitchToJoint(true);
                    pivots[i].Transform.Rotate(pivots[i].Axis, -1.5f * Mathf.Sign(angles[i].Angle));
                    robot.SwitchToJoint(false);
                }
                break;
        }

        PrepareAngles(pivots[i], out rot);
        if (RobotData.Instance.MovementType != MovementType.Joint)
            rot = CalucalteLocalRotation(i).eulerAngles;

        angles[i].ClampRot.x = rot.x;
        angles[i].ClampRot.y = rot.y;
        angles[i].ClampRot.z = rot.z;
        angles[i].LowerLimit = limitFlag;
    }

    /// <summary>
    /// Prepares data to set limit of pivot
    /// </summary>
    /// <param name="pivot">Pivot structure</param>
    /// <param name="rot">Current rotation of pivot</param>
    private void PrepareAngles(Pivot pivot, out Vector3 rot)
    {
        rot = Vector3.Scale(pivot.Transform.localEulerAngles, pivot.Axis);
    }

    /// <summary>
    /// Detects which value of a rotation to get for pivot
    /// </summary>
    /// <param name="rot">Vecotr rotation of pivot</param>
    /// <returns>Value of proper axis</returns>
    private float GetValue(Vector3 rot)
    {
        if ((int)rot.x != 0)
            return ((rot.x + 540) % 360) - 180;
        if ((int)rot.y != 0)
            return ((rot.y + 540) % 360) - 180;
        if ((int)rot.z != 0)
            return ((rot.z + 540) % 360) - 180;

        return 0;
    }

    /// <summary>
    /// Sets target to reach for pivots
    /// </summary>
    private void SetAnglesTarget()
    {
        ang.Clear();
        for (int i = 0; i < angles.Count; ++i)
        {
            ang.Add(angles[i].Angle);
        }
        RobotData.Instance.CurrentAngles = new Angles(ang);
    }

    /// <summary>
    /// Resets all limits of robot
    /// </summary>
    public void ResetLimits()
    {

    }

    private void OnDrawGizmos()
    {
        if (VisualizeLimits)
            for (int i = 0; i < pivots.Count; ++i)
            {
                DrawEllipse(pivots[i].MaxAxisLimit, pivots[i].MinAxisLimit, pivots[i].Transform.position, pivots[i].Transform.GetComponent<ConfigurableJoint>().axis, pivots[i].Transform.forward, 0.2f, 0.2f, 100, Color.green);
            }
    }

    private void DrawEllipse(float max, float min, Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, int segments, Color color, float duration = 0)
    {
        float angle = 0f;
        Quaternion rot = Quaternion.LookRotation(forward, up);
        Vector3 lastPoint = Vector3.zero;
        Vector3 thisPoint = Vector3.zero;

        for (int i = 0; i < segments + 1; i++)
        {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
            thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;

            if (i > 0)
            {
                Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
            }

            lastPoint = thisPoint;
            angle += 360f / segments;
        }

        thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * max) * radiusX;
        thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * max) * radiusY;
        Debug.DrawLine(pos, rot * thisPoint + pos, Color.blue, duration);

        thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * min) * radiusX;
        thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * min) * radiusY;
        Debug.DrawLine(pos, rot * thisPoint + pos, Color.blue, duration);
    }

    public Transform GetPivotOnLimit()
    {
        Debug.Log(this.name);
        return pivots[clampRobot.PivotErrorIndex].Transform;
    }

    public bool[] GetPivotsLimitArray()
    {
        bool[] activeLimits = new bool[angles.Count()];
        for (int i = 0; i < angles.Count(); i++)
        {
            activeLimits[i] = angles[i].Limit;
        }
        return activeLimits;
    }
}

