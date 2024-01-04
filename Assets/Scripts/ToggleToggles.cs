using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Helper class for video using keyboard to check toggles
/// </summary>
//TODO: We dont need this anymore, check if anything refers to it
public class ToggleToggles : MonoBehaviour
{
    /// <summary>
    /// Toggle used when player stakcs cans on top
    /// </summary>
    public Toggle stackToggle;
    /// <summary>
    /// Toggle used when player sends robot to home position
    /// </summary>
    public Toggle homeToggle;
    /// <summary>
    /// Check if specified keys were pressed
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            stackToggle.isOn = true;
            foreach (Graphic g in stackToggle.GetComponentsInChildren<Graphic>())
            {
                g.color = Color.white;
            }
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            homeToggle.isOn = true;
            foreach (Graphic g in homeToggle.GetComponentsInChildren<Graphic>())
            {
                g.color = Color.white;
            }
        }
    }
}
