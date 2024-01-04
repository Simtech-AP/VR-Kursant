using UnityEngine;

/// <summary>
/// Allows checking of singularity has been reset
/// </summary>
public class AxisSingularityResetChecker : MonoBehaviour
{
    /// <summary>
    /// Reference to singularity checker
    /// </summary>
    [SerializeField]
    private AxisSingularityChecker axisAngleChecker = default;
    /// <summary>
    /// Reference to switch materials object
    /// </summary>
    [SerializeField]
    private MaterialSwitcher materialSwitcher = default;
    /// <summary>
    /// Reference to materials of robot object
    /// </summary>
    [SerializeField]
    private RobotMaterialContainer robotMaterials = default;

    /// <summary>
    /// Resets axis material to default
    /// </summary>
    public void ResetMaterial()
    {
        materialSwitcher.SwitchMaterial(axisAngleChecker.axis.transform, robotMaterials.Materials[FindObjectOfType<Robot>().GetType()]);
    }
}
