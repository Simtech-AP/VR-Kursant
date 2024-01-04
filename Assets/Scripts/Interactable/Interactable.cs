using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Parent class for interactable objects
/// </summary>
public class Interactable : MonoBehaviour
{
    /// <summary>
    /// Event for started interaction listeners
    /// </summary>
    [SerializeField]
    private UnityEvent onInteracted = new UnityEvent();
    /// <summary>
    /// Event for ending interaction listeners
    /// </summary>
    [SerializeField]
    private UnityEvent onInteractionStop = new UnityEvent();

    /// <summary>
    /// Method called when stating interaction with object
    /// </summary>
    /// <param name="interactObject"></param>
    public virtual void Interact(GameObject interactObject) { onInteracted.Invoke(); }
    /// <summary>
    /// Method called when ending interaction with object
    /// </summary>
    /// <param name="interactObject"></param>
    public virtual void InteractionStop(GameObject interactObject) { onInteractionStop.Invoke(); }

    /// <summary>
    /// Main disabling event method
    /// </summary>
    public virtual void OnDisable() { }
}
