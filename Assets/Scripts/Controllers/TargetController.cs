using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows visualization of targets for trainee
/// </summary>
public class TargetController : MonoBehaviour
{
    /// <summary>
    /// List of position and rotation values for targets
    /// </summary>
    List<KeyValuePair<Vector3, Vector3>> targets = new List<KeyValuePair<Vector3, Vector3>>();
    /// <summary>
    /// Visual objects for each target
    /// </summary>
    List<GameObject> targetVisualizations = new List<GameObject>();
    /// <summary>
    /// Main target base for current target
    /// </summary>
    [SerializeField]
    private GameObject targetBase = default;
    /// <summary>
    /// Target prefab to instantiate when adding new point
    /// </summary>
    [SerializeField]
    private GameObject targetPrefab = default;

    /// <summary>
    /// Enable visibility of base target
    /// </summary>
    /// <param name="isOn">Should the target bnase be visible?</param>
    public void ShowTarget(bool isOn)
    {
        targetBase.SetActive(isOn);
    }

    /// <summary>
    /// Adds target visualization in place of base target
    /// </summary>
    public void AddTarget()
    {
        targets.Add(new KeyValuePair<Vector3, Vector3>(targetBase.transform.position, targetBase.transform.eulerAngles));
        targetVisualizations.Add(Instantiate(targetPrefab, targetBase.transform.position, targetBase.transform.rotation));
    }

    /// <summary>
    /// Removes target from scene
    /// </summary>
    /// <param name="index">Index of a target in targets list</param>
    public void RemoveTarget(int index)
    {
        Destroy(targetVisualizations[index]);
        targetVisualizations.RemoveAt(index);
        targets.RemoveAt(index);
    }

    /// <summary>
    /// Moves base target based on position delta
    /// </summary>
    /// <param name="positionDelta">Delta position to move the target by</param>
    public void MoveTarget(Vector3 positionDelta)
    {
        targetBase.transform.position += positionDelta;
    }

    /// <summary>
    /// Rotates base target based on rotation delta
    /// </summary>
    /// <param name="rotationDelta">Delta rotation to rotate the target by</param>
    public void MoveRotation(Vector3 rotationDelta)
    {
        targetBase.transform.Rotate(rotationDelta);
    }
}
