using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for holding and maintaining cell state
/// </summary>
public class CellStateData : MonoBehaviour
{
    /// <summary>
    /// State of bumper cap
    /// </summary>
    public static int bumpCapState = BumperCapState.OnDoor;
    /// <summary>
    /// State of padlock
    /// </summary>
    public static int padLockState = PadLockState.InLockBox;
    /// <summary>
    /// State of cell entrance
    /// </summary>
    public static int cellEntranceState = CellEntranceState.Closed;
    /// <summary>
    /// State of player location
    /// </summary>
    public static int playerLocation = PlayerLocation.Outside;

    public static bool IsHandFree { get { return bumpCapState != BumperCapState.InHand && padLockState != PadLockState.InHand; } }
    /// <summary>
    /// List holding all of EStop states
    /// </summary>
    public static List<int> EStopStates = new List<int>() { };
    /// <summary>
    /// List of all resetable objects on scene
    /// </summary>
    [SerializeField]
    private List<MonoBehaviour> resetables = new List<MonoBehaviour>() { };

    /// <summary>
    /// Sets up list of EStops with objects from scene
    /// </summary>
    private void Awake()
    {
        int index = 0;
        var estops = InteractablesManager.Instance.GetAllInteractableBehaviour<EStop>();
        foreach (EStop estop in estops)
        {
            estop.EStopIndex = index;
            EStopStates.Add(EStopButtonState.Released);
            index++;
        }
    }

    /// <summary>
    /// Resets all IResetable objects to default state
    /// </summary>
    public void ResetCellState()
    {
        for (int i = 0; i < resetables.Count; ++i)
        {
            (resetables[i] as IResetable).Reset();
        }
    }
}
