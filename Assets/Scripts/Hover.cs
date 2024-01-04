using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base class for hover events for objects
/// </summary>
public abstract class Hover : MonoBehaviour
{
    /// <summary>
    /// Time neede for timed hover event
    /// </summary>
    protected const float timeMax = 3f;
    /// <summary>
    /// Time hand has hovered in object
    /// </summary>
    protected float timer = 0f;
    /// <summary>
    /// Event run when hovered for specified time
    /// </summary>
    public UnityEvent onHoveredForTime;
    /// <summary>
    /// Event run when exited
    /// </summary>
    public UnityEvent onExit;
    /// <summary>
    /// Reference to mesh renderer of object
    /// </summary>
    protected MeshRenderer meshRenderer;

    /// <summary>
    /// Sets up references
    /// </summary>
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Checks if hand has hovered
    /// </summary>
    /// <param name="other">Structure containing other collider data</param>
    protected virtual void OnTriggerStay(Collider other)
    {
    }

    /// <summary>
    /// Checks if hand has ended hover
    /// </summary>
    /// <param name="other">Structure containing other collider data</param>
    protected virtual void OnTriggerExit(Collider other)
    {
    }

    /// <summary>
    /// Checks if object with componenet of T type has hovered over for specified time
    /// </summary>
    /// <typeparam name="T">Template type for object</typeparam>
    /// <param name="other">Collider data</param>
    protected void TriggerStay<T>(Collider other)
        where T : MonoBehaviour
    {
        if (other.GetComponent<T>())
        {
            timer += Time.fixedDeltaTime;
        }
        if (timer >= timeMax)
        {
            onHoveredForTime.Invoke();
        }
    }

    /// <summary>
    /// Checks if object with component of T type has ended hovering over this object
    /// </summary>
    /// <typeparam name="T">Template type for object</typeparam>
    /// <param name="other">Collider data</param>
    protected void TriggerExit<T>(Collider other)
        where T: MonoBehaviour
    {
        if (timer >= timeMax)
        {
            timer = 0f;
        }
        else
        {
            if (other.GetComponent<T>())
            {
                onExit.Invoke();
                timer = 0f;
            }
        }
    }
}
