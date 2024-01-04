using System.Collections;
using UnityEngine;

/// <summary>
/// Controls behaviour of lights on control box on cell
/// </summary>
public class ControlBoxLightController : MonoBehaviour, IBlinkable
{
    /// <summary>
    /// Red light game object
    /// </summary>
    [SerializeField]
    private GameObject redLight = default;
    /// <summary>
    /// Yellow light game object
    /// </summary>
    [SerializeField]
    private GameObject greenLight = default;
    /// <summary>
    /// Color to blink the casette
    /// </summary>
    private LightColor colorToBlink;
    /// <summary>
    /// Are the control box lights blinking?
    /// </summary>
    public bool Blinking { get; set; }

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
    /// Turns on color on casette
    /// </summary>
    /// <param name="color">Color to change on</param>
    private void TurnOn(LightColor color)
    {
        switch (color)
        {
            case LightColor.RED:
                redLight.SetActive(true);
                break;
            case LightColor.GREEN:
                greenLight.SetActive(true);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Turns off color on casette
    /// </summary>
    /// <param name="color">Color to change off</param>
    private void TurnOff(LightColor color)
    {
        switch (color)
        {
            case LightColor.RED:
                redLight.SetActive(false);
                break;
            case LightColor.GREEN:
                greenLight.SetActive(false);
                break;
            case LightColor.ALL:
                redLight.SetActive(false);
                greenLight.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// Disables light on casette
    /// </summary>
    /// <param name="lightName">String depicting color enum to disable</param>
    [EnumAction(typeof(LightColor))]
    public void DisableLight(int lightName)
    {
        if (ErrorRequester.HasAllErrorsReset() && ErrorRequester.HasResetAlarmErrors())
        {
            EndBlinking(lightName);
            Blinking = false;
            TurnOff((LightColor)lightName);
        }
    }

    /// <summary>
    /// Enables light on casette
    /// </summary>
    /// <param name="lightName">String depicting color enum to enable</param>
    [EnumAction(typeof(LightColor))]
    public void EnableLight(int lightName)
    {
        TurnOff(LightColor.ALL);
        EndBlinking();
        Blinking = false;
        TurnOn((LightColor)lightName);
    }

    /// <summary>
    /// Starts blinking of light
    /// </summary>
    /// <param name="lightName">String depicting color enum to blink</param>
    [EnumAction(typeof(LightColor))]
    public void BlinkLight(int lightName)
    {
        if (!Blinking && !ErrorRequester.HasAnyErrors())
        {
            TurnOff(LightColor.ALL);
            EndBlinking();
            Blinking = true;
            StartCoroutine(StartBlinking((LightColor)lightName));
        }
    }

    /// <summary>
    /// Blink light on delay
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public IEnumerator StartBlinking(object obj)
    {
        while (true)
        {
            TurnOn(colorToBlink);
            yield return new WaitForSeconds(0.5f);
            TurnOff(colorToBlink);
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Stops blinking of casette lights
    /// </summary>
    public void EndBlinking()
    {
        StopAllCoroutines();
        Blinking = false;
        TurnOff(LightColor.ALL);
    }

    /// <summary>
    /// Stops blinking of casette specific lights
    /// </summary>
    public void EndBlinking(int lightName)
    {
        StopAllCoroutines();
        Blinking = false;
        TurnOff((LightColor)lightName);
    }
}
