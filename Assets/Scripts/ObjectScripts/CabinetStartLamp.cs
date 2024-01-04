using UnityEngine;

public class CabinetStartLamp : MonoBehaviour
{
    /// <summary>
    /// Reference on startLamp
    /// </summary>
    [SerializeField]
    private GameObject startLamp = default;

    /// <summary>
    /// Is power on robot's cabinet turn on? If not: start lamp doesn't work until power will be turn on again
    /// </summary>
    [HideInInspector]
    private bool isPowerOn = true;

    /// <summary>
    /// Flag of lam state in prev frame (to avoid suddent blinking for example when reseting errors in T1)
    /// </summary>
    [HideInInspector]
    private bool lastLampState = default;


    /// <summary>
    /// Runs methode to turn on, or off startLamp
    /// </summary>
    private void Update()
    {
        checkStartLampState(RobotData.Instance);
    }

    /// <summary>
    /// Turns startLamp on, or off depending on movement type and errors state
    /// </summary>
    private void checkStartLampState(RobotData data)
    {
        if (!isPowerOn)
        {
            startLamp.SetActive(false);
            lastLampState = false;
            return;
        }
        switch (data.MovementMode)
        {
            case MovementMode.T1:
            case MovementMode.T2:
                if (ErrorRequester.HasAllErrorsReset() && ErrorRequester.HasResetAlarmErrors())
                {
                    if (lastLampState)
                        startLamp.SetActive(true);
                    else
                        lastLampState = true;
                }
                else
                {
                    if (!lastLampState)
                        startLamp.SetActive(false);
                    else
                        lastLampState = false;
                }
                break;

            case MovementMode.AUTO:
                if (data.IsRunning)
                {
                    if (lastLampState)
                        startLamp.SetActive(true);
                    else
                        lastLampState = true;
                }
                else
                {
                    if (!lastLampState)
                        startLamp.SetActive(false);
                    else
                        lastLampState = false;
                }
                break;

            default:
                Debug.LogError("Unexpected movement type: " + data.MovementMode);
                break;
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
    }
}
