using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Robot class representiong robot using matrices to move
/// </summary>
public class MatrixRobot : Robot
{
    /// <summary>
    /// Reference to robot controller
    /// </summary>
    [SerializeField]
    private MatrixRobotController robotController = default;
    /// <summary>
    /// Data used to move and visualize robot axes
    /// </summary>
    private List<float> angleData = new List<float>();
    /// <summary>
    /// References to axes/pivots of robot
    /// </summary>
    [SerializeField]
    private List<Transform> pivots = new List<Transform>();
    /// <summary>
    /// Reference to tool point pivot
    /// </summary>
    [SerializeField]
    private Transform toolPivot = default;
    /// <summary>
    /// Reference to end point of a robot 6th pivot
    /// </summary>
    [SerializeField]
    private Transform endingPoint = default;
    /// <summary>
    /// Direction of movement
    /// </summary>
    private Vector3 moveDirection;
    /// <summary>
    /// Rotation of movement
    /// </summary>
    private Vector3 rotationDirection;
    //----DEBUG
    /// <summary>
    /// Debug position to test movement of robot in editor
    /// </summary>
    public Vector3 debugPosition = Vector3.zero;
    /// <summary>
    /// Debug rotation to test movement of robot in editor
    /// </summary>
    public Vector3 debugRotation = Vector3.zero;
    /// <summary>
    /// Speed of translation for testing
    /// </summary>
    public float debugSpeed = 0.5f;
    /// <summary>
    /// Testing type of movement
    /// </summary>
    public InstructionMovementType debugType = InstructionMovementType.JOINT;

    /// <summary>
    /// Sets up reference to controller
    /// Sets up default variables for data
    /// </summary>
    public override void Start()
    {
        base.Start();
        robotController = FindObjectOfType<MatrixRobotController>();
        data.RobotSpeed = 0.1f;
        data.MaxRobotSpeed = 2f;
        data.MaxRobotJointSpeed = 150f;
    }

    /// <summary>
    /// Updates ending point positions to visual representation
    /// </summary>
    /// <param name="fromRobot">Are we updating visuals from robot controller?</param>
    public void UpdatePositions(bool fromRobot = true)
    {
        if (!robotController) return;
        if (fromRobot)
        {
            var tempAngleData = robotController.GetAngleData();
            if (tempAngleData.Equals(angleData)) return;
            angleData = tempAngleData;
        }
        if (angleData.Contains(float.NaN) || angleData.Count <= 0) return;
        float[] tempArr = new float[6];
        angleData.CopyTo(tempArr);
        List<float> visAngleData = tempArr.ToList();
        visAngleData[4] *= -1;
        visAngleData[5] *= -1;
        for (int i = 0; i < pivots.Count; i++)
        {
            if (i != 1)
                pivots[i].localRotation = Quaternion.Euler(pivots[i].localEulerAngles.x, pivots[i].localEulerAngles.y, visAngleData[i]);
            else
                pivots[i].localRotation = Quaternion.Euler(pivots[i].localEulerAngles.x, visAngleData[i], pivots[i].localEulerAngles.z);
        }
    }

    /// <summary>
    /// Trying to get data for rotation of axes
    /// </summary>
    [ContextMenu("Check Axes to Point")]
    private void CheckTargetPointRotation()
    {
        robotController.GetRotationFromAxes(angleData);
    }

    /// <summary>
    /// Allows robot to move
    /// </summary>
    /// <param name="type">Type of movement</param>
    /// <param name="direction">Direction of movement</param>
    /// <param name="speed">Speed of movement</param>
    public override void MovePosition(MovementType type, MovementDirection direction, float speed)
    {
        moveDirection = Vector3.zero;
        switch (direction)
        {
            case MovementDirection.Up:
                moveDirection = new Vector3(0, 0, 1);
                break;
            case MovementDirection.Down:
                moveDirection = new Vector3(0, 0, -1);
                break;
            case MovementDirection.Forward:
                moveDirection = new Vector3(1, 0, 0);
                break;
            case MovementDirection.Backward:
                moveDirection = new Vector3(-1, 0, 0);
                break;
            case MovementDirection.Right:
                moveDirection = new Vector3(0, 1, 0);
                break;
            case MovementDirection.Left:
                moveDirection = new Vector3(0, -1, 0);
                break;
        }
        data.MovementType = type;
        //data.RobotSpeed = speed;
        movingPosition = true;
    }

    /// <summary>
    /// Allows robot to rotate the tool
    /// </summary>
    /// <param name="type">Type of rotation</param>
    /// <param name="direction">Direction of rotation</param>
    /// <param name="speed">Speed of rotation</param>
    public override void MoveRotation(MovementType type, MovementDirection direction, float speed)
    {
        rotationDirection = Vector3.zero;
        switch (direction)
        {
            case MovementDirection.Up:
                rotationDirection = new Vector3(0, 0, 1);
                break;
            case MovementDirection.Down:
                rotationDirection = new Vector3(0, 0, -1);
                break;
            case MovementDirection.Forward:
                rotationDirection = new Vector3(1, 0, 0);
                break;
            case MovementDirection.Backward:
                rotationDirection = new Vector3(-1, 0, 0);
                break;
            case MovementDirection.Right:
                rotationDirection = new Vector3(0, 1, 0);
                break;
            case MovementDirection.Left:
                rotationDirection = new Vector3(0, -1, 0);
                break;
        }
        data.MovementType = type;
        //data.RobotSpeed = speed;
        movingRotation = true;
    }

    /// <summary>
    /// Moves and roatates robot according to specified values
    /// </summary>
    private void Update()
    {
        if (!ErrorDetected)
        {
            if (movingPosition)
            {
                switch (data.MovementType)
                {
                    case MovementType.Base:
                        MovePositionBase();
                        break;
                    case MovementType.Tool:
                    case MovementType.User:
                        MovePositionTool(moveDirection);
                        break;
                    case MovementType.Joint:
                        MoveJoint();
                        break;
                }
            }

            if (movingRotation)
            {
                switch (data.MovementType)
                {
                    case MovementType.Base:
                        MoveRotationBase();
                        break;
                    case MovementType.Tool:
                    case MovementType.User:
                        MoveRotationTool(rotationDirection);
                        break;
                    case MovementType.Joint:
                        MoveJoint();
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Moves robot 6th pivot according to tool position and rotation
    /// </summary>
    private void MovePositionTool(Vector3 moveDirection)
    {
        robotController.ChangedCoordinateSystem();
        Vector3 vectorDirection = Vector3.zero;

        if (moveDirection == Vector3.forward)
            vectorDirection = toolPivot.forward;
        else if (moveDirection == Vector3.back)
            vectorDirection = -toolPivot.forward;
        else if (moveDirection == Vector3.right)
            vectorDirection = toolPivot.right;
        else if (moveDirection == Vector3.left)
            vectorDirection = -toolPivot.right;
        else if (moveDirection == Vector3.up)
            vectorDirection = toolPivot.up;
        else if (moveDirection == Vector3.down)
            vectorDirection = -toolPivot.up;

        robotController.currentTarget.position += vectorDirection.normalized * data.RobotSpeed * data.MaxRobotSpeed * Time.deltaTime;
        RobotData.Instance.CurrentTarget = robotController.currentTarget;
    }

    /// <summary>
    /// Rotates robot 6th pivot according to tool position and rotation
    /// </summary>
    private void MoveRotationTool(Vector3 rotationDirection)
    {
        robotController.ChangedCoordinateSystem();
        Vector3 vectorDirection = Vector3.zero;

        if (rotationDirection == Vector3.forward)
            vectorDirection = toolPivot.forward;
        else if (rotationDirection == Vector3.back)
            vectorDirection = -toolPivot.forward;
        else if (rotationDirection == Vector3.right)
            vectorDirection = toolPivot.right;
        else if (rotationDirection == Vector3.left)
            vectorDirection = -toolPivot.right;
        else if (rotationDirection == Vector3.up)
            vectorDirection = toolPivot.up;
        else if (rotationDirection == Vector3.down)
            vectorDirection = -toolPivot.up;


        robotController.currentTarget.rotation += vectorDirection.normalized * data.RobotSpeed * data.MaxRobotSpeed * Time.deltaTime;
        RobotData.Instance.CurrentTarget = robotController.currentTarget;
    }

    /// <summary>
    /// Allows movement of robot in joint movement type
    /// </summary>
    private void MoveJoint()
    {
        robotController.ChangedCoordinateSystem();
        Tuple<int, float> axeAndDirection = new Tuple<int, float>(0, 0);
        List<float> axes = new List<float>() { moveDirection.x, moveDirection.y, moveDirection.z, rotationDirection.x, rotationDirection.y, rotationDirection.z };
        for (int i = 0; i < axes.Count; i++)
        {
            if (axes[i] != 0)
            {
                axeAndDirection = new Tuple<int, float>(i, axes[i]);
                break;
            }
        }
        robotController.RotateAxe(axeAndDirection.Item1, axeAndDirection.Item2 * data.RobotSpeed * data.MaxRobotJointSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Rotates robot 6th pivot according to base
    /// </summary>
    private void MoveRotationBase()
    {
        robotController.ChangedCoordinateSystem();
        robotController.currentTarget.rotation += rotationDirection * data.RobotSpeed * data.MaxRobotJointSpeed * Time.deltaTime;
        RobotData.Instance.CurrentTarget = robotController.currentTarget;
    }

    /// <summary>
    /// Moves position of robot 6th pivot according to base
    /// </summary>
    private void MovePositionBase()
    {
        robotController.ChangedCoordinateSystem();
        robotController.currentTarget.position += moveDirection * data.RobotSpeed * data.MaxRobotSpeed * Time.deltaTime;
        RobotData.Instance.CurrentTarget = robotController.currentTarget;
    }

    /// <summary>
    /// Stops movement of robot
    /// </summary>
    public override void StopPositionMovement()
    {
        movingPosition = false;
        moveDirection = Vector3.zero;
    }

    /// <summary>
    /// Stops rotation of robot
    /// </summary>
    public override void StopRotationMovement()
    {
        movingRotation = false;
    }

    /// <summary>
    /// Main coroutine allowing to move robot to a point
    /// </summary>
    /// <param name="position">Position to move to</param>
    /// <param name="rotation">Rotation to rotate to</param>
    /// <param name="speed">Speed of translation</param>
    /// <param name="movementType">Type of movement</param>
    /// <returns>Handle to coroutine</returns>
    public override IEnumerator MoveToPoint(Point point, float speed, InstructionMovementType movementType)
    {
        yield return robotController.GoToPoint(speed, point, movementType);
    }

    /// <summary>
    /// Testing method to use in editor
    /// </summary>
    [ContextMenu("Test point in joint mode")]
    private void TestPoints()
    {
        // StartCoroutine(MoveToPoint(debugPosition, debugRotation, debugSpeed, debugType));
    }

    /// <summary>
    /// Stops robot with raising error
    /// </summary>
    public override void EmergencyStop()
    {

    }

    /// <summary>
    /// Stops robot without error
    /// </summary>
    public override void SoftStop()
    {

    }

    /// <summary>
    /// Get current point of 6th pivot
    /// </summary>
    /// <returns>Current position of 6th pivot</returns>
    public override Point GetCurrentPoint()
    {
        return new Point(robotController.currentTarget.position, robotController.currentTarget.rotation, new List<Quaternion>());
    }

    /// <summary>
    /// Allows to get a home point of current robot
    /// </summary>
    /// <returns>Home point of a robot</returns>
    public override Point GetHomePoint()
    {
        return data.HomePoint;
    }

    /// <summary>
    /// Switches to specified movement type
    /// </summary>
    /// <param name="movementType">Movemnt type to switch to</param>
    public override void SwitchMovementType(MovementType movementType)
    {
        if (movementType == MovementType.Base)
        {
            CalculateBaseEndPoint();
        }
        else if (movementType == MovementType.Tool)
        {
            CalculateToolEndPoint(data);
        }
    }

    /// <summary>
    /// Calculates ending point difference according to robota data
    /// </summary>
    /// <param name="data">Reference to robot data</param>
    private void CalculateToolEndPoint(RobotData data)
    {
        Vector3 dest = Vector3.zero;
        Vector3 rot = Vector3.zero;

        if (data.IsTCPCollected())
        {
            for (int i = 0; i < data.TCPPoint.Length; ++i)
            {
                dest += data.TCPPoint[i].Item1;
            }
            dest /= data.TCPPoint.Length;


            for (int i = 0; i < data.TCPPoint.Length; ++i)
            {
                rot += data.TCPPoint[i].Item2;
            }
            rot /= data.TCPPoint.Length;

        }
        else
        {
            FollowPivot6 tool = toolPivot.GetComponent<FollowPivot6>();
            dest = tool.GetPosition(0.4f);
            rot = toolPivot.eulerAngles;
        }

        Vector3 dist = endingPoint.transform.position - dest;
        robotController.lambda6 = Math.Abs(dist.z);
        robotController.currentTarget.position = dest;
        robotController.currentTarget.rotation = rot;
    }

    /// <summary>
    /// Resets 6th pivot tool difference
    /// </summary>
    private void CalculateBaseEndPoint()
    {
        robotController.lambda6 = 0;
    }

    public override void MoveToHomePosition(bool instant)
    {

    }
}
