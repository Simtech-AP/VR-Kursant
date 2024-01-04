using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class for pressing EStop switch
/// </summary>
public class EStopPress : MonoBehaviour
{
    /// <summary>
    /// Presses EStop on robot control unit
    /// </summary>
    public void PressEstop()
    {
        List<GameObject> estops = InteractablesManager.Instance.GetAllInteractableButton(ButtonType.ESTOP);
        estops.Find(x => x.name == "szafa_robota_EStop").GetComponent<EStop>().EStopToggle(true);
    }
}
