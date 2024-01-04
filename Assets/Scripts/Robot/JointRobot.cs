using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

/// <summary>
/// Base class for a robot
/// Contains full logic needed to control a single robot using Unity based joints
/// </summary>
public class JointRobot : Robot
{
    /// <summary>
    /// Maximum distance of robot ending pivot form base (first pivot - currently about 2.08f)
    /// </summary>
    public float Distance;
    public Transform objectJoint;
    /// <summary>
    /// The end point of a robot, usually at the base of a tool
    /// </summary>
    [SerializeField]
    private GameObject robotEndPoint = default;
    /// <summary>
    /// Reference point for safe movement calculation
    /// </summary>
    private GameObject toolReferencePoint = default;
    /// <summary>
    /// Transform of a base reference
    /// </summary>
    [SerializeField]
    private Transform baseFrame = default;
    /// <summary>
    /// Reference for user frame movement
    /// </summary>
    [SerializeField]
    private Transform userReference = default;
    /// <summary>
    /// Target controller reference
    /// </summary>
    [SerializeField]
    private TargetController targetController = default;
    /// <summary>
    /// List of a joints that will be used to join them when using joint movement
    /// </summary>
    [SerializeField]
    private List<Transform> jointHierarchy = default;
    /// <summary>
    /// Base of a joint movement
    /// </summary>
    [SerializeField]
    private Transform jointBase = default;
    /// <summary>
    /// Maximum speed of linear movement of an end point of a robot
    /// </summary>
    [SerializeField]
    private float maxMovementSpeed = 1f;
    /// <summary>
    /// Maximum speed of rotation movement of a tool
    /// </summary>
    [SerializeField]
    private float maxRotationSpeed = 35f;
    /// <summary>
    /// End point of a decoy robot
    /// </summary>
    [SerializeField]
    private Transform jointMoveEndPoint = default;
    /// <summary>
    /// Decoy robot hierarchy for joint instruction movement
    /// </summary>
    [SerializeField]
    private List<Transform> decoyRobotJoints = new List<Transform>();
    /// <summary>
    /// Base point for decoy robot
    /// </summary>
    [SerializeField]
    private Transform decoyBase = default;
    /// <summary>
    /// Switch for joint movement
    /// </summary>
    private bool jointMove = false;
    /// <summary>
    /// Current movement speed
    /// </summary>
    private float moveSpeed;
    /// <summary>
    /// Current rotation speed
    /// </summary>
    private float rotationSpeed;
    /// <summary>
    /// Current position movement direction
    /// </summary>
    private MovementDirection positionDirection = default;
    /// <summary>
    /// Current rotation direction
    /// </summary>
    private MovementDirection rotationDirection = default;
    /// <summary>
    /// Last position of end point used in correcting position
    /// </summary>
    private Vector3 lastPosition = Vector3.zero;
    /// <summary>
    /// Sequence of movement of an instruction
    /// </summary>
    private Sequence movementToPoint;
    /// <summary>
    /// home point of the robot
    /// </summary>
    private Point homePoint;
    /// <summary>
    /// Time needed to complete joint move
    /// </summary>
    private float jointMoveTimer = 0f;
    /// <summary>
    /// Current joint movement time
    /// </summary>
    private float jointTraverseTime = 0f;
    /// <summary>
    /// Rotations of a joints for linear interpolation of axes
    /// </summary>
    private List<Quaternion> jointBaseRotations = new List<Quaternion>();
    private List<Quaternion> targetJointAngles = new List<Quaternion>();
    /// <summary>
    /// Switch for checking if robot is in joint movement
    /// </summary>
    private bool _inJointMovement;
    private bool inJointMovement
    {
        get => _inJointMovement;
        set
        {
            RobotData.Instance.IsInJointConfiguration = value;
            _inJointMovement = value;
        }
    }
    /// <summary>
    /// Accessor for movement type
    /// </summary>
    public MovementType MovementType { get => data.MovementType; set => data.MovementType = value; }
    /// <summary>
    /// Accessor for robot end point
    /// </summary>
    public GameObject RobotEndPoint { get => robotEndPoint; }

    public List<Transform> JointHierarchy { get => jointHierarchy; }

    [SerializeField] private Grasper grasper;
    public Grasper RobotGrasper { get => grasper; }
    public GameObject activeIndicator = default;

    private Coroutine jointMoveProcess = default;
    private Coroutine homeMoveProcess = default;

    /// <summary>
    /// Setup starting elements:
    /// Link robot to controller
    /// Set distance between base and end point for movement calculations
    /// Set home position and rotation for relative movement
    /// </summary>
    public override void Start()
    {
        base.Start();

        // Force stop program if limit is reached during execution
        // ErrorController.OnErrorOccurred += (ErrorController controller, Error error) => { KillMovement(); };

        toolReferencePoint = GetComponentInChildren<FollowPivot6>().gameObject;
        data = RobotData.Instance;
        data.MaxRobotSpeed = maxMovementSpeed;
        data.MaxRobotJointSpeed = maxRotationSpeed;
        lastPosition = robotEndPoint.transform.position;

        SwitchToJoint(true);
        var homeAngles = new List<Quaternion>();
        foreach (var pivot in jointHierarchy)
        {
            homeAngles.Add(pivot.localRotation);
        }
        SwitchToJoint(false);

        homePoint = new Point()
        {
            position = robotEndPoint.transform.localPosition,
            rotation = robotEndPoint.transform.localEulerAngles,
            jointAngles = homeAngles
        };
        UpdateRobotTarget();
    }

    private Target GetNewTarget()
    {
        Vector3 position = Vector3.zero;
        Vector3 rotation = Vector3.zero;
        List<Quaternion> jointAngles = new List<Quaternion>();

        // define linear end point position and rotation
        if (MovementType == MovementType.Joint)
        {
            SwitchToJoint(false);
            position = robotEndPoint.transform.localPosition;
            rotation = robotEndPoint.transform.localEulerAngles;
            SwitchToJoint(true);
        }
        else if (MovementType == MovementType.Tool)
        {
            rotation = robotEndPoint.transform.localEulerAngles;
            position = robotEndPoint.transform.localPosition - Quaternion.Euler(rotation) * Vector3.forward * 0.35f;
        }
        else if (MovementType == MovementType.User)
        {
            // TODO
            position = robotEndPoint.transform.localPosition;
            rotation = robotEndPoint.transform.localEulerAngles;
        }
        else if (MovementType == MovementType.Base)
        {
            position = robotEndPoint.transform.localPosition;
            rotation = robotEndPoint.transform.localEulerAngles;
        }

        // define joint pivot angles
        if (MovementType == MovementType.Joint)
        {
            foreach (var pivot in jointHierarchy)
            {
                jointAngles.Add(pivot.localRotation);
            }
        }
        else
        {
            SwitchToJoint(true);
            foreach (var pivot in jointHierarchy)
            {
                jointAngles.Add(pivot.localRotation);
            }
            SwitchToJoint(false);
        }

        return new Target(position, rotation, jointAngles);
    }

    public void UpdateRobotTarget()
    {
        RobotData.Instance.CurrentTarget = GetNewTarget();
    }

    /// <summary>
    /// Home position helper method.
    /// </summary>
    /// <returns>Home (base) position</returns>
    public Vector3 GetHomePosition()
    {
        return homePoint.position;
    }

    /// <summary>
    /// Home rotation helper method.
    /// </summary>
    /// <returns>Home (base) rotation</returns>
    public Vector3 GetHomeRotation()
    {
        return homePoint.rotation;
    }

    /// <summary>
    /// Get current point of end of a robot.
    /// </summary>
    /// <returns>A Point structure containing position and rotation of end of a robot</returns>
    public override Point GetCurrentPoint()
    {
        return new Point() { position = robotEndPoint.transform.localPosition, rotation = robotEndPoint.transform.localEulerAngles };
    }

    /// <summary>
    /// Switches robot to allow movement of robot end in direction with a specified type, direction and speed. 
    /// </summary>
    /// <param name="type">Base reference point for a move</param>
    /// <param name="direction">Direction of a movement relative to the base reference point</param>
    /// <param name="speed">Speed of movement</param>
    public override void MovePosition(MovementType type, MovementDirection direction, float speed)
    {
        MovementType = type;
        positionDirection = direction;
        if (type == MovementType.Joint)
        {
            rotationSpeed = speed;
        }
        else
        {
            moveSpeed = speed;
        }
        movingPosition = true;
    }

    /// <summary>
    /// Switches robot to allow rotation of robot end to rotation with a specified type, direction and speed. 
    /// </summary>
    /// <param name="type">Base reference point for a move</param>
    /// <param name="direction">Direction of a rotation relative to the base reference point</param>
    /// <param name="speed">Speed of rotation</param>
    public override void MoveRotation(MovementType type, MovementDirection direction, float speed)
    {
        MovementType = type;
        rotationDirection = direction;
        rotationSpeed = speed;
        movingRotation = true;
    }

    public void SetManualSpeed(float speed)
    {
        if (movingRotation)
        {
            rotationSpeed = speed;
        }
        else if (movingPosition)
        {
            if (MovementType == MovementType.Joint)
            {
                rotationSpeed = speed;
            }
            else
            {
                moveSpeed = speed;
            }
        }
    }

    /// <summary>
    /// Stops movement of a robot end
    /// </summary>
    public override void StopPositionMovement()
    {
        movingPosition = false;
    }

    /// <summary>
    /// Stops rotation of a robot end
    /// </summary>
    public override void StopRotationMovement()
    {
        movingRotation = false;
    }

    /// <summary>
    /// Allows for visalisation of robot target
    /// </summary>
    /// <param name="isOn">Is the target on or off?</param>
    public void ShowTargetPoint(bool isOn)
    {
        targetController.ShowTarget(isOn);
    }

    /// <summary>
    /// Calculates current 6th pivot position in base mode
    /// </summary>
    private void CalculateBaseEndPoint()
    {
        FollowPivot6 tool = toolReferencePoint.GetComponent<FollowPivot6>();
        SwitchEndPoint(tool.GetPosition(0f), tool.GetRotation());
    }

    /// <summary>
    /// Calculates current 6th pivot position in tool mode
    /// </summary>
    /// <param name="data">Current robot data</param>
    private void CalculateToolEndPoint(RobotData data)
    {
        Vector3 dest = Vector3.zero;
        Vector3 rot = Vector3.zero;
        FollowPivot6 tool = toolReferencePoint.GetComponent<FollowPivot6>();

        if (data.IsTCPCollected())
        {
            for (int i = 0; i < data.TCPPoint.Length; ++i)
            {
                rot += data.TCPPoint[i].Item2;
            }
            rot /= data.TCPPoint.Length;
        }
        else
        {
            rot = tool.GetRotation().eulerAngles;
        }

        dest = tool.GetPosition(0.35f);
        SwitchEndPoint(dest, Quaternion.Euler(rot));
    }

    /// <summary>
    /// Calculates current 6th pivot position in tool mode
    /// </summary>
    /// <param name="data">Current robot data</param>
    private void CalculateUserEndPoint(RobotData data)
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
        userReference.position = dest;
        userReference.rotation = Quaternion.Euler(rot);
    }

    /// <summary>
    /// Changes end point of a preview object to specified position and rotation
    /// </summary>
    /// <param name="pos">Position to change end point to</param>
    /// <param name="rot">Rotation to change end point to</param>
    private void SwitchEndPoint(Vector3 pos, Quaternion rot)
    {
        var prevBody = robotEndPoint.GetComponent<FixedJoint>().connectedBody;
        robotEndPoint.GetComponent<FixedJoint>().connectedBody = null;
        robotEndPoint.transform.position = pos;
        robotEndPoint.transform.rotation = rot;
        robotEndPoint.GetComponent<FixedJoint>().connectedBody = prevBody;
    }

    /// <summary>
    /// Update method containing:
    /// Deadman switch - if any of deadman switches is off robot won't move
    /// If movement of position or rotation is requested robot will choose method according to type of movement
    /// </summary>
    private void Update()
    {
        if (!ErrorDetected)
        {
            if (movingPosition)
            {
                switch (MovementType)
                {
                    case MovementType.Base:
                        if (inJointMovement) SwitchToJoint(false);
                        MovePositionBase(positionDirection, Time.deltaTime);
                        break;
                    case MovementType.Tool:
                        if (inJointMovement) SwitchToJoint(false);
                        MovePositionTool(positionDirection, Time.deltaTime);
                        break;
                    case MovementType.User:
                        if (inJointMovement) SwitchToJoint(false);
                        MovePositionUser(positionDirection, Time.deltaTime);
                        break;
                    case MovementType.Joint:
                        if (!inJointMovement) SwitchToJoint(true);
                        MoveJoint(positionDirection, Time.deltaTime, false);
                        break;
                }

                UpdateRobotTarget();
            }

            if (movingRotation)
            {
                switch (MovementType)
                {
                    case MovementType.Base:
                        if (inJointMovement) SwitchToJoint(false);
                        MoveRotationBase(rotationDirection, Time.deltaTime);
                        break;
                    case MovementType.Tool:
                        if (inJointMovement) SwitchToJoint(false);
                        MoveRotationTool(rotationDirection, Time.deltaTime);
                        break;
                    case MovementType.User:
                        if (inJointMovement) SwitchToJoint(false);
                        MoveRotationUser(rotationDirection, Time.deltaTime);
                        break;
                    case MovementType.Joint:
                        if (!inJointMovement) SwitchToJoint(true);
                        MoveJoint(rotationDirection, Time.deltaTime, true);
                        break;
                }

                UpdateRobotTarget();
            }

        }

        maintainScale();
    }

    private void maintainScale()
    {
        foreach (var joint in jointHierarchy)
        {
            joint.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// Actual movmement of robot end is done in this method
    /// This is crucial for physics engine to correctly interpret joints and collisions
    /// </summary>
    private void FixedUpdate()
    {
        if (!ErrorDetected)
        {
            if (movingRotation || movingPosition)
            {
                if ((robotEndPoint.transform.position - jointHierarchy[0].position).magnitude > Distance)
                {
                    robotEndPoint.transform.position = lastPosition;
                    UpdateRobotTarget();
                }
                else
                {
                    lastPosition = robotEndPoint.transform.position;
                }
            }
            if (jointMove)
            {
                jointMoveTimer += Time.fixedDeltaTime;
                for (int i = 0; i < jointHierarchy.Count; i++)
                {
                    jointHierarchy[i].localRotation = Quaternion.Slerp(jointBaseRotations[i], targetJointAngles[i], jointMoveTimer / jointTraverseTime);
                }
                if (jointMoveTimer / jointTraverseTime >= 1f)
                {
                    jointMove = false;
                    jointMoveTimer = 0f;
                }
            }
        }
    }

    /// <summary>
    /// Allows robot to move in joint movement
    /// Direction of rotation is determined by parameters
    /// </summary>
    /// <param name="direction">Direction of rotation according to pendant buttons</param>
    /// <param name="deltaTime">Delta time of a frame, allows for time-consistent rotation of joints</param>
    /// <param name="isRotation">Determines if the pressed button is a rotation modifier (such as +X rot etc)</param>
    private void MoveJoint(MovementDirection direction, float deltaTime, bool isRotation)
    {
        if (deltaTime >= 1f) return;
        int jointIndex = 0;
        int moveDirection = 0;
        switch (direction)
        {
            case MovementDirection.Forward:
                if (isRotation) jointIndex = 3; else jointIndex = 0;
                moveDirection = 1;
                break;
            case MovementDirection.Backward:
                if (isRotation) jointIndex = 3; else jointIndex = 0;
                moveDirection = -1;
                break;
            case MovementDirection.Right:
                if (isRotation) jointIndex = 4; else jointIndex = 1;
                moveDirection = 1;
                break;
            case MovementDirection.Left:
                if (isRotation) jointIndex = 4; else jointIndex = 1;
                moveDirection = -1;
                break;
            case MovementDirection.Up:
                if (isRotation) jointIndex = 5; else jointIndex = 2;
                moveDirection = 1;
                break;
            case MovementDirection.Down:
                if (isRotation) jointIndex = 5; else jointIndex = 2;
                moveDirection = -1;
                break;
        }
        var joint = jointHierarchy[jointIndex].GetComponent<ConfigurableJoint>();
        var axis = Vector3.zero;
        if (joint.angularXMotion != ConfigurableJointMotion.Locked) axis = Vector3.right;
        else if (joint.angularYMotion != ConfigurableJointMotion.Locked) axis = Vector3.up;
        else axis = Vector3.forward;
        jointHierarchy[jointIndex].Rotate(axis * deltaTime * rotationSpeed * moveDirection);
    }

    /// <summary>
    /// Allows robot to move according to base reference point
    /// </summary>
    /// <param name="direction">Direction of movement</param>
    /// <param name="deltaTime">Delta time of a frame, allows for time-consistent movement</param>
    void MovePositionBase(MovementDirection direction, float deltaTime)
    {
        if (deltaTime >= 1f) return;
        Vector3 vectorDirection = Vector3.zero;
        switch (direction)
        {
            case MovementDirection.Forward:
                vectorDirection = -baseFrame.forward;
                break;
            case MovementDirection.Backward:
                vectorDirection = baseFrame.forward;
                break;
            case MovementDirection.Right:
                vectorDirection = baseFrame.right;
                break;
            case MovementDirection.Left:
                vectorDirection = -baseFrame.right;
                break;
            case MovementDirection.Up:
                vectorDirection = baseFrame.up;
                break;
            case MovementDirection.Down:
                vectorDirection = -baseFrame.up;
                break;
        }
        robotEndPoint.transform.position += vectorDirection.normalized * moveSpeed * deltaTime;
    }

    /// <summary>
    /// Allows robot to move according to tool reference coordinates
    /// </summary>
    /// <param name="direction">Direction of movement</param>
    /// <param name="deltaTime">Delta time of a frame, allows for time-consistent movement</param>
    void MovePositionTool(MovementDirection direction, float deltaTime)
    {
        if (deltaTime >= 1f) return;
        Vector3 vectorDirection = Vector3.zero;
        switch (direction)
        {
            case MovementDirection.Forward:
                vectorDirection = robotEndPoint.transform.forward;
                break;
            case MovementDirection.Backward:
                vectorDirection = -robotEndPoint.transform.forward;
                break;
            case MovementDirection.Right:
                vectorDirection = robotEndPoint.transform.right;
                break;
            case MovementDirection.Left:
                vectorDirection = -robotEndPoint.transform.right;
                break;
            case MovementDirection.Up:
                vectorDirection = robotEndPoint.transform.up;
                break;
            case MovementDirection.Down:
                vectorDirection = -robotEndPoint.transform.up;
                break;
        }
        robotEndPoint.transform.position += vectorDirection.normalized * moveSpeed * deltaTime;
    }

    /// <summary>
    /// Allows robot to move according to user reference frame
    /// </summary>
    /// <param name="direction">Direction of movement</param>
    /// <param name="deltaTime">Delta time of a frame, allows for time-consistent movement</param>
    private void MovePositionUser(MovementDirection direction, float deltaTime)
    {
        if (deltaTime >= 1f) return;
        Vector3 vectorDirection = Vector3.zero;
        switch (direction)
        {
            case MovementDirection.Forward:
                vectorDirection = userReference.forward;
                break;
            case MovementDirection.Backward:
                vectorDirection = -userReference.forward;
                break;
            case MovementDirection.Right:
                vectorDirection = userReference.right;
                break;
            case MovementDirection.Left:
                vectorDirection = -userReference.right;
                break;
            case MovementDirection.Up:
                vectorDirection = userReference.up;
                break;
            case MovementDirection.Down:
                vectorDirection = -userReference.up;
                break;
        }
        robotEndPoint.transform.position += vectorDirection.normalized * moveSpeed * deltaTime;
    }

    /// <summary>
    /// Allows robot to change tool rotation using base as a reference
    /// </summary>
    /// <param name="direction">Direction of rotation</param>
    /// <param name="deltaTime">Delta time of a frame, allows for time-consistent movement</param>
    void MoveRotationBase(MovementDirection direction, float deltaTime)
    {
        if (deltaTime >= 1f) return;
        Vector3 rotationDirection = Vector3.zero;
        switch (direction)
        {
            case MovementDirection.Forward:
                rotationDirection = baseFrame.forward;
                break;
            case MovementDirection.Backward:
                rotationDirection = -baseFrame.forward;
                break;
            case MovementDirection.Right:
                rotationDirection = baseFrame.right;
                break;
            case MovementDirection.Left:
                rotationDirection = -baseFrame.right;
                break;
            case MovementDirection.Up:
                rotationDirection = baseFrame.up;
                break;
            case MovementDirection.Down:
                rotationDirection = -baseFrame.up;
                break;
        }
        robotEndPoint.transform.Rotate(rotationDirection.normalized * rotationSpeed * deltaTime);
    }

    /// <summary>
    /// Allows robot to change tool rotation with tool frame reference
    /// </summary>
    /// <param name="direction">Direction of rotation</param>
    /// <param name="deltaTime">Delta time of a frame, allows for time-consistent movement</param>
    private void MoveRotationTool(MovementDirection direction, float deltaTime)
    {
        if (deltaTime >= 1f) return;
        Vector3 rotationDirection = Vector3.zero;
        switch (direction)
        {
            case MovementDirection.Forward:
                rotationDirection = robotEndPoint.transform.forward;
                break;
            case MovementDirection.Backward:
                rotationDirection = -robotEndPoint.transform.forward;
                break;
            case MovementDirection.Right:
                rotationDirection = robotEndPoint.transform.right;
                break;
            case MovementDirection.Left:
                rotationDirection = -robotEndPoint.transform.right;
                break;
            case MovementDirection.Up:
                rotationDirection = robotEndPoint.transform.up;
                break;
            case MovementDirection.Down:
                rotationDirection = -robotEndPoint.transform.up;
                break;
        }
        robotEndPoint.transform.Rotate(rotationDirection.normalized * rotationSpeed * deltaTime);
    }

    /// <summary>
    /// Allows robot to change tool rotation with user frame reference
    /// </summary>
    /// <param name="direction">Direction of rotation</param>
    /// <param name="deltaTime">Delta time of a frame, allows for time-consistent movement</param>
    private void MoveRotationUser(MovementDirection direction, float deltaTime)
    {
        if (deltaTime >= 1f) return;
        Vector3 rotationDirection = Vector3.zero;
        switch (direction)
        {
            case MovementDirection.Forward:
                rotationDirection = userReference.forward;
                break;
            case MovementDirection.Backward:
                rotationDirection = -userReference.forward;
                break;
            case MovementDirection.Right:
                rotationDirection = userReference.right;
                break;
            case MovementDirection.Left:
                rotationDirection = -userReference.right;
                break;
            case MovementDirection.Up:
                rotationDirection = userReference.up;
                break;
            case MovementDirection.Down:
                rotationDirection = -userReference.up;
                break;
        }
        robotEndPoint.transform.RotateAround(userReference.position, rotationDirection.normalized, rotationSpeed * deltaTime);
    }


    /// <summary>
    /// Switches manual movement of robot to joint and back
    /// </summary>
    /// <param name="enable">Is joint movement enabled?</param>
    public void SwitchToJoint(bool enable)
    {
        movementToPoint.TogglePause();
        if (enable)
        {
            for (int i = 0; i < jointHierarchy.Count - 1; i++)
            {
                jointHierarchy[i + 1].parent = jointHierarchy[i];
                jointHierarchy[i].GetComponent<Rigidbody>().isKinematic = true;
            }
        }
        else
        {
            foreach (Transform t in jointHierarchy)
            {
                t.parent = jointBase;
                t.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        inJointMovement = enable;
        movementToPoint.TogglePause();
    }

    /// <summary>
    /// Sets decoy robot hirarchy
    /// Needed to count axes rotations
    /// </summary>
    /// <param name="isJoined">Is the decoy in joint movement?</param>
    private void SetDecoyHierarchy(bool isJoined)
    {
        if (isJoined)
        {
            for (int i = 0; i < decoyRobotJoints.Count - 1; i++)
            {
                decoyRobotJoints[i + 1].parent = decoyRobotJoints[i];
                decoyRobotJoints[i].GetComponent<Rigidbody>().isKinematic = true;
            }
        }
        else
        {
            foreach (Transform t in decoyRobotJoints)
            {
                t.parent = decoyBase;
                t.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    /// <summary>
    /// Coroutine allowing robot to move using instructions
    /// </summary>
    /// <param name="position">Target position</param>
    /// <param name="rotation">Target rotation</param>
    /// <param name="speed">Speed of movement</param>
    /// <param name="movementType">Type of moevement</param>
    /// <returns>Handle to coroutine</returns>
    public override IEnumerator MoveToPoint(Point point, float speed, InstructionMovementType movementType)
    {
        if (movementType == InstructionMovementType.LINEAR)
        {
            yield return MoveToPointLinear(point.position, point.rotation, speed);
        }
        else
        {
            jointMoveProcess = StartCoroutine(MoveToPointJoint(point.jointAngles, speed));
            yield return jointMoveProcess;
        }
    }

    public IEnumerator MoveToPointLinear(Vector3 position, Vector3 rotation, float instructionSpeed)
    {

        movingToPoint = true;
        movementToPoint.Pause();
        movementToPoint = DOTween.Sequence();

        var movementModeSpeedModifier = data.MovementMode == MovementMode.T2 || data.MovementMode == MovementMode.AUTO ? 1f : 0.3f;
        var manualRobotSpeedModiffier = data.MovementMode == MovementMode.AUTO ? 1f : data.RobotSpeed;
        var robotSpeed = instructionSpeed * maxMovementSpeed * movementModeSpeedModifier * manualRobotSpeedModiffier;

        SwitchToJoint(false);
        movementToPoint.OnComplete(() => { movingToPoint = false; });
        movementToPoint.Append(robotEndPoint.transform.DOLocalMove(position, Vector3.Distance(robotEndPoint.transform.localPosition, position) / (robotSpeed)).SetEase(Ease.Linear));
        movementToPoint.Join(robotEndPoint.transform.DOLocalRotateQuaternion(Quaternion.Euler(rotation), Vector3.Distance(robotEndPoint.transform.localPosition, position) / (robotSpeed)).SetEase(Ease.Linear));
        yield return new WaitForSeconds(Vector3.Distance(robotEndPoint.transform.localPosition, position) / (robotSpeed));
        movingToPoint = false;

    }

    public IEnumerator MoveToPointJoint(List<Quaternion> targetAngles, float speed, Action onComplete = null)
    {
        movingToPoint = true;
        SwitchToJoint(true);
        targetJointAngles = targetAngles;
        float maxDifference = 0f;
        for (int i = 0; i < jointHierarchy.Count; i++)
        {
            float angle = Quaternion.Angle(targetJointAngles[i], jointHierarchy[i].localRotation);
            if (Mathf.Abs(angle) > maxDifference)
            {
                maxDifference = Mathf.Abs(angle);
            }
        }
        jointBaseRotations = new List<Quaternion>();
        foreach (Transform t in jointHierarchy)
        {
            jointBaseRotations.Add(t.localRotation);
        }

        var movementModeSpeedModifier = data.MovementMode == MovementMode.T2 || data.MovementMode == MovementMode.AUTO ? 1f : 0.3f;
        var manualRobotSpeedModiffier = data.MovementMode == MovementMode.AUTO ? 1f : data.RobotSpeed;
        var robotSpeed = speed * maxRotationSpeed * movementModeSpeedModifier * manualRobotSpeedModiffier;

        float time = maxDifference / robotSpeed;
        jointTraverseTime = time;
        jointMove = true;
        yield return new WaitForSeconds(jointTraverseTime);
        movingToPoint = false;

        if (onComplete != null)
        {
            onComplete();
        }
    }

    /// <summary>
    /// Stops robot in case of emergency
    /// </summary>
    public override void EmergencyStop()
    {
        KillMovement();
    }

    /// <summary>
    /// Stops robot movement without error
    /// </summary>
    public override void SoftStop()
    {
        KillMovement();
    }

    /// <summary>
    /// Immiedietly stops robot form moving
    /// </summary>
    public void KillMovement()
    {
        IsExecutingMoveTo = false;
        jointMove = false;
        RobotData.Instance.IsRunning = false;
        jointMoveTimer = 0;
        movingToPoint = false;
        movementToPoint.Kill();
        StopAllCoroutines();

        if (RobotData.Instance.MovementType == MovementType.Joint)
        {
            SwitchToJoint(true);
        }
        else
        {
            SwitchToJoint(false);
        }
    }

    /// <summary>
    /// Clears the movement sequence containing position and rotation changes
    /// </summary>
    public void ClearSequence()
    {
        movementToPoint = DOTween.Sequence();
    }

    /// <summary>
    /// Moves ending of robot smoothly to specified home position
    /// </summary>
    public override void MoveToHomePosition(bool instant)
    {
        KillMovement();
        StartCoroutine(MoveToPointJoint(homePoint.jointAngles, 1000f, () =>
        {
            SwitchToJoint(false);
            ControllersManager.Instance.GetController<RobotController>().CurrentRobot.SwitchMovementType(RobotData.Instance.MovementType);
        }));

        // jointMove = false;
        // movementToPoint.Kill();
        // ClearSequence();

        // if (instant)
        // {
        //     RobotEndPoint.transform.localPosition = homePoint.position;
        //     RobotEndPoint.transform.localRotation = Quaternion.Euler(homePoint.rotation);
        // }
        // else
        // {
        //     StartCoroutine(MoveToPoint(homePoint.position, homePoint.rotation, 3000f, InstructionMovementType.LINEAR));
        // }

    }

    /// <summary>
    /// Get home point for this robot
    /// </summary>
    /// <returns>Point structiore defining default position and rotation of pivot 6</returns>
    public override Point GetHomePoint()
    {
        return homePoint;
    }

    /// <summary>
    /// Switches movement type to specified value
    /// </summary>
    /// <param name="movementType">Movement type to switch to</param>
    public override void SwitchMovementType(MovementType movementType)
    {
        switch (movementType)
        {
            case MovementType.Base:
                SwitchToJoint(false);
                CalculateBaseEndPoint();
                break;
            case MovementType.Tool:
                SwitchToJoint(false);
                CalculateToolEndPoint(data);
                break;
            case MovementType.User:
                SwitchToJoint(false);
                CalculateUserEndPoint(data);
                break;
            case MovementType.Joint:
                SwitchToJoint(true);
                CalculateBaseEndPoint();
                break;
        }
    }

    /// <summary>
    /// Debug gizmos drawer
    /// </summary>
    private void OnDrawGizmos()
    {
        // Debug.DrawRay(baseFrame.transform.position, (robotEndPoint.transform.position - baseFrame.transform.position), Color.red);
    }


    public void SetEndPointOrientation(DirectionType orientation)
    {
        switch (orientation)
        {
            case DirectionType.FORWARD:
                RobotEndPoint.transform.forward = Vector3.forward;
                break;
            case DirectionType.BACK:
                RobotEndPoint.transform.forward = Vector3.back;
                break;
            case DirectionType.RIGHT:
                RobotEndPoint.transform.forward = Vector3.right;
                break;
            case DirectionType.LEFT:
                RobotEndPoint.transform.forward = Vector3.left;
                break;
            case DirectionType.UP:
                RobotEndPoint.transform.forward = Vector3.up;
                break;
            case DirectionType.DOWN:
                RobotEndPoint.transform.forward = Vector3.down;
                break;
        }
    }
}
