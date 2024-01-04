using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for sides of a grasper, enables object detection
/// </summary>
public class GrasperSide : MonoBehaviour
{
    /// <summary>
    /// List of all objects in side collider
    /// </summary>
    public List<GameObject> objectsInGrasp = new List<GameObject>();

    /// <summary>
    /// Detect objects in side trigger
    /// </summary>
    /// <param name="other">Structure holding all collider data</param>
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<RobotInteractiveObject>())
        {
            if (!objectsInGrasp.Contains(other.gameObject))
            {
                objectsInGrasp.Add(other.gameObject);
            }
        }
    }

    /// <summary>
    /// Remove objects from side trigger
    /// </summary>
    /// <param name="other">Structure holding all collider data</param>
    private void OnTriggerExit(Collider other)
    {
        if (objectsInGrasp.Contains(other.gameObject))
        {
            objectsInGrasp.Remove(other.gameObject);
        }
    }
}
