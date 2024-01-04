using System;
using UnityEngine;

/// <summary>
/// Allows checking of distance of tool to TCP Point
/// </summary>
public class TCPPointChecker : StepEnabler
{
    [Header("TCP Point Checker")]
    /// <summary>
    /// Refrence to transfrm of TCP Point
    /// </summary>
    [SerializeField]
    private Transform tcpPoint = default;
    /// <summary>
    /// Reference to transform of TCP Pointer
    /// </summary>
    [SerializeField]
    private Transform tcpPointer = default;
    /// <summary>
    /// Maximum distance difference to enable setting TCP Point
    /// </summary>
    public float diff = 2f;
    /// <summary>
    /// Are we in range of TCP Point?
    /// </summary>
    public bool Range = true;
    /// <summary>
    /// Difference between different axes of saved point
    /// </summary>
    public float savedDiff = 2f;
    /// <summary>
    /// Saved TCP Points
    /// </summary>
    public Tuple<Vector3, Vector3>[] SavedPoints;
    /// <summary>
    /// Reference to robot tool
    /// </summary>
    private FollowPivot6 grasper;

    /// <summary>
    /// Sets up references
    /// </summary>
    protected override void initialize()
    {
        SavedPoints = RobotData.Instance.TCPPoint;
        grasper = FindObjectOfType<RobotController>().CurrentRobot.GetComponentInChildren<FollowPivot6>();
    }

    /// <summary>
    /// CHecks if we are in range of saving TCP Point
    /// </summary>
    protected override void onUpdate()
    {
        CheckPointMetCondition();
    }

    private void CheckPointMetCondition()
    {
        Action met = Range == true ? () => InRange() : met = () => OutRange();
        met.Invoke();
    }

    /// <summary>
    /// If we are in range of saving enable going to next step
    /// </summary>
    private void InRange()
    {
        if ((tcpPoint.position - tcpPointer.position).magnitude < diff)
        {
            if (CheckIfAllDifferent(grasper.transform.eulerAngles))
            {
                Enabled = true;
                SetPointersColor(Color.green);
            }
        }
        else
        {
            SetPointersColor(Color.red);
            Enabled = false;
        }
    }

    private void SetPointersColor(Color color)
    {
        tcpPoint.GetComponent<MeshRenderer>().material.color = color;
    }

    /// <summary>
    /// Checks if different axes are on different planes
    /// </summary>
    /// <param name="eulers">Axes amounts to check</param>
    /// <returns>Are axes different?</returns>
    private bool CheckIfAllDifferent(Vector3 eulers)
    {
        for (int i = 0; i < SavedPoints.Length; ++i)
        {
            if (Math.Abs(SavedPoints[i].Item2.x - eulers.x) < savedDiff)
                return false;
            if (Math.Abs(SavedPoints[i].Item2.y - eulers.y) < savedDiff)
                return false;
            if (Math.Abs(SavedPoints[i].Item2.z - eulers.z) < savedDiff)
                return false;
        }

        return true;
    }

    /// <summary>
    /// If we are out of range of setting TCP Point disable going to next step
    /// </summary>
    private void OutRange()
    {
        if ((tcpPoint.position - tcpPointer.position).magnitude > diff)
        {
            Enabled = true;
            SetPointersColor(Color.green);
        }
        else
        {
            Enabled = false;
            SetPointersColor(Color.red);
        }
    }
}
