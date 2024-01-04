using System.Collections;
using UnityEngine;

/// <summary>
/// Main template class 
/// </summary>
public abstract class Robot : MonoBehaviour
{
    /// <summary>
    /// Reference to current robot data
    /// </summary>
    public RobotData data;
    //TODO REFACTOR THIS LATER
    /// <summary>
    /// Have we detected any errors?
    /// </summary>
    public bool ErrorDetected = false;
    /// <summary>
    /// Are we currently moving position of 6th pivot?
    /// </summary>
    protected bool movingPosition;
    /// <summary>
    /// Are we currently moving rotation of 6th pivot?
    /// </summary>
    protected bool movingRotation;

    protected bool movingToPoint;
    protected bool isExecutingMoveTo;

    /// <summary>
    /// Are we currently moving 6th pivot in any way?
    /// </summary>
    public bool IsMoving { get { return movingPosition || movingRotation || movingToPoint; } }
    public bool IsMovingPosition { get => movingPosition; }
    public bool IsMovingRotation { get => movingRotation; }
    public bool IsMovedManually { get => movingPosition || movingRotation; }
    public bool IsExecutingInstructions { get => movingToPoint; }
    public bool IsExecutingMoveTo { get => isExecutingMoveTo; set => isExecutingMoveTo = value; }

    /// <summary>
    /// Enables linking as soon as start of the scene
    /// </summary>
    public virtual void Start()
    {

    }

    /// <summary>
    /// Moves position of 6th axis robot
    /// </summary>
    /// <param name="type">Type of movement</param>
    /// <param name="direction">Direction of movement</param>
    /// <param name="speed">Speed of movement</param>
    public abstract void MovePosition(MovementType type, MovementDirection direction, float speed);
    /// <summary>
    /// Moves rotation of 6th axis robot
    /// </summary>
    /// <param name="type">Type of rotation</param>
    /// <param name="direction">Direction of rotation</param>
    /// <param name="speed">Speed of rotation</param>
    public abstract void MoveRotation(MovementType type, MovementDirection direction, float speed);
    /// <summary>
    /// Moves ending of robot smoothly to specified home position
    /// </summary>
    public abstract void MoveToHomePosition(bool instant);
    /// <summary>
    /// Stops movement of position of 6th axis
    /// </summary>
    public abstract void StopPositionMovement();
    /// <summary>
    /// Stops movement of rotation of 6th axis
    /// </summary>
    public abstract void StopRotationMovement();
    /// <summary>
    /// Coroutine allowing the robot to move smoothly to next target
    /// </summary>
    /// <param name="position">Position to move to</param>
    /// <param name="rotation">Rotation to rotate to</param>
    /// <param name="speed">Speed with which robot will translate</param>
    /// <param name="movementType"></param>
    /// <returns>Handle to coroutine</returns>
    public abstract IEnumerator MoveToPoint(Point point, float speed, InstructionMovementType movementType);
    /// <summary>
    /// Stops movement of robot with an error
    /// </summary>
    public abstract void EmergencyStop();
    /// <summary>
    /// Stops movement of a robot
    /// </summary>
    public abstract void SoftStop();
    /// <summary>
    /// Gets current point of 6th axis
    /// </summary>
    /// <returns>Point structure with current position and rotation of 6th axis</returns>
    public abstract Point GetCurrentPoint();
    /// <summary>
    /// Gets home point of a robot in linear coordinates
    /// </summary>
    /// <returns>Point structure with home position and rotation of 6th axis</returns>
    public abstract Point GetHomePoint();
    /// <summary>
    /// Swithces movement type to specified type
    /// </summary>
    /// <param name="movementType">Movement type to switch to</param>
    public abstract void SwitchMovementType(MovementType movementType);
}
