using UnityEngine;

/// <summary>
/// Allows stopping robot with raising errors
/// </summary>
public class RobotStop : MonoBehaviour
{
    /// <summary>
    /// Reference to mode key on scene
    /// </summary>
    private ModeKey modeKey;
    /// <summary>
    /// Reference to cell entrance on scene
    /// </summary>
    private CellEntrance cellEntrance;

    /// <summary>
    /// Sets up references
    /// </summary>
    private void Awake()
    {
        modeKey = InteractablesManager.Instance.GetInteractableBehaviour<ModeKey>();
        cellEntrance = InteractablesManager.Instance.GetInteractableBehaviour<CellEntrance>();
    }

    /// <summary>
    /// Stops robot and raises error
    /// </summary>
    public void StopRobot()
    {
        modeKey.SetMode(MovementMode.T1);
        cellEntrance.Open();
        ErrorRequester.RaiseError("S-1002");
    }
}
