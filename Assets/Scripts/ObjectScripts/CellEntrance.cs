using UnityEngine;
using DG.Tweening;
using Valve.VR;
using System;
using UnityEngine.Events;

/// <summary>
/// Class to handle the behavior of the cell entrance
/// </summary>
public class CellEntrance : MonoBehaviour, IResetable
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
    private readonly Quaternion defalutClosedDoorRotation = Quaternion.Euler(-90, 0, 0);
    /// <summary>
    /// Default rotation for opened door
    /// </summary>
    private readonly Quaternion defaultOpenedDoorRotation = Quaternion.Euler(-90, 0, -3);
    /// <summary>
    /// Maximum rotation for opened door
    /// </summary>
    private readonly Quaternion maxOpenedDoorRotation = Quaternion.Euler(-90, 0, -50);
    /// <summary>
    /// Current door angle
    /// </summary>
    public float angle;
    /// <summary>
    /// X rotation vale of unpressed door handle
    /// </summary>
    [SerializeField]
    private float releasedHandleXRotation = default;
    /// <summary>
    /// X rotation vale of pressed door handle
    /// </summary>
    [SerializeField]
    private float pressedHandleXRotation = default;
    /// <summary>
    /// Time for DOTween to animate rotation of the door handle
    /// </summary>
    [SerializeField]
    private float handleRotateTime = default;
    /// <summary>
    /// Flag for opening door
    /// </summary>
    private bool isMoving = false;
    /// <summary>
    /// Flag for hand inside handle trigger
    /// </summary>
    private bool isHandOnHandle = false;
    /// <summary>
    /// Right Hand position
    /// </summary>
    [SerializeField]
    private Transform rightHand = default;
    /// <summary>
    /// Reference to animator object for door locking
    /// </summary>
    [SerializeField]
    private Animator animator = default;
    /// <summary>
    /// Reference to transform of cell door handle
    /// </summary>
    [SerializeField]
    private Transform cellHandle = default;

    /// <summary>
    /// Reference to transform of the first part of the door handle
    /// </summary>
    [SerializeField]
    private Transform firstDoorHandle = default;

    /// <summary>
    /// Reference to transform of the second part of the door handle
    /// </summary>
    [SerializeField]
    private Transform secondDoorHandle = default;

    /// <summary>
    /// Action to run when door to cell has been opened
    /// </summary>
    public static Action CellOpened;
    /// <summary>
    /// Action to run when door to cell has been closed
    /// </summary>
    public static Action CellClosed;
    /// <summary>
    /// Reference to collider of hand using door
    /// </summary>
    private Collider hand;
    /// <summary>
    /// Current distance of hand from door handle
    /// </summary>
    float distance = 1f;

    [SerializeField]
    private UnityEvent onDoorLatchSnap;


    [SerializeField]
    private UnityEvent onDoorLatchOpen;

    /// <summary>
    /// Check if hand is in handle trigger
    /// </summary>
    /// <param name="other"> hand collider </param>
    private void OnTriggerStay(Collider other)
    {
        if (CellStateData.padLockState == PadLockState.OnDoor && CellStateData.cellEntranceState == CellEntranceState.Closed || !CellStateData.IsHandFree) return;

        if (other.GetComponent<InteractGlove>())
        {
            if (VRInputController.UserClick.Down)
            {
                if (transform.rotation == defalutClosedDoorRotation)
                {
                    CellOpened?.Invoke();
                    animator.SetTrigger("OpenRygiel");
                    onDoorLatchOpen.Invoke();
                }
                isHandOnHandle = true;
                pressHandle();
                StartMoving();
                hand = other;
            }
        }
    }

    /// <summary>
    /// Check if hand exited handle trigger
    /// </summary>
    /// <param name="other"> hand collider </param>
    private void OnTriggerExit(Collider other)
    {
        if (CellStateData.padLockState == PadLockState.OnDoor && CellStateData.cellEntranceState == CellEntranceState.Closed) return;
    }

    /// <summary>
    /// function to open door
    /// </summary>
    public void StartMoving()
    {
        if (CellStateData.padLockState == PadLockState.OnDoor && CellStateData.cellEntranceState == CellEntranceState.Closed) return;

        isMoving = true;
        CellStateData.cellEntranceState = CellEntranceState.Moving;
    }

    /// <summary>
    /// funcion to close door
    /// </summary>
    public void CloseDoor()
    {
        isMoving = false;
        if (CellStateData.padLockState == PadLockState.OnDoor)
        {
            RepealDoor();
        }
        else if (CellStateData.padLockState != PadLockState.OnDoor)
        {
            SnapDoor();
        }
    }

    /// <summary>
    /// Open door if padlock is on lock
    /// </summary>
    private void RepealDoor()
    {
        CellStateData.cellEntranceState = CellEntranceState.Repealed;
        if (transform.rotation.y > -0.1f && transform.rotation.y < 0.1f)
        {
            transform.DOLocalRotateQuaternion(defaultOpenedDoorRotation, 1f);
        }
    }

    /// <summary>
    /// Close door logic
    /// </summary>
    public void SnapDoor()
    {
        if (transform.rotation.y > -0.1f && transform.rotation.y < 0.1f)
        {
            CellClosed?.Invoke();
            transform.DOLocalRotateQuaternion(defalutClosedDoorRotation, 1f);
            if (CellStateData.cellEntranceState != CellEntranceState.Closed)
            {
                animator.SetTrigger("CloseRygiel");
                onDoorLatchSnap.Invoke();
            }
            CellStateData.cellEntranceState = CellEntranceState.Closed;
        }
    }

    /// <summary>
    /// Method for forcing the complete closing of the cell entrance
    /// </summary>
    public void ForceClose()
    {
        isMoving = false;
        CellStateData.cellEntranceState = CellEntranceState.Closed;
        animator.SetTrigger("CloseRygiel");
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
                releaseHandle();
            }
        }

        angle = Vector3.Angle(rightHand.position - transform.position, transform.position);
        if (isMoving && isHandOnHandle && angle > maxDoorAngle)
        {
            Vector3 directionToRotate = rightHand.position - transform.position;
            directionToRotate.y = 0;
            if (directionToRotate.x < 0.05)
            {
                RotateDirection(directionToRotate);
            }
        }
    }

    /// <summary>
    /// Reads and sets distance variable for interaction with door
    /// </summary>
    private void FixedUpdate()
    {
        distance = (cellHandle.position - rightHand.position).sqrMagnitude;
    }

    /// <summary>
    /// Rotates model door
    /// </summary>
    /// <param name="directionToRotate"> direction where door should be rotated </param>
    private void RotateDirection(Vector3 directionToRotate)
    {
        Quaternion rotationDirection = Quaternion.LookRotation(directionToRotate, transform.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, (Quaternion.Euler(-90, rotationDirection.eulerAngles.y, rotationDirection.eulerAngles.z)), distance * ROTATION_DURATION);
    }

    /// <summary>
    /// Interacts with door and runs methods according to state
    /// </summary>
    public void Interact()
    {
        if (CellStateData.cellEntranceState == CellEntranceState.Moving)
        {
            CloseDoor();
        }
        else if (CellStateData.cellEntranceState == CellEntranceState.Stopped
            || CellStateData.cellEntranceState == CellEntranceState.Closed
            || CellStateData.cellEntranceState == CellEntranceState.Repealed)
        {
            StartMoving();
        }
    }

    /// <summary>
    /// Sets state of door
    /// </summary>
    /// <param name="state">State to set</param>
    public void SetState(int state)
    {
        switch (state)
        {
            case 0:
                SnapDoor();
                break;
            case 2:
                Open();
                break;
        }
    }

    public void ToggleEntranceState()
    {
        if (CellStateData.cellEntranceState == CellEntranceState.Closed)
        {
            if (CellStateData.padLockState != PadLockState.OnDoor)
            {
                Open();
            }
        }
        else
        {
            Close();
        }
    }

    /// <summary>
    /// Open cell and set state
    /// </summary>
    [ContextMenu("OPEN")]
    public void Open()
    {
        CellStateData.cellEntranceState = CellEntranceState.Stopped;
        transform.DOLocalRotateQuaternion(maxOpenedDoorRotation, 1f);
        CellOpened?.Invoke();
    }

    public void Close()
    {
        if (CellStateData.padLockState == PadLockState.OnDoor)
        {
            RepealDoor();
        }
        else
        {
            CellStateData.cellEntranceState = CellEntranceState.Closed;
            transform.DOLocalRotateQuaternion(defalutClosedDoorRotation, 1f);
            CellClosed?.Invoke();
        }
    }

    /// <summary>
    /// Reset door to default state
    /// </summary>
    [ContextMenu("CLOSE")]
    void IResetable.Reset()
    {
        CellClosed?.Invoke();
        transform.DOLocalRotateQuaternion(defalutClosedDoorRotation, 1f);
        if (CellStateData.cellEntranceState != CellEntranceState.Closed)
            animator.SetTrigger("CloseRygiel");
        CellStateData.cellEntranceState = CellEntranceState.Closed;
    }

    /// <summary>
    /// Starts pressing both door handles "animation"
    /// </summary>
    private void pressHandle()
    {
        firstDoorHandle.DOLocalRotate(new Vector3(pressedHandleXRotation, firstDoorHandle.transform.rotation.y, firstDoorHandle.transform.rotation.z), handleRotateTime);
        secondDoorHandle.DOLocalRotate(new Vector3(pressedHandleXRotation, secondDoorHandle.transform.rotation.y, secondDoorHandle.transform.rotation.z), handleRotateTime);
    }

    /// <summary>
    /// Starts releasing both door handles "animation"
    /// </summary>
    private void releaseHandle()
    {
        firstDoorHandle.DOLocalRotate(new Vector3(releasedHandleXRotation, firstDoorHandle.transform.rotation.y, firstDoorHandle.transform.rotation.z), handleRotateTime);
        secondDoorHandle.DOLocalRotate(new Vector3(releasedHandleXRotation, secondDoorHandle.transform.rotation.y, secondDoorHandle.transform.rotation.z), handleRotateTime);
    }
}
