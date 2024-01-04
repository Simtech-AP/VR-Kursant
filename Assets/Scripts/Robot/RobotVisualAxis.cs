using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aids in visual representation for robot axes
/// </summary>
public class RobotVisualAxis : MonoBehaviour
{
    /// <summary>
    /// List of all axes representations
    /// </summary>
    [SerializeField]
    private List<GameObject> axis = new List<GameObject>();

    /// <summary>
    /// Gets visual representatio object for specified axis
    /// </summary>
    /// <param name="index">Index of object to get</param>
    /// <returns>Game object of visual representation</returns>
    public GameObject GetAxis(int index)
    {
        return axis[index - 1];
    }
}
