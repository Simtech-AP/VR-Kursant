using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Class for interaction with interactable objects
/// </summary>
public class InteractGlove : MonoBehaviour
{
    /// <summary>
    /// current object to interact with
    /// </summary>
    private GameObject currentObject = default;

    /// <summary>
    /// Gets object which currently entered trigger
    /// </summary>
    /// <param name="other">Structure holding other collider data</param>
    private void OnTriggerStay(Collider other)
    {
        if (currentObject != other.gameObject)
        {
            if (currentObject && currentObject.GetComponents<Highlight>() != null)
                currentObject.GetComponents<Highlight>().ForEach(x => x.SetHighlight(false));
        }
        currentObject = other.gameObject;
        if (currentObject.GetComponents<Highlight>() != null)
            currentObject.GetComponents<Highlight>().ForEach(x => x.SetHighlight(true));
    }

    /// <summary>
    /// Gets object which currently exited trigger
    /// </summary>
    /// <param name="other">Structure holding other collider data</param>
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
    /// Continously check if trigger of controller is pressed
    /// </summary>
    private void Update()
    {
        if (!currentObject) return;
        if (VRInputController.UserClick.Down)
        {
            Interactable inter = currentObject.GetComponent<Interactable>();
            if (inter)
            {
                inter.Interact(gameObject);
            }
        }
    }

    /// <summary>
    /// Are we currently in object?
    /// </summary>
    /// <returns></returns>
    public bool IsInteracting()
    {
        return currentObject ? true : false;
    }

    /// <summary>
    /// Release currently interacted object
    /// </summary>
    public void ReleaseObject()
    {
        currentObject = null;
    }
}
