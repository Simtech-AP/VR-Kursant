using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows enabling objects
/// </summary>
public class ObjectsActivator : MonoBehaviour
{
    /// <summary>
    /// List of objects to activate
    /// </summary>
    [SerializeField]
    private List<GameObject> objectsToActivate = default;

    /// <summary>
    /// Activates objects
    /// </summary>
    private void Awake()
    {
        ActivateObjects();
    }

    /// <summary>
    /// Enables all objects in a list
    /// </summary>
    public void ActivateObjects()
    {
        for (int i = 0; i < objectsToActivate.Count; ++i)
        {
            objectsToActivate[i].SetActive(true);
        }
    }

    /// <summary>
    /// Disables all objects in a list
    /// </summary>
    public void DeactivateObjects()
    {
        for (int i = 0; i < objectsToActivate.Count; ++i)
        {
            objectsToActivate[i].SetActive(false);
        }
    }
}
