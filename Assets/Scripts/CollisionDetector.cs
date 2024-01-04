using System;
using UnityEngine;

/// <summary>
/// Class detecting forces for robot stopping 
/// </summary>
public class CollisionDetector : MonoBehaviour
{
    /// <summary>
    /// Edge force for stopping
    /// </summary>
    [SerializeField]
    private float collisionForce = 0f;
    /// <summary>
    /// Is detection enabled?
    /// </summary>
    private bool canDetectCollision = true;
    /// <summary>
    /// Public accessor for collision force
    /// </summary>
    public float CollisionForce { get => collisionForce; }
    /// <summary>
    /// Event called when robot detects collision
    /// </summary>
    public Action CollisionDetected;

    /// <summary>
    /// Method fired when collision is detected
    /// </summary>
    public void DetectedCollision(GameObject collider)
    {
        if (canDetectCollision)
        {
            CollisionDetected?.Invoke();
            canDetectCollision = false;
        }
    }

    /// <summary>
    /// Enable detection of collison force
    /// </summary>
    public void EnableDetection()
    {
        canDetectCollision = true;
    }

    /// <summary>
    /// Disable detection of collision force
    /// </summary>
    public void DisableDetection()
    {
        canDetectCollision = false;
    }
}
