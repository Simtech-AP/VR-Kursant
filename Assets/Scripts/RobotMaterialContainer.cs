using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Robot materials container data class
/// </summary>
[CreateAssetMenu(fileName = "RobotMaterials")]
public class RobotMaterialContainer : ScriptableObject, ISerializationCallbackReceiver
{
    /// <summary>
    /// Yellow metal material for joint robot
    /// </summary>
    [SerializeField]
    private Material yellowMaterial = default;

    /// <summary>
    /// White metal material for matrix robot
    /// </summary>
    [SerializeField]
    private Material whiteMaterial = default;

    /// <summary>
    /// Data materials dictionary data holder
    /// </summary>
    public Dictionary<System.Type, Material> Materials = new Dictionary<System.Type, Material>();

    public void OnAfterDeserialize()
    {
        Materials.Clear();
        Materials.Add(typeof(MatrixRobot), whiteMaterial);
        Materials.Add(typeof(JointRobot), yellowMaterial);
    }

    public void OnBeforeSerialize()
    {
        Materials.Clear();
        Materials.Add(typeof(MatrixRobot), whiteMaterial);
        Materials.Add(typeof(JointRobot), yellowMaterial);
    }
}
