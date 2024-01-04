using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows activation of pallete hover interaction
/// </summary>
public class PalleteHoverActivator : MonoBehaviour
{
    /// <summary>
    /// List of objects portraying hover
    /// </summary>
    [SerializeField]
    private List<Hover> hoverObjects = new List<Hover>();
    /// <summary>
    /// Object to hover above palette
    /// </summary>
    [SerializeField]
    private GameObject hoverAboveObject = default;
    /// <summary>
    /// Position 
    /// </summary>
    [SerializeField]
    private Vector3 hoverPosition = Vector3.zero;
    /// <summary>
    /// Starting parent object of hover objects
    /// </summary>
    [SerializeField]
    private Transform initialParent = default;
    
    /// <summary>
    /// Activates all hover objects in a list
    /// </summary>
    public void ActivateObjects()
    {
        for (int i = 0; i < hoverObjects.Count; ++i)
        {
            hoverObjects[i].gameObject.SetActive(true);
            hoverObjects[i].transform.SetParent(hoverAboveObject.transform);
            hoverObjects[i].transform.localPosition = hoverPosition;
        }
    }

    /// <summary>
    /// Disables all objects in a list
    /// </summary>
    public void DeactivateObjects()
    {
        for (int i = 0; i < hoverObjects.Count; ++i)
        {
            hoverObjects[i].gameObject.SetActive(false);
            hoverObjects[i].transform.SetParent(initialParent);
            hoverObjects[i].transform.localPosition = Vector3.zero;
        }
    }
}
