using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Allows checking if object is on the floor or not
/// </summary>
public class CrateTouch : MonoBehaviour
{
    /// <summary>
    /// Is the crate touching floor?
    /// </summary>
    public bool touchingFloor = false;
    public bool touchingHand = false;

    /// <summary>
    /// Flag to prevent multiply audio starting
    /// </summary>
    private bool alreadyCollided = false;
    /// <summary>
    /// Event to start audio clip when box is dropped
    /// </summary>
    [SerializeField]
    public UnityEvent OnBoxCollision = default;

    /// <summary>
    /// Detect if on collision crate is touching floor
    /// </summary>
    /// <param name="collision">Collision object sent to method by engine</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name.Equals("Floor"))
        {
            if (touchingFloor == false)
            {
                OnBoxCollision.Invoke();
            }
            touchingFloor = true;
        }
        else if (alreadyCollided == false)
        {
            alreadyCollided = true;
            OnBoxCollision.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("vr_glove_right"))
        {
            touchingHand = true;
        }
    }

    /// <summary>
    /// Detect if on collision crate is touching floor
    /// </summary>
    /// <param name="collision">Collision object sent to method by engine</param>
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.name.Equals("Floor"))
        {
            touchingFloor = true;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name.Equals("vr_glove_right"))
        {
            touchingHand = true;
        }
    }

    /// <summary>
    /// Detect if crate has stopped touching floor
    /// </summary>
    /// <param name="collision">Collision object sent to method by engine</param>
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.name.Equals("Floor"))
        {
            touchingFloor = false;
        }
        else
        {
            alreadyCollided = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.Equals("vr_glove_right"))
        {
            touchingHand = false;
        }
    }
}
