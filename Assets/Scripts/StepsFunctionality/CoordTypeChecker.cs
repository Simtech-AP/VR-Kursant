using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows checking of desired coordinate type
/// </summary>
public class CoordTypeChecker : StepEnabler
{
    /// <summary>
    /// Temporary variable to hold current movment type
    /// </summary>
    private MovementType type = default;

    [Header("Coord Type Checker")]
    /// <summary>
    /// List of possble desired types of movement
    /// </summary>
    [SerializeField]
    private List<MovementType> desiredTypes = new List<MovementType>();

    protected override void onUpdate()
    {
        CheckForRequiredCoordType();
    }

    /// <summary>
    /// Checks for desired type of movement
    /// </summary>
    public void CheckForRequiredCoordType()
    {
        type = RobotData.Instance.MovementType;
        if (desiredTypes.Contains(type))
        {
            Enabled = true;
        }
        else
        {
            Enabled = false;
        }
    }
}
