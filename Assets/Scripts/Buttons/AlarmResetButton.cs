using System.Collections;
using UnityEngine;

/// <summary>
/// Class for handling alarm reset button behaviour
/// </summary>
public class AlarmResetButton : PhysicalButton, IBlinkable, IResetable
{
    private enum lampState
    {
        OFF,
        ON,
        BLINKING
    }

    /// <summary>
    /// Light object to enable/blink
    /// </summary>
    [SerializeField]
    private GameObject errorLight = default;
    /// <summary>
    /// Is the button light currently blinking?     || Unused since 01.04.2021
    /// </summary>
    public bool Blinking { get; set; }

    /// <summary>
    /// Is power on robot's cabinet turn on? If not: error light won't turn on despite anything unless power is turn on again
    /// </summary>
    private bool isPowerOn = true;

    /// <summary>
    /// Variable to hold current lamp state -> used when power is reset to resume error light state
    /// </summary>
    private lampState errorLightState = lampState.OFF;

    /// <summary>
    /// Last used color that will be used in the event of power reset
    /// </summary>
    private int lastColor = default;

    /// <summary>
    /// Set up listeneres for events
    /// </summary>
    private void OnEnable()
    {
        RobotErrorController.OnErrorReset += () => DisableLight((int)LightColor.RED);
        AlarmErrorController.OnErrorReset += () => DisableLight((int)LightColor.RED);
    }

    /// <summary>
    /// Clears up listeners for events
    /// </summary>
    private void OnDisable()
    {
        RobotErrorController.OnErrorReset -= () => DisableLight((int)LightColor.RED);
        AlarmErrorController.OnErrorReset -= () => DisableLight((int)LightColor.RED);
    }

    /// <summary>
    /// Start blinking alarm light
    /// </summary>
    [EnumAction(typeof(LightColor))]
    public void BlinkLight(int color)
    {
        if (errorLightState != lampState.BLINKING && !ErrorRequester.HasAnyErrors())
        {
            EndBlinking();
            // Blinking = true;
            errorLightState = lampState.BLINKING;
            if (isPowerOn)
                StartCoroutine(StartBlinking(color));
            else
                lastColor = color;
        }
    }

    /// <summary>
    /// Enables alarm light
    /// </summary>
    [EnumAction(typeof(LightColor))]
    public void EnableLight(int color)
    {
        EndBlinking();
        // Blinking = false;
        errorLightState = lampState.ON;
        if (isPowerOn)
            errorLight.SetActive(true);
        else
            lastColor = color;
    }

    /// <summary>
    /// Disables alarm light
    /// </summary>
    [EnumAction(typeof(LightColor))]
    public void DisableLight(int color)
    {
        if (ErrorRequester.HasAllErrorsReset() && ErrorRequester.HasResetAlarmErrors())
        {
            EndBlinking();
            // Blinking = false;
            if (isPowerOn)
                errorLightState = lampState.OFF;
            errorLight.SetActive(false);
        }
    }

    /// <summary>
    /// Ends blinking of alarm light
    /// </summary>
    public void EndBlinking()
    {
        StopAllCoroutines();
        // Blinking = false;
        if (isPowerOn)
            errorLightState = lampState.OFF;
        errorLight.SetActive(false);
    }

    /// <summary>
    /// Main coroutine for blinking light
    /// </summary>
    /// <param name="obj">Template object (not used)</param>
    /// <returns>Handle to coroutine</returns>
    public IEnumerator StartBlinking(object obj)
    {
        while (true)
        {
            errorLight.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            errorLight.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Action run when button is pressed
    /// </summary>
    [ContextMenu("PRESS")]
    public void AlarmResetButtonPress()
    {
        if (isPowerOn)
        {
            OnPressed?.Invoke();
            DisableLight((int)LightColor.RED);
        }
    }

    /// <summary>
    /// Function that is called when power knob is turn off
    /// </summary>
    public void TurnPowerOff()
    {
        isPowerOn = false;
        StopAllCoroutines();
        errorLight.SetActive(false);
    }

    /// <summary>
    /// Function that is called when power knob is turn on
    /// </summary>
    public void TurnPowerOn()
    {
        isPowerOn = true;
        if (errorLightState != lampState.OFF)
        {
            switch (errorLightState)
            {
                case lampState.ON:
                    EnableLight(lastColor);
                    break;

                case lampState.BLINKING:
                    StopAllCoroutines();
                    errorLight.SetActive(false);
                    StartCoroutine(StartBlinking(lastColor));
                    break;
            }
        }
    }


    /// <summary>
    /// Resets object to default state
    /// </summary>
    void IResetable.Reset()
    {
        OnPressed?.Invoke();
    }
}
