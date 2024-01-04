using UnityEngine;

/// <summary>
/// Allows matrix robot to move using standarized methods in linear mode
/// </summary>
public class MoveMatrixRobot : MonoBehaviour
{
    /// <summary>
    /// Are we currently moving?
    /// </summary>
    private bool move = false;
    /// <summary>
    /// Are we currently rotating?
    /// </summary>
    private bool rotate = false;
    /// <summary>
    /// Reference to main matrix robot object
    /// </summary>
    [SerializeField]
    private MatrixRobotController matrixRobot = default;
    /// <summary>
    /// Direction of movement
    /// </summary>
    private MovementDirection moveDirection;
    /// <summary>
    /// Direction of rotation
    /// </summary>
    private MovementDirection rotationDirection;
    /// <summary>
    /// Speed of movement
    /// </summary>
    [SerializeField]
    private float moveSpeed = 0.2f;
    /// <summary>
    /// Speed of rotation
    /// </summary>
    [SerializeField]
    private float  rotationSpeed = 2f;
    /// <summary>
    /// Maximum movement speed
    /// </summary>
    private float maxMoveSpeed = 5f;
    /// <summary>
    /// Reference point to transform used to calculate ending of tool
    /// </summary>
    [SerializeField]
    private Transform referencePoint = default;
    /// <summary>
    /// Base point of a robot
    /// </summary>
    [SerializeField]
    private Transform basePoint = default;
    /// <summary>
    /// Calculated position of ending of tool
    /// </summary>
    [SerializeField]
    private Vector3 referencePosition = default;
    /// <summary>
    /// Calculated rotation of ending of tool
    /// </summary>
    [SerializeField]
    private Vector3 referenceRotation = default;
    /// <summary>
    /// X axis of tool
    /// </summary>
    [SerializeField]
    private Vector3 toolXVector = default;
    /// <summary>
    /// Y axis of tool
    /// </summary>
    [SerializeField]
    private Vector3 toolYVector = default;
    /// <summary>
    /// Z axis of tool
    /// </summary>
    [SerializeField]
    private Vector3 toolZVector = default;

    /// <summary>
    /// Moves and rotates matrix robot according to direction, speed and type of movement
    /// </summary>
    void Update()
    {
        if (move)
        {
            Vector3 step = Vector3.zero;
            switch (moveDirection)
            {
                case MovementDirection.Forward:
                    step = new Vector3(1, 0, 0);
                    break;
                case MovementDirection.Backward:
                    step = new Vector3(-1, 0, 0);
                    break;
                case MovementDirection.Right:
                    step = new Vector3(0, 1, 0);
                    break;
                case MovementDirection.Left:
                    step = new Vector3(0, -1, 0);
                    break;
                case MovementDirection.Up:
                    step = new Vector3(0, 0, 1);
                    break;
                case MovementDirection.Down:
                    step = new Vector3(0, 0, -1);
                    break;
            }
            if (RobotData.Instance.MovementType == MovementType.Tool)
            {
                var move = step.x * toolXVector + step.y * toolYVector + step.z * toolZVector;
                matrixRobot.currentTarget.position += move * moveSpeed * Time.deltaTime;
                RobotData.Instance.CurrentTarget = matrixRobot.currentTarget;
            }
            else
            {
                matrixRobot.currentTarget.position += step * moveSpeed * Time.deltaTime;
                RobotData.Instance.CurrentTarget = matrixRobot.currentTarget;
            }
        }
        if (rotate)
        {
            Vector3 step = Vector3.zero;
            switch (rotationDirection)
            {
                case MovementDirection.Forward:
                    step = new Vector3(1, 0, 0);
                    break;
                case MovementDirection.Backward:
                    step = new Vector3(-1, 0, 0);
                    break;
                case MovementDirection.Right:
                    step = new Vector3(0, 1, 0);
                    break;
                case MovementDirection.Left:
                    step = new Vector3(0, -1, 0);
                    break;
                case MovementDirection.Up:
                    step = new Vector3(0, 0, 1);
                    break;
                case MovementDirection.Down:
                    step = new Vector3(0, 0, -1);
                    break;
            }
            matrixRobot.currentTarget.rotation += step * rotationSpeed * Time.deltaTime;
            RobotData.Instance.CurrentTarget = matrixRobot.currentTarget;
        }
        referencePosition = referencePoint.position - basePoint.position;
        var temp = referencePosition;
        referencePosition.z = temp.y;
        referencePosition.y = temp.z;
        referencePosition.x = -temp.x + 0.035f;
        referenceRotation = referencePoint.eulerAngles - basePoint.eulerAngles;
    }

    /// <summary>
    /// Enables movement of robot
    /// </summary>
    /// <param name="direction">Defines direction the robot will move in</param>
    public void MoveRobot(MovementDirection direction)
    {
        move = true;
        moveDirection = direction;
    }

    /// <summary>
    /// Enables rotation of robot
    /// </summary>
    /// <param name="direction">Defines direction the robot will rotate in</param>
    public void RotateRobot(MovementDirection direction)
    {
        rotate = true;
        rotationDirection = direction;
    }

    /// <summary>
    /// Stops rotation of robot
    /// </summary>
    public void StopRotation()
    {
        rotate = false;
    }

    /// <summary>
    /// Stops movement of robot
    /// </summary>
    public void StopMovement()
    {
        move = false;
    }

    /// <summary>
    /// Chanes robot speed up and down
    /// </summary>
    /// <param name="up">Are we speeding up the robot?</param>
    public void ChangeRobotSpeed(bool up)
    {
        if (up)
        {
            if (moveSpeed < maxMoveSpeed)
            {
                moveSpeed += 0.2f;
                rotationSpeed += 2f;
            }
        }
        else
        {
            if (moveSpeed > 0.3f)
            {
                moveSpeed -= 0.2f;
                rotationSpeed -= 2f;
            }
        }
    }
}
