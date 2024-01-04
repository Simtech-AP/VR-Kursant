using UnityEngine;

public class CabinetResetButton : PhysicalButton
{
    /// <summary>
    /// Is power on robot's cabinet turn on? If not: reset cannot be used until power will be turn on again
    /// </summary>
    private bool isPowerOn = true;

    /// <summary>
    /// Action run when button is pressed
    /// </summary>
    [ContextMenu("PRESS")]
    public void CabinetButtonPress()
    {
        if (isPowerOn)
            OnPressed?.Invoke();
    }

    /// <summary>
    /// Function that is called when power knob is turn off
    /// </summary>
    public void TurnPowerOff()
    {
        isPowerOn = false;
    }

    /// <summary>
    /// Function that is called when power knob is turn on
    /// </summary>
    public void TurnPowerOn()
    {
        isPowerOn = true;
    }
}
