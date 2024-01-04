using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Parent class for grabable objects
/// </summary>
public class Grabable : MonoBehaviour
{
    /// <summary>
    /// Is the object grabbed?
    /// </summary>
    private bool isGrabbed;
    /// <summary>
    /// Joint of parent object grabbing this
    /// </summary>
    private Transform parentJoint;
    /// <summary>
    /// Is the object attached by joint?
    /// </summary>
    [SerializeField]
    private bool isJointBased = false;

    /// <summary>
    /// Flag to prevent multiply audio starting
    /// </summary>
    private bool alreadyPickedUp = false;
    /// <summary>
    /// Event to start audio clip when object is picked
    /// </summary>
    [SerializeField]
    public UnityEvent OnPickUpObject = default;

    /// <summary>
    /// Grabs object and attaches it to parent
    /// </summary>
    /// <param name="parent">Parent transform to attach to</param>
    public void Grab(Transform parent)
    {
        if (alreadyPickedUp == false)
        {
            alreadyPickedUp = true;
            OnPickUpObject.Invoke();
        }

        if (isJointBased)
        {
            parent.GetComponent<FixedJoint>().connectedBody = GetComponent<Rigidbody>();
            parentJoint = parent;
            isGrabbed = true;
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = true;
            transform.parent = parent;
        }
    }

    /// <summary>
    /// Detaches object from parent
    /// </summary>
    /// <param name="parent">Parent to release object from</param>
    public void GrabStop(Transform parent)
    {
        if (alreadyPickedUp == true)
            alreadyPickedUp = false;

        if (isJointBased)
        {
            parent.GetComponent<FixedJoint>().connectedBody = null;
            parentJoint = null;
            isGrabbed = false;
        }
        else if (GetComponent<Rigidbody>().isKinematic)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            transform.parent = null;
        }
    }

    /// <summary>
    /// Detaches object when it collides with something
    /// </summary>
    /// <param name="collision">Collision data</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (isGrabbed)
        {
            GrabStop(parentJoint);
        }
    }

    /// <summary>
    /// Gets Grabable component from object
    /// </summary>
    /// <returns> Grabable component </returns>
    public GameObject GetGrabableComponent()
    {
        return gameObject;
    }
}
