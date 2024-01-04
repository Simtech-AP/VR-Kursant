using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class for object travelling on conveyor belt
/// </summary>
[System.Serializable]
public class ConveyorObject
{
    /// <summary>
    /// Starting parent of object
    /// </summary>
    public Transform initialParent;
    /// <summary>
    /// Transform of an object on belt
    /// </summary>
    public Transform objectOnBelt;
    /// <summary>
    /// List of colliders of an object
    /// </summary>
    public List<Collider> collidersOnBelt;
    /// <summary>
    /// Main constructor
    /// </summary>
    /// <param name="parent">Parent transform to attach to</param>
    /// <param name="child">Children objects attached to this object</param>
    public ConveyorObject(Transform parent, Transform child)
    {
        initialParent = parent;
        objectOnBelt = child;
        collidersOnBelt = new List<Collider>();
    }
}

/// <summary>
/// Class holding logic for conveyor belt and pallet moving
/// </summary>
public class ConveyorBeltAttacher : MonoBehaviour
{
    /// <summary>
    /// Reference to main Conveyor object
    /// </summary>
    [SerializeField]
    private Conveyor conveyor = default;
    /// <summary>
    /// Reference to Pallet laying on conveyor belt
    /// </summary>
    [SerializeField]
    private List<ConveyorObject> objects = new List<ConveyorObject>();

    [SerializeField]
    private bool callOnAttachEvents = true;
    public bool CallOnAttachEvents
    {
        set
        {
            if (objects.Count != 0) onObjectAttached.Invoke();
            callOnAttachEvents = value;
        }
        get
        {
            return callOnAttachEvents;
        }
    }
    public UnityEvent onObjectAttached = default;

    /// <summary>
    /// Check if object has collided with conveyor belt
    /// </summary>
    /// <param name="collision">Collision data</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<RobotInteractiveObject>() == null || collision.gameObject.GetComponent<RobotInteractiveObject>().isInInteraction)
        {
            return;
        }

        ConveyorObject obj = objects.Find(x => x.objectOnBelt.Equals(collision.transform));
        if (obj == null)
        {
            obj = new ConveyorObject(collision.transform.parent, collision.transform);
            objects.Add(obj);
            collision.transform.SetParent(conveyor.ConveyorParent);
        }
        obj.collidersOnBelt.Add(collision.collider);

        if (callOnAttachEvents)
        {
            onObjectAttached.Invoke();
        }
    }

    /// <summary>
    /// Resets Pallet laying on conveyor
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<Grabable>() == null)
        {
            return;
        }

        ConveyorObject obj = objects.Find(x => x.objectOnBelt == collision.transform);
        if (obj != null)
        {
            obj.collidersOnBelt.Remove(collision.collider);
            if (obj.collidersOnBelt.Count <= 0)
            {
                obj.objectOnBelt.SetParent(obj.initialParent);
                objects.Remove(obj);
            }
        }
    }
}
