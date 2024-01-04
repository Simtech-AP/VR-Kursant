using System.Collections;
using UnityEngine;

/// <summary>
/// Contains logic for enabling and disabling button for next step
/// </summary>
public class NextStepButtonController : Controller
{
    /// <summary>
    /// Reference to button gameobject on scene
    /// </summary>
    [SerializeField]
    private NextStepCockpitButton button = default;


    /// <summary>
    /// Disables button on scene
    /// </summary>
    public void DisableButton()
    {
        button.DisableButton();
    }

    /// <summary>
    /// Enables button on scene
    /// </summary>
    public void EnableButton()
    {
        button.EnableButton();
    }
}
