using DG.Tweening;
using UnityEngine;
using Valve.VR;

public class Wardrobe : MonoBehaviour
{
    /// <summary>
    /// Offset not to move cell
    /// </summary>
    private const float NOT_MOVING_DISTANCE = 0.025f;
    /// <summary>
    /// Door rotating duration time
    /// </summary>
    private const float ROTATION_DURATION = 5f;
    /// <summary>
    /// Max door rotation angle
    /// </summary>
    [SerializeField]
    private float maxDoorAngle = 50f;
    /// <summary>
    /// Default rotation for closed door
    /// </summary>
    private readonly Quaternion defalutClosedDoorRotation = Quaternion.Euler(0, -100.5f, 0);
    /// <summary>
    /// Maximum rotation for opened door
    /// </summary>
    private readonly Quaternion maxOpenedDoorRotation = Quaternion.Euler(0, 0, 0);
    /// <summary>
    /// Current door angle
    /// </summary>
    public float angle;
    /// <summary>
    /// Flag for opening door
    /// </summary>
    public bool isMoving = false;
    /// <summary>
    /// Flag for hand inside handle trigger
    /// </summary>
    public bool isHandOnHandle = false;
    /// <summary>
    /// Right Hand position
    /// </summary>
    [SerializeField]
    private Transform rightHand = default;
    /// <summary>
    /// Reference to transform of cell door handle
    /// </summary>
    [SerializeField]
    private Transform handle = default;
    /// <summary>
    /// Reference to collider of hand using door
    /// </summary>
    private Collider hand;
    /// <summary>
    /// Current distance of hand from door handle
    /// </summary>
    float distance = 1f;
    /// <summary>
    /// Reference to the door transform
    /// </summary>
    [SerializeField]
    private Transform door;
    /// <summary>
    /// Calculated vector to rotate to
    /// </summary>
    private Vector3 directionToRotate;
    /// <summary>
    /// Temporary variable for storing Y coordinate
    /// </summary>
    private float prevY;

    /// <summary>
    /// Sets the temporary variable to door coordinate Y
    /// </summary>
    private void Awake()
    {
        prevY = door.localEulerAngles.y;
    }

    /// <summary>
    /// Check if hand is in handle trigger
    /// </summary>
    /// <param name="other"> hand collider </param>
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<InteractGlove>())
        {
            if (VRInputController.UserClick.Down)
            {
                isHandOnHandle = true;
                StartMoving();
                hand = other;
            }
        }
    }

    /// <summary>
    /// function to open door
    /// </summary>
    public void StartMoving()
    {
        isMoving = true;
    }

    /// <summary>
    /// funcion to close door
    /// </summary>
    public void CloseDoor()
    {
        isMoving = false;
        SnapDoor();
    }

    /// <summary>
    /// Close door logic
    /// </summary>
    private void SnapDoor()
    {
        if (door.rotation.y > -0.1f && door.rotation.y < 0.1f)
        {
            door.DOLocalRotateQuaternion(defalutClosedDoorRotation, 1f);
        }
    }

    /// <summary>
    /// function for forcing the complete closing of the cell entrance
    /// </summary>
    public void ForceClose()
    {
        isMoving = false;
    }

    /// <summary>
    /// Open door logic
    /// </summary>
    private void Update()
    {
        if (VRInputController.UserClick.Up)
        {
            if (hand)
            {
                isHandOnHandle = false;
                CloseDoor();
                hand = null;
            }
        }

        if (distance > NOT_MOVING_DISTANCE)
        {
            angle = Vector3.Angle(rightHand.position, door.position);
            if (isMoving && isHandOnHandle && angle > maxDoorAngle)
            {
                directionToRotate = rightHand.position - door.position;
                if (directionToRotate.x < 0.05)
                {
                    RotateDirection(directionToRotate);
                }
            }
        }
    }

    /// <summary>
    /// Reads and sets distance variable for interaction with door
    /// </summary>
    private void FixedUpdate()
    {
        distance = (handle.position - rightHand.position).sqrMagnitude;
    }

    /// <summary>
    /// Rotates model door
    /// </summary>
    /// <param name="directionToRotate"> direction where door should be rotated </param>
    private void RotateDirection(Vector3 directionToRotate)
    {
        Quaternion rotationDirection = Quaternion.LookRotation(directionToRotate, door.up);
        door.rotation = Quaternion.Slerp(door.rotation, (Quaternion.Euler(0, prevY + rotationDirection.eulerAngles.y, 0)), ROTATION_DURATION * distance);
    }

    /// <summary>
    /// Interacts with door and runs methods according to state
    /// </summary>
    public void Interact()
    {
        if (isMoving)
        {
            CloseDoor();
        }
        else
        {
            StartMoving();
        }
    }

    /// <summary>
    /// Open cell and set state
    /// </summary>
    [ContextMenu("PRESS")]
    public void Open()
    {
        door.DOLocalRotateQuaternion(maxOpenedDoorRotation, 1f);
    }

    /// <summary>
    /// Debug method for testing 
    /// </summary>
    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, directionToRotate);
    }
}
