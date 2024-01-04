using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Events;

/// <summary>
/// Implements logic of power knob on robot's cabinet
/// </summary>
public class PowerKnob : MonoBehaviour, IResetable
{
    /// <summary>
    /// Flag to check current power state (knob position)
    /// </summary>
    [HideInInspector]
    private bool turnOnState = default;

    /// <summary>
    /// Flag to check if powerOffError has been raised (and et not set to "unraised" state)
    /// </summary>
    [HideInInspector]
    private bool powerResetFlag = default;

    /// <summary>
    /// Transform of power knob (starting position set to powerOn state)
    /// </summary>
    [SerializeField]
    private Transform knobTransform = default;

    /// <summary>
    /// Reference to POWER lamp on robot's cabinet (which represents current power state) 
    /// </summary>
    [SerializeField]
    private GameObject powerLight = default;

    /// <summary>
    /// Event called when power is turn on again (this is ALSO called on Awake)
    /// </summary>
    [SerializeField]
    public UnityEvent onPowerTurnOn = default;

    /// <summary>
    /// Event called when power is turned off
    /// </summary>
    [SerializeField]
    public UnityEvent onPowerTurnOff = default;

    /// <summary>
    /// Code used in errorController (precissly: RobotErrorController AND RobotErrorContainer) to identify powerOffError
    /// </summary>
    [HideInInspector]
    private static string powerOffErrorName = "R-1006-0";

    /// <summary>
    /// Sets knob and flags at starting (default - turn on state) positions
    /// </summary>
    private void Awake()
    {
        turnOnState = true;
        powerResetFlag = false;
        turnPowerOn();
    }

    /// <summary>
    /// Methode to run after power is switched to "turnON" state
    /// </summary>
    private void turnPowerOn()
    {
        Vector3 rot = new Vector3(0f, 0f, 0f);
        knobTransform.DOLocalRotate(rot, 2.0f);
        onPowerTurnOn?.Invoke();
        powerLight?.SetActive(true);
        if (powerResetFlag)
        {
            powerResetFlag = false;
            ErrorRequester.UnraiseError(powerOffErrorName);
        }
    }

    /// <summary>
    /// Methode to run after power is switched to "turnOFF" state
    /// </summary>
    private void turnPowerOff()
    {
        Vector3 rot = new Vector3(0f, -150f, 0f);
        knobTransform.DOLocalRotate(rot, 2.0f);
        onPowerTurnOff?.Invoke();
        powerLight?.SetActive(false);
        if (!powerResetFlag)
        {
            powerResetFlag = true;
            ErrorRequester.RaiseError(powerOffErrorName);
        }
    }

    /// <summary>
    /// Function that should be called on interaction with power knob on scene
    /// </summary>
    public void RotateKnob()
    {
        if (turnOnState)
        {
            turnOnState = false;
            turnPowerOff();
        }
        else
        {
            turnOnState = true;
            turnPowerOn();
        }
    }

    /// <summary>
    /// Resets power knob to it's default position (turnOn state)
    /// </summary>
    void IResetable.Reset()
    {
        if (!turnOnState)
        {
            turnOnState = true;
            Vector3 rot = new Vector3(0f, 0f, 0f);
            knobTransform.DOLocalRotate(rot, 0.5f);
            onPowerTurnOn.Invoke();
            if (powerResetFlag)
            {
                powerResetFlag = false;
                ErrorRequester.ResetError(powerOffErrorName);
            }
        }
    }
}
