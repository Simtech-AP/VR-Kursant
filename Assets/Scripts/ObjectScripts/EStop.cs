using UnityEngine;

/// <summary>
/// class to handle the behavior of the EStop button
/// </summary>
public class EStop : PhysicalButton, IResetable
{
    /// <summary>
    /// Index of this EStop button
    /// </summary>
    [HideInInspector]
    public int EStopIndex = 0;
    /// <summary>
    /// Position of pressed button in local space
    /// </summary>
    [SerializeField]
    private Vector3 pressedLocalPosition = default;
    /// <summary>
    /// Position of released button in local space
    /// </summary>
    [SerializeField]
    private Vector3 relesedLocalPosition = default;

    /// <summary>
    /// Is power on robot's shelf turn on? If not: pressing EStop doesn't call error until power will be turn on again
    /// </summary>
    private bool isPowerOn = true;
    /// <summary>
    /// Current state of EStop
    /// </summary>
    private int estopState = EStopButtonState.Released;

    /// <summary>
    /// Presses EStop
    /// </summary>
    public void InvokeEStopPressed()
    {
        EStopToggle(true);
    }

    /// <summary>
    /// Releases EStop 
    /// </summary>
    public void InvokeEStopRelesed()
    {
        EStopToggle(false);
    }

    /// <summary>
    /// Changes position of EStop button to pressed
    /// </summary>
    public void ChangePositionPressed()
    {
        transform.localPosition = pressedLocalPosition;
    }

    /// <summary>
    /// Changes position of EStop button to released
    /// </summary>
    public void ChangePositionRelesed()
    {
        transform.localPosition = relesedLocalPosition;
    }

    /// <summary>
    /// Toggles EStop state according do passed state
    /// </summary>
    /// <param name="active">Is the button pressed?</param>
    public void EStopToggle(bool active)
    {
        if (CellStateData.EStopStates[EStopIndex] != EStopButtonState.Pressed && active)
        {
            CellStateData.EStopStates[EStopIndex] = EStopButtonState.Pressed;
            OnPressed.Invoke();
            ChangePositionPressed();
        }
        else if (CellStateData.EStopStates[EStopIndex] == EStopButtonState.Pressed && !active)
        {
            CellStateData.EStopStates[EStopIndex] = EStopButtonState.Released;
            OnReleased.Invoke();
            ChangePositionRelesed();
        }
    }

    /// <summary>
    /// Toggles EStop state
    /// </summary>
    [ContextMenu("PRESS")]
    public void EStopToggle()
    {
        if (estopState == EStopButtonState.Released)
        {
            estopState = EStopButtonState.Pressed;
            if (isPowerOn)
            {
                CellStateData.EStopStates[EStopIndex] = EStopButtonState.Pressed;
                OnPressed.Invoke();
            }
            ChangePositionPressed();
        }
        else
        {
            estopState = EStopButtonState.Released;
            if (isPowerOn)
            {
                CellStateData.EStopStates[EStopIndex] = EStopButtonState.Released;
                OnReleased.Invoke();
            }
            ChangePositionRelesed();
        }
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
        if (CellStateData.EStopStates[EStopIndex] != estopState)
        {
            CellStateData.EStopStates[EStopIndex] = estopState;
            if (estopState == EStopButtonState.Pressed)
                OnPressed?.Invoke();
            else
                OnReleased?.Invoke();
        }
    }

    /// <summary>
    /// Resets object to default state
    /// </summary>
    void IResetable.Reset()
    {
        EStopToggle(false);
    }
}
