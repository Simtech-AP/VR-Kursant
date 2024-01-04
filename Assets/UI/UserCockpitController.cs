using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum CockpitPosition
{
    INSIDE = 1,
    OUTSIDE = 2
}

public class UserCockpitController : Controller
{

    [SerializeField]
    private NextStepCockpitButton nextStepButton = default;

    [SerializeField]
    private TeleportCockpitButton teleportButton = default;

    [SerializeField]
    private Transform outsidePosition = default;

    [SerializeField]
    private Transform insidePosition = default;



    public void EnableContinueButton()
    {
        nextStepButton.EnableButton();
    }

    public void DisableContinueButton()
    {
        nextStepButton.DisableButton();
    }

    public void EnableTeleportButton()
    {
        teleportButton.EnableButton();
    }

    public void DisableTeleportButton()
    {
        teleportButton.DisableButton();
    }

    public void Reposition(int targetPosition)
    {
        if ((CockpitPosition)targetPosition == CockpitPosition.INSIDE)
        {
            transform.position = insidePosition.position;
        }
        else
        {
            transform.position = outsidePosition.position;
        }
    }

    public void SetTeleportType(int targetPosition)
    {
        teleportButton.SetType(targetPosition);
    }

}
