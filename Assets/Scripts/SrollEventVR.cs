using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

/// <summary>
/// Class allowing scrolling for error list
/// </summary>
public class SrollEventVR : MonoBehaviour
{
    /// <summary>
    /// Hand to interact with
    /// </summary>
    private Transform hand;
    /// <summary>
    /// Safeguard timer for jittery movements
    /// </summary>
    private float timer = 0f;
    /// <summary>
    /// Saved Y coordinate of hand
    /// </summary>
    private float prevY;
    /// <summary>
    /// New Y coordinate of hand
    /// </summary>
    private float nextY;
    /// <summary>
    /// Reference to visual scrollbar
    /// </summary>
    [SerializeField]
    private Scrollbar bar = default;

    /// <summary>
    /// Updates visual scrollbar according to hand position
    /// </summary>
    private void Update()
    {
        if (hand && VRInputController.UserClick.Stay)
        {
            timer += Time.deltaTime;
            if (timer > 0 && timer < 0.5)
            {
                prevY = hand.position.y;
            }
            if (timer > 1)
            {
                nextY = hand.position.y;
            }

            if (nextY != 0)
                bar.value += (nextY - prevY) / 75;
        }

        if (VRInputController.UserClick.Up)
        {
            prevY = nextY = timer = 0;
        }
    }

    /// <summary>
    /// Detects if hand has entered scrollbar position
    /// </summary>
    /// <param name="other">Collider that entered the scrollbar trigger</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<InteractGlove>())
            hand = other.transform;
        else
            hand = null;
    }

    /// <summary>
    /// Detects if hand is still in scrollbar 
    /// </summary>
    /// <param name="other">Collider that stays in the trigger</param>
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<InteractGlove>())
            hand = other.transform;
        else
            hand = null;
    }

    /// <summary>
    /// If any collider exited the trigger reset the hand reference
    /// </summary>
    /// <param name="other">Collider that exited the trigger</param>
    private void OnTriggerExit(Collider other)
    {
        hand = null;
    }
}
