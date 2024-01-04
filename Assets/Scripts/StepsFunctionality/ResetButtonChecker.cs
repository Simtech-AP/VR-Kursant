using UnityEngine;

/// <summary>
/// Checks if we presset reset button while holding deadman
/// </summary>
public class ResetButtonChecker : StepEnabler
{
    /// <summary>
    /// Are we pressing any deadman switch?
    /// </summary>
    private bool deadmanPressed = false;


    /// <summary>
    /// Toggles pressed edaman flag according to press
    /// </summary>
    /// <param name="flag">State of presss to set</param>
    public void DeadManToggle(bool flag)
    {
        deadmanPressed = flag;
    }

    /// <summary>
    /// Run when reset button is pressed
    /// </summary>
    public void ResetPressedOnDeadManPushed()
    {
        if (deadmanPressed)
        {
            Enabled = true;
        }
        else
        {
            Enabled = false;
        }
    }

    /// <summary>
    /// Run when reset button is released
    /// </summary>
    public void ResetPressedOnDeadManReleased()
    {
        if (!deadmanPressed)
        {
            Enabled = true;
        }
        else
        {
            Enabled = false;
        }
    }
}
