using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

/// <summary>
/// class to handle the behavior of the bumpercap
/// </summary>
public class BumperCap : MonoBehaviour, IResetable
{
    /// <summary>
    /// starting position
    /// </summary>
    private Vector3 initialWorldPosition;
    /// <summary>
    /// starting rotation
    /// </summary>
    private Quaternion initialWorldRotation;
    /// <summary>
    /// starting parent
    /// </summary>
    private Transform initialParent;
    /// <summary>
    /// Head parent
    /// </summary>
    [SerializeField]
    private Transform headParent = default;
    /// <summary>
    /// Hand parent
    /// </summary>
    [SerializeField]
    private Transform handParent = default;
    /// <summary>
    /// Base rotation of bumperCap
    /// </summary>
    private Quaternion baseRot = Quaternion.Euler(-10, 47, 96f);
    /// <summary>
    /// Bumper cap offset position
    /// </summary>
    private Vector3 handOffsetPosition = new Vector3(-0.071f, 0.005f, 0.259f);
    /// <summary>
    /// Reference to collider of cap
    /// </summary>
    private Collider bumperCollider;

    /// <summary>
    /// Event called when cap is picked from head/door or placed on head/door
    /// </summary>
    [SerializeField]
    private UnityEvent onCapInteraction = default;

    /// <summary>
    /// save the initial transformation and set initial state
    /// </summary>
    public void Awake()
    {
        initialWorldPosition = transform.position;
        initialParent = transform.parent;
        initialWorldRotation = transform.rotation;
        CellStateData.bumpCapState = BumperCapState.OnDoor;
    }

    /// <summary>
    /// funtion to reset state of bumpercap
    /// </summary>
    public void PlaceOnDoor()
    {
        transform.SetParent(initialParent);
        transform.position = initialWorldPosition;
        transform.rotation = initialWorldRotation;
        GetComponent<MeshRenderer>().enabled = true;
        CellStateData.bumpCapState = BumperCapState.OnDoor;
        // onCapInteraction?.Invoke();
    }

    /// <summary>
    /// Detect and store objects in trigger
    /// </summary>
    /// <param name="other">Object that stays in Collider range</param>
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera") || other.CompareTag("Bumper"))
        {
            bumperCollider = other;
        }
    }

    /// <summary>
    /// Cleares detected object in trigger
    /// </summary>
    /// <param name="other">Collider of object intersecting with this</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera") || other.CompareTag("Bumper"))
        {
            bumperCollider = null;
        }
    }

    /// <summary>
    /// Place bumper cap on head
    /// </summary>
    private void PlaceOnHead()
    {
        transform.SetParent(headParent);
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = new Vector3(-2, 0, 0);
        if (FindObjectOfType<InteractGlove>())
        {
            FindObjectOfType<InteractGlove>().ReleaseObject();
        }
        CellStateData.bumpCapState = BumperCapState.OnHead;
        onCapInteraction?.Invoke();
    }

    /// <summary>
    /// Pickup bumperCap
    /// </summary>
    public void Pickup()
    {
        switch (CellStateData.bumpCapState)
        {
            case BumperCapState.OnHead:
                PickUpBumperCap();
                break;
            case BumperCapState.OnDoor:
                PickUpBumperCap();
                break;
        }
    }

    /// <summary>
    /// Pick up
    /// </summary>
    private void PickUpBumperCap()
    {
        transform.SetParent(handParent);
        transform.localPosition = handOffsetPosition;
        transform.localRotation = baseRot;

        if (CellStateData.bumpCapState == BumperCapState.OnHead)
        {
            onCapInteraction?.Invoke();
        }

        CellStateData.bumpCapState = BumperCapState.InHand;
    }

    /// <summary>
    /// Sets current state of cap
    /// </summary>
    /// <param name="state">State to set</param>
    public void SetState(int state)
    {
        switch (state)
        {
            case BumperCapState.OnDoor:
                PlaceOnDoor();
                break;
            case BumperCapState.InHand:
                PickUpBumperCap();
                break;
            case BumperCapState.OnHead:
                PlaceOnHead();
                break;
        }
    }

    public void ToggleState()
    {
        if (CellStateData.bumpCapState == BumperCapState.OnDoor)
        {
            PlaceOnHead();
        }
        else
        {
            PlaceOnDoor();
        }
    }

    /// <summary>
    /// Reset this object to default state
    /// </summary>
    void IResetable.Reset()
    {
        PlaceOnDoor();
    }

    /// <summary>
    /// Release cap from hand and set it to default position
    /// </summary>
    public void Release()
    {
        transform.SetParent(initialParent);
        transform.position = Vector3.Lerp(transform.position, initialWorldPosition, 2f);
        transform.rotation = Quaternion.Slerp(transform.rotation, initialWorldRotation, 2f);
        GetComponent<MeshRenderer>().enabled = true;
        CellStateData.bumpCapState = BumperCapState.OnDoor;
        // onCapInteraction?.Invoke();
    }

    /// <summary>
    /// Monitors inputs from controllers and runs methods
    /// </summary>
    private void Update()
    {
        if (VRInputController.UserClick.Up)
        {
            if (bumperCollider)
            {
                if (bumperCollider.CompareTag("MainCamera"))
                {
                    PlaceOnHead();
                }
                else if (bumperCollider.CompareTag("Bumper"))
                {
                    PlaceOnDoor();
                }
            }
            else
            {
                if (CellStateData.bumpCapState == BumperCapState.InHand)
                    Release();
            }
        }
    }
}
