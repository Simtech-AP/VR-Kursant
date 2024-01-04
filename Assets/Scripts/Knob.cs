using UnityEngine;
using Valve.VR;

/// <summary>
/// Class representing usable knob object on scene
/// </summary>
public class Knob : MonoBehaviour
{
    /// <summary>
    /// Reference to players glove
    /// </summary>
    private Transform glove;

    /// <summary>
    /// Detects if the player glove is in this objects trigger
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<InteractGlove>())
        {
            if (VRInputController.UserClick.Stay)
            {
                glove = other.GetComponent<InteractGlove>().transform.parent;
            }
        }
    }

    /// <summary>
    /// If anything has stoppped being in trigger reset glove object
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        glove = null;
    }

    /// <summary>
    /// If we are touching this object enable rotating it
    /// </summary>
    private void Update()
    {
        if (glove)
        {
            RotateKnob();
        }
    }

    /// <summary>
    /// Rotates this object according to glove position
    /// </summary>
    public void RotateKnob()
    {
        Vector3 rotation = Vector3.zero;
        rotation.y = -glove.transform.localRotation.eulerAngles.z;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(rotation), 5f * Time.deltaTime);
    }
}
