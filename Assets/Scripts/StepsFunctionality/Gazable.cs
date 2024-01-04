using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base class for item that can be gazed upon
/// </summary>
public class Gazable : MonoBehaviour
{
    /// <summary>
    /// Event run when gazed for a specified time
    /// </summary>
    public UnityEvent finishedGazing = new UnityEvent();
    /// <summary>
    /// Event run when started looking at object
    /// </summary>
    public UnityEvent startedGazing = new UnityEvent();
    /// <summary>
    /// Event tun when looked away from object
    /// </summary>
    public UnityEvent stoppedGazing = new UnityEvent();
}
