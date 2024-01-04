using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class helping in visualizing robot axes, joints and frames
/// </summary>
public class RobotVisualAid : MonoBehaviour
{
    /// <summary>
    /// Helpers for each joint
    /// </summary>
    [SerializeField]
    private List<GameObject> jointAides = new List<GameObject>();
    /// <summary>
    /// Helper for base reference frame
    /// </summary>
    [SerializeField]
    private GameObject baseAid = default;
    /// <summary>
    /// Helper for tool reference frame
    /// </summary>
    [SerializeField]
    private GameObject toolAid = default;

    /// <summary>
    /// Shows helper for specified joint
    /// </summary>
    /// <param name="joint">Joint number according to robot specification</param>
    public void ShowJointAid(int joint)
    {
        foreach (GameObject aid in jointAides)
        {
            aid.SetActive(false);
        }
        jointAides[joint - 1].SetActive(true);
    }

    /// <summary>
    /// Allows getting joint helper object
    /// </summary>
    /// <param name="joint">Joint number according to robot specifications</param>
    /// <returns>Helper object</returns>
    public GameObject GetJointAid(int joint)
    {
        return jointAides[joint - 1];
    }

    /// <summary>
    /// Allows getting tool helper object
    /// </summary>
    /// <returns>Helper object</returns>
    public GameObject GetToolAid()
    {
        return toolAid;
    }

    /// <summary>
    /// Allows getting base helper object
    /// </summary>
    /// <returns>Helper object</returns>
    public GameObject GetBaseAid()
    {
        return baseAid;
    }

    /// <summary>
    /// Shows all joint helpers
    /// </summary>
    public void ShowAllJointAides()
    {
        foreach (GameObject aid in jointAides)
        {
            aid.SetActive(true);
        }
    }

    /// <summary>
    /// Hides all helpers including joint, base and tool helpers
    /// </summary>
    public void HideAllAides()
    {
        foreach (GameObject aid in jointAides)
        {
            if(aid)
                aid.SetActive(false);
        }
        baseAid.SetActive(false);
        toolAid.SetActive(false);
    }

    /// <summary>
    /// Hides all helpers including joint, base and tool helpers
    /// </summary>
    public void HideAid(int index)
    {
        jointAides[index - 1].SetActive(false);
    }

    /// <summary>
    /// Shows base frame helper
    /// </summary>
    public void ShowBaseAid()
    {
        baseAid.SetActive(true);
    }

    /// <summary>
    /// Shows tool frame helper
    /// </summary>
    public void ShowToolAid()
    {
        toolAid.SetActive(true);
    }
}
