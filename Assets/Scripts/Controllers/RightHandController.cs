using UnityEngine;
using Valve.VR;

/// <summary>
/// Enables usgae of controller with pendant
/// Switches right hand intereactions off and on
/// </summary>
public class RightHandController : Controller
{
    /// <summary>
    /// Main right hand object
    /// </summary>
    [SerializeField]
    private GameObject physicalHand = default;
    /// <summary>
    /// Pendant object
    /// </summary>
    [SerializeField]
    private GameObject pendantHand = default;
    /// <summary>
    /// Reference to pickup object class
    /// </summary>
    [SerializeField]
    private PickUpItem pickUpItem = default;
    /// <summary>
    /// Reference to glove interactable class object
    /// </summary>
    [SerializeField]
    private InteractGlove interactGlove = default;
    /// <summary>
    /// Reference to transform of a hand
    /// </summary>
    [SerializeField]
    private Transform handTransform = default;

    /// <summary>
    /// When user is holding touchpad enable intearctions with right hand and hide hand on pendant
    /// </summary>
    void Update()
    {
        if (VRInputController.UserTouchpadTouch.Stay)
        {
            if (!physicalHand.activeInHierarchy)
            {
                physicalHand.SetActive(true);
                pendantHand.SetActive(false);
            }
        }
        else
        {
            if (physicalHand.activeInHierarchy && !pickUpItem.IsHoldingObject() && !interactGlove.IsInteracting())
            {
                physicalHand.SetActive(false);
                pendantHand.SetActive(true);
            }
        }

        // if (SteamVR_Input.GetStateDown("TeleportHold", SteamVR_Input_Sources.RightHand))
        // {
        //     handTransform.localScale = new Vector3(handTransform.localScale.x, handTransform.localScale.y * -1, handTransform.localScale.z);
        // }
    }
}
