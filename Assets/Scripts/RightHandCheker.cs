using UnityEngine;

/// <summary>
/// Part of a module 1
/// Allows for checking if we hold an item for long enough
/// </summary>
public class RightHandCheker : MonoBehaviour
{
    public RightHandUnlockController rightHandUnlock;

    /// <summary>
    /// Start checking right hand for collision and holding of object
    /// </summary>
    void OnEnable()
    {
        if (rightHandUnlock)
            rightHandUnlock.CheckRightHand();
    }

    /// <summary>
    /// When disabled reset couting of holding an object
    /// </summary>
    void OnDisable()
    {
        if (rightHandUnlock)
            rightHandUnlock.ResetCounterForRightHand();
    }
}
