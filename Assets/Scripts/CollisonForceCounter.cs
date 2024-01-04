using UnityEngine;

/// <summary>
/// Helper class for detecting forces on elements
/// </summary>
// TODO: This class doesn't do much, should it be refactored? Or just name changed?
public class CollisonForceCounter : MonoBehaviour
{
    /// <summary>
    /// Reference to collision detector
    /// </summary>
    [SerializeField]
    private CollisionDetector detector = default;

    /// <summary>
    /// Detect collision when force is more than threshold
    /// </summary>
    /// <param name="collision">Structure holding all info about collision</param>
    private void OnCollisionEnter(Collision collision)
    {
        detector.DetectedCollision(collision.gameObject);      
    }
}
