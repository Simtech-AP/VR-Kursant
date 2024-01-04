using System;
using UnityEngine;

/// <summary>
/// Class allowing for full TCP configuration on pendant
/// </summary>
public class TCPPendant : MonoBehaviour
{
    /// <summary>
    /// Reference to grasper tool
    /// </summary>
    private GameObject grasper;

    /// <summary>
    /// Links grasper of current robot to reference
    /// </summary>
    /// <param name="robot"></param>
    public void LinkRobot(Robot robot)
    {
        grasper = robot.GetComponentInChildren<FollowPivot6>().gameObject;
    }

    /// <summary>
    /// Calculates current TCP point using grasper position
    /// </summary>
    /// <param name="index"></param>
    public void CalculatePointTCP(int index)
    {
        var currentArray = RobotData.Instance.TCPPoint;
        currentArray[index] = new System.Tuple<Vector3, Vector3>(
                new Vector3((float)Math.Round(grasper.transform.position.x, 1),
                                (float)Math.Round(grasper.transform.position.y, 1),
                                (float)Math.Round(grasper.transform.position.z, 1)),
                new Vector3((float)Math.Round(grasper.transform.rotation.eulerAngles.x, 1),
                                (float)Math.Round(grasper.transform.rotation.eulerAngles.y, 1),
                                (float)Math.Round(grasper.transform.rotation.eulerAngles.z, 1)));
        RobotData.Instance.TCPPoint = currentArray;
    }

    /// <summary>
    /// Calculates orientation of TCP point
    /// </summary>
    /// <param name="index">Index of TCP point to calculate</param>
    public void CalculateOrientationTCP(int index)
    {
        var currentArray = RobotData.Instance.TCPPoint;
        currentArray[index] = new System.Tuple<Vector3, Vector3>(
            currentArray[index].Item1,
            new Vector3((float)Math.Round(grasper.transform.rotation.eulerAngles.x, 1),
            (float)Math.Round(grasper.transform.rotation.eulerAngles.y, 1),
            (float)Math.Round(grasper.transform.rotation.eulerAngles.z, 1)));
        RobotData.Instance.TCPPoint = currentArray;
    }

    /// <summary>
    /// Allows setting TCP point from end of tool
    /// </summary>
    /// <param name="index">Index of TCP point to set</param>
    public void CalculateDirectTCP(int index)
    {
        var currentArray = RobotData.Instance.TCPPoint;
        currentArray[index] = new System.Tuple<Vector3, Vector3>(
                new Vector3((float)Math.Round(grasper.transform.position.x, 1),
                                (float)Math.Round(grasper.transform.position.y, 1),
                                (float)Math.Round(grasper.transform.position.z, 1)),
                currentArray[index].Item2);
        RobotData.Instance.TCPPoint = currentArray;
    }

    /// <summary>
    /// Moves robot to point
    /// </summary>
    /// <param name="list">Menu list to get point from</param>
    public void MoveToPoint(MenuList list)
    {
        if (list.OptionIndex < 3)
        {
            var point = new Point(RobotData.Instance.TCPPoint[list.OptionIndex].Item1, RobotData.Instance.TCPPoint[list.OptionIndex].Item2);
            StartCoroutine(FindObjectOfType<RobotController>().CurrentRobot.MoveToPoint(point, 2f, InstructionMovementType.LINEAR));
        }
    }

    /// <summary>
    /// Moves robot to specified orientation
    /// </summary>
    /// <param name="list">Menu list to get orientation from</param>
    public void MoveToOrientation(MenuList list)
    {
        if (list.OptionIndex < 3)
        {
            var point = new Point(RobotData.Instance.TCPPoint[list.OptionIndex].Item1, RobotData.Instance.TCPPoint[list.OptionIndex].Item2);
            StartCoroutine(FindObjectOfType<RobotController>().CurrentRobot.MoveToPoint(point, 2f, InstructionMovementType.JOINT));
        }
    }
}
