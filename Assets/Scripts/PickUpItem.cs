using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Allows right hand to pick up items
/// </summary>
public class PickUpItem : MonoBehaviour
{
    /// <summary>
    /// Currently collided object
    /// </summary>
    private GameObject currentObject = default;

    /// <summary>
    /// If the other collider is a rigidbody allow picking it up
    /// </summary>
    /// <param name="other">Collider with which we triggered</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Grabable>())
        {
            currentObject = other.gameObject;
            if (currentObject.GetComponents<Highlight>() != null)
                currentObject.GetComponents<Highlight>().ForEach(x => x.SetHighlight(true));
        }
    }

    /// <summary>
    /// Remove current object if we stopped colliding with it
    /// </summary>
    /// <param name="other">Collider with which we triggered</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentObject)
        {
            if (currentObject.GetComponents<Highlight>() != null)
                currentObject.GetComponents<Highlight>().ForEach(x => x.SetHighlight(false));
            currentObject = null;
        }
    }

    /// <summary>
    /// Called when script disables
    /// </summary>
    private void OnDisable()
    {
        if (currentObject)
        {
            Grabable grabController = currentObject.GetComponent<Grabable>();
            if (grabController)
            {
                grabController.GrabStop(transform);
            }
        }
    }

    /// <summary>
    /// Every frame check input from controllers
    /// </summary>
    private void Update()
    {
        if (VRInputController.UserClick.Down)
        {
            GrabItem();
        }
        else if (VRInputController.UserClick.Up)
        {
            UnGrabItem();
        }
    }

    /// <summary>
    /// Grab stop
    /// </summary>
    private void UnGrabItem()
    {
        if (currentObject)
        {
            Grabable grabController = currentObject.GetComponent<Grabable>();
            if (grabController)
            {
                grabController.GrabStop(transform);
            }
        }
    }

    /// <summary>
    /// Called when object is grabbed
    /// </summary>
    private void GrabItem()
    {
        if (currentObject)
        {
            Grabable grabController = currentObject.GetComponent<Grabable>();
            if (grabController)
            {
                grabController.Grab(transform);
            }
        }
    }

    /// <summary>
    /// Checks if player is holding object
    /// </summary>
    /// <returns> Flag of holding object </returns>
    public bool IsHoldingObject()
    {
        return currentObject ? true : false;
    }
}
