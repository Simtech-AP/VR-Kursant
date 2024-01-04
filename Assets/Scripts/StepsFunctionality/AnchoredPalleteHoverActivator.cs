using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows activation of pallete hover interaction and placing object (hoverObjects) hoverPosition transform from palleteBelowObject
/// </summary>
public class AnchoredPalleteHoverActivator : MonoBehaviour
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
    /// Pallete above which will the object be placed
    /// </summary>
    [SerializeField]
    private GameObject palleteBelowObject = default;
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
        Vector3 newPosition;
        for (int i = 0; i < hoverObjects.Count; ++i)
        {
            hoverObjects[i].gameObject.SetActive(true);
            hoverObjects[i].transform.SetParent(hoverAboveObject.transform);
            newPosition.x = palleteBelowObject.transform.position.x + hoverPosition.x;
            newPosition.y = hoverAboveObject.transform.position.y + hoverPosition.y;
            newPosition.z = palleteBelowObject.transform.position.z + hoverPosition.z;

            hoverObjects[i].transform.position = newPosition;
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
