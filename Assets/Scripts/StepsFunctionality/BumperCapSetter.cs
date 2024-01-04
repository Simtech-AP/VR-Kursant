using UnityEngine;

/// <summary>
/// Sets up bumper caps state
/// </summary>
public class BumperCapSetter : MonoBehaviour
{
    /// <summary>
    /// Reference to bumper cap
    /// </summary>
    private BumperCap bumperCap;

    /// <summary>
    /// Sets up references
    /// </summary>
    private void OnEnable()
    {
        bumperCap = InteractablesManager.Instance.GetInteractableBehaviour<BumperCap>();
    }

    /// <summary>
    /// Changes bumper cap state to be on user head
    /// </summary>
    public void PlaceOnHead()
    {
        bumperCap.SetState(BumperCapState.OnHead);
    }

    /// <summary>
    /// Changes bumper cap state to be held in hand
    /// </summary>
    public void PlaceInHand()
    {
        bumperCap.SetState(BumperCapState.InHand);
    }

    /// <summary>
    /// Changes bumper cap state to be hanged on door
    /// </summary>
    public void PlaceOnDoor()
    {
        bumperCap.SetState(BumperCapState.OnDoor);
    }
}
