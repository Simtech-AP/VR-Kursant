using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base class for buttons in scene
/// </summary>
public abstract class PhysicalButton : MonoBehaviour
{
    /// <summary>
    /// Event fired when this button was pressed
    /// </summary>
    [HideInInspector]
    public UnityEvent OnPressed = new UnityEvent();

    /// <summary>
    /// Event fired when this button was released
    /// </summary>
    [HideInInspector]
    public UnityEvent OnReleased = new UnityEvent();
}
