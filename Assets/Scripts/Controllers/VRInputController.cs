using UnityEngine;
using Valve.VR;

public class VRInputController : Controller
{
    public static VRControllerInteraction UserTouchpadTouch = new VRControllerInteraction();

    public static CombinedVRControllerInteraction UserClick = new CombinedVRControllerInteraction();

    private VRControllerInteraction TriggerClick = new VRControllerInteraction();
    private VRControllerInteraction TouchpadClick = new VRControllerInteraction();


    protected override void InitalizeControllers()
    {
        VRControllerInteraction[] combinedClicks = { TriggerClick, TouchpadClick };
        VRInputController.UserClick = new CombinedVRControllerInteraction(combinedClicks);
    }


    private void Update()
    {
        CheckForTouchpadTouch();
        CheckForUserClick();
    }

    private void CheckForTouchpadTouch()
    {
        VRInputController.UserTouchpadTouch.Stay = SteamVR_Input.GetState("TouchPadTouch", SteamVR_Input_Sources.RightHand);
    }

    private void CheckForUserClick()
    {
        TriggerClick.Stay = SteamVR_Input.GetState("InteractUI", SteamVR_Input_Sources.RightHand);
        TouchpadClick.Stay = SteamVR_Input.GetState("TeleportHold", SteamVR_Input_Sources.RightHand);
    }
}