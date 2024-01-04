using System;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

/// <summary>
/// Class to handle the behavior of the padlock
/// </summary>
public class PadLock : MonoBehaviour, IResetable
{
    /// <summary>
    /// Starting position of padlock
    /// </summary>
    private Vector3 initialWorldPosition;
    /// <summary>
    /// Starting rotation of padlock
    /// </summary>
    private Quaternion initialWorldRotation;
    /// <summary>
    /// Starting parent of padlock
    /// </summary>
    private Transform initialParent;
    /// <summary>
    /// Reference to collider of padlock
    /// </summary>
    private Collider padLockCollider;

    /// <summary>
    /// Event called when padlock is put in a hand
    /// </summary>
    [SerializeField]
    public UnityEvent onPadlockPickUp = default;

    /// <summary>
    /// Event called when padlock is put on door lock or on padlock box
    /// </summary>
    [SerializeField]
    public UnityEvent onPadlockPutAway = default;

    /// <summary>
    /// Flag to check if padlock is already on door lock
    /// </summary>
    [HideInInspector]
    private bool locked = true;

    /// <summary>
    /// Sets starting variables to default
    /// Sets state to default state
    /// </summary>
    public void Awake()
    {
        initialWorldPosition = transform.position;
        initialParent = transform.parent;
        initialWorldRotation = transform.rotation;
        CellStateData.padLockState = PadLockState.InLockBox;
    }

    /// <summary>
    /// Detects if padlock is within the door lock 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Lock"))
        {
            padLockCollider = other;
        }
    }

    /// <summary>
    /// Resets connected collider if not colliding with anything
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        padLockCollider = null;
    }

    /// <summary>
    /// Detects if holding padlock and resets it to place of dropped
    /// </summary>
    private void Update()
    {
        if (VRInputController.UserClick.Up)
        {
            if (padLockCollider && !locked)
            {
                if (padLockCollider.CompareTag("Lock"))
                {
                    MoveToDoor();
                }
                else
                {
                    ResetPadLock();
                }
            }
            else if (!padLockCollider)
            {
                if (CellStateData.padLockState == PadLockState.InHand)
                    ResetPadLock();
            }
        }
    }

    public void TogglePadlockState()
    {
        if (CellStateData.padLockState == PadLockState.OnDoor)
        {
            MoveToLockbox();
        }
        else
        {
            MoveToDoor();
        }
    }

    /// <summary>
    /// Resets state of pad lock to initial state
    /// </summary>
    public void ResetPadLock()
    {
        transform.position = initialWorldPosition;
        transform.parent = initialParent;
        transform.rotation = initialWorldRotation;
        CellStateData.padLockState = PadLockState.InLockBox;
        if (!locked)        //prevents playing sound while reseting cell state
        {
            locked = true;
            onPadlockPutAway?.Invoke();
        }
    }

    /// <summary>
    /// Moves pad lock to hand
    /// </summary>
    public void MoveToHand()
    {
        GameObject hand = FindObjectOfType<InteractGlove>().gameObject;
        transform.parent = hand.transform;
        transform.localPosition = new Vector3(-.0101f, 0, 0.1163f);
        transform.localRotation = Quaternion.Euler(-90, 0, 90);
        CellStateData.padLockState = PadLockState.InHand;
        locked = false;
        onPadlockPickUp?.Invoke();
    }

    /// <summary>
    /// Moves pad lock to door
    /// </summary>
    public void MoveToDoor()
    {
        GameObject cellLock = FindObjectOfType<CellLock>().gameObject;
        transform.parent = cellLock.gameObject.transform;
        transform.localPosition = new Vector3(-0.0111f, 0.0274f, -0.0842f);
        transform.localRotation = Quaternion.Euler(0, 0, 90);
        CellStateData.padLockState = PadLockState.OnDoor;
        if (!locked)     //prevents playing sound while reseting cell state
        {
            locked = true;
            onPadlockPutAway?.Invoke();
        }
    }

    /// <summary>
    /// Sets state of padlock
    /// </summary>
    /// <param name="state">State to set</param>
    public void SetState(int state)
    {
        switch (state)
        {
            case 0:
                ResetPadLock();
                break;
            case 2:
                MoveToDoor();
                break;
        }
    }

    /// <summary>
    /// Moves pad lock to lock box
    /// </summary>
    public void MoveToLockbox()
    {
        ResetPadLock();
    }

    /// <summary>
    /// Allows interaction with padlock
    /// </summary>
    public void Interact()
    {
        switch (CellStateData.padLockState)
        {
            case PadLockState.InLockBox:
                MoveToHand();
                break;
            case PadLockState.OnDoor:
                MoveToHand();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Resets padlock to initial state
    /// </summary>
    void IResetable.Reset()
    {
        ResetPadLock();
    }
}
