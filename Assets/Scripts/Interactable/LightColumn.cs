using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Light states on column
/// </summary>
public enum LightColor
{
    RED = 0,
    YELLOW = 1,
    GREEN = 2,
    ALL = 3
}

/// <summary>
/// Class to handle the behavior of the light column
/// </summary>
public class LightColumn : MonoBehaviour, IBlinkable
{
    /// <summary>
    /// Current color of light column
    /// </summary>
    private LightColor currentColor;
    /// <summary>
    /// Red light game object
    /// </summary>
    [SerializeField]
    private MeshRenderer redLight = default;
    /// <summary>
    /// Yellow light game object
    /// </summary>
    [SerializeField]
    private MeshRenderer yellowLight = default;
    /// <summary>
    /// Green light game object
    /// </summary>
    [SerializeField]
    private MeshRenderer greenLight = default;
    /// <summary>
    /// List of all buttons with green light
    /// </summary>
    [SerializeField]
    private List<GameObject> greenLightButtons = default;
    /// <summary>
    /// Intensity of light emission
    /// </summary>
    [SerializeField]
    private float emissionIntensity = 1.7f;
    /// <summary>
    /// Is the column lights blinking?
    /// </summary>
    public bool Blinking { get; set; }

    /// <summary>
    /// Sets up listeners for column
    /// </summary>
    private void OnEnable()
    {
        RobotData.OnUpdatedData += SwitchLight;
        RobotErrorController.OnErrorReset += () => DisableLight((int)LightColor.RED);
        AlarmErrorController.OnErrorReset += () => DisableLight((int)LightColor.RED);
    }

    /// <summary>
    /// Resets listeners for column
    /// </summary>
    private void OnDisable()
    {
        RobotData.OnUpdatedData -= SwitchLight;
        RobotErrorController.OnErrorReset -= () => DisableLight((int)LightColor.RED);
        AlarmErrorController.OnErrorReset -= () => DisableLight((int)LightColor.RED);
    }

    /// <summary>
    /// Enables light of specified color
    /// </summary>
    /// <param name="color">Color enumeration</param>
    [EnumAction(typeof(LightColor))]
    public void EnableLight(int color)
    {
        TurnOff(LightColor.ALL);
        EndBlinking();
        Blinking = false;
        TurnOn((LightColor)color);
    }

    /// <summary>
    /// Enables blinking light of specified color
    /// </summary>
    /// <param name="color">Color enumeration</param>
    [EnumAction(typeof(LightColor))]
    public void BlinkLight(int color)
    {
        if (!Blinking && !ErrorRequester.HasAnyErrors())
        {
            TurnOff(LightColor.ALL);
            EndBlinking();
            Blinking = true;
            StartCoroutine(StartBlinking((object)color));
        }
    }

    /// <summary>
    /// Disables light of specified color
    /// </summary>
    /// <param name="color">Color enumeration</param>
    [EnumAction(typeof(LightColor))]
    public void DisableLight(int color)
    {
        if (ErrorRequester.HasAllErrorsReset() && ErrorRequester.HasResetAlarmErrors())
        {
            EndBlinking((LightColor)color);
            Blinking = false;
            TurnOff((LightColor)color);
        }
    }

    /// <summary>
    /// Sets column color according to specified state
    /// </summary>
    /// <param name="color">State to set color to</param>
    public void SetColumnColor(LightColor color)
    {
        TurnOff(currentColor);
        currentColor = color;
        TurnOn(currentColor);
        Blinking = false;
    }

    /// <summary>
    /// Enables light according to state
    /// </summary>
    /// <param name="color">State to set color to</param>
    private void TurnOn(LightColor color)
    {
        switch (color)
        {
            case LightColor.RED:
                redLight.material.SetColor("_EmissionColor", Color.red * emissionIntensity);
                break;
            case LightColor.YELLOW:
                yellowLight.material.SetColor("_EmissionColor", Color.yellow * emissionIntensity);
                break;
            case LightColor.GREEN:
                greenLight.material.SetColor("_EmissionColor", Color.green * emissionIntensity);
                greenLightButtons.ForEach(x => x.SetActive(true));
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Disables light according to state
    /// </summary>
    /// <param name="color">State to disable color of</param>
    private void TurnOff(LightColor color)
    {
        switch (color)
        {
            case LightColor.RED:
                redLight.material.SetColor("_EmissionColor", Color.black * emissionIntensity);
                break;
            case LightColor.YELLOW:
                yellowLight.material.SetColor("_EmissionColor", Color.black * emissionIntensity);
                break;
            case LightColor.GREEN:
                greenLight.material.SetColor("_EmissionColor", Color.black * emissionIntensity);
                greenLightButtons.ForEach(x => x.SetActive(false));
                break;
            case LightColor.ALL:
                greenLight.material.SetColor("_EmissionColor", Color.black * emissionIntensity);
                yellowLight.material.SetColor("_EmissionColor", Color.black * emissionIntensity);
                redLight.material.SetColor("_EmissionColor", Color.black * emissionIntensity);
                greenLightButtons.ForEach(x => x.SetActive(false));
                break;
            default:

                break;
        }
    }

    /// <summary>
    /// Main blinking coroutine
    /// </summary>
    /// <param name="color">Color of light to blink</param>
    /// <returns>Handle to coroutine</returns>
    public IEnumerator StartBlinking(object color)
    {
        while (true)
        {
            TurnOn((LightColor)color);
            yield return new WaitForSeconds(0.5f);
            TurnOff((LightColor)color);
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Ends all light blinking
    /// </summary>
    public void EndBlinking()
    {
        StopAllCoroutines();
        Blinking = false;
        TurnOff(LightColor.ALL);
    }

    /// <summary>
    /// Ends specific light blinking
    /// </summary>
    public void EndBlinking(LightColor color)
    {
        StopAllCoroutines();
        Blinking = false;
        TurnOff(color);
    }

    /// <summary>
    /// Switches state of column according to data object
    /// </summary>
    /// <param name="obj">Robot data object</param>
    private void SwitchLight(RobotData obj)
    {
        if (obj.IsRunning)
            SetColumnColor(LightColor.GREEN);
        else
            TurnOff(LightColor.GREEN);

        switch (obj.MovementMode)
        {
            case MovementMode.T1:
            case MovementMode.T2:
                TurnOff(LightColor.GREEN);
                break;
        }
    }
}
