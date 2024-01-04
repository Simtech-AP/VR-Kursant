using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Class to handle the behavior of the mode key
/// </summary>
public class ModeKey : PhysicalButton, IResetable
{
    /// <summary>
    /// Reference to robot controller
    /// </summary>
    [SerializeField]
    private RobotController robotController = default;
    /// <summary>
    /// Is the direction of cycling states reversed?
    /// </summary>
    private bool reversed;

    /// <summary>
    /// Is power on robot's cabinet turn on? If not: changing key position doesn't change movement mode until power will be turn on again
    /// </summary>
    private bool isPowerOn = true;

    /// <summary>
    /// Variable to hold current key "position" (movement mode) - to set robot movement mode after power reset
    /// </summary>
    private MovementMode keyMovementMode = default;

    /// <summary>
    /// Event called on switching movement mode
    /// </summary>
    [SerializeField]
    private UnityEvent onModeSwitch = default;

    /// <summary>
    /// Sets position of the key and keyMovementMode on starting position
    /// </summary>
    private void Awake()
    {
        isPowerOn = false;
        SetMode(RobotData.Instance.MovementMode);
        isPowerOn = true;
    }

    /// <summary>
    /// Move mode key to next state
    /// </summary>
    [ContextMenu("CYCLE")]
    public void CycleState()
    {
        switch (keyMovementMode)
        {
            case MovementMode.AUTO:
                reversed = false;
                Set(MovementMode.T1);
                break;
            case MovementMode.T1:
                Action met = (reversed == true ? () => Set(MovementMode.AUTO) : met = () => Set(MovementMode.T2));
                met.Invoke();
                break;
            case MovementMode.T2:
                reversed = true;
                Set(MovementMode.T1);
                break;
        }
    }

    /// <summary>
    /// Sets movement mode
    /// </summary>
    /// <param name="movementMode">Movement mode to set</param>
    [EnumAction(typeof(MovementMode))]
    public void SetMode(MovementMode movementMode)
    {
        Set(movementMode);
    }

    /// <summary>
    /// Rotates key visually according to current state
    /// </summary>
    /// <param name="state">State of key</param>
    private void RotateTowards(MovementMode state)
    {
        switch (state)
        {
            case MovementMode.T1:
                RotateKey(new Vector3(0f, 30f, 0f));
                break;
            case MovementMode.T2:
                RotateKey(new Vector3(0f, 87f, 0f));
                break;
            case MovementMode.AUTO:
                RotateKey(new Vector3(0f, 0f, 0f));
                break;
        }
    }

    /// <summary>
    /// Rotates key visually to specified rotation
    /// </summary>
    /// <param name="rot">Rotation to rotate to</param>
    private void RotateKey(Vector3 rot)
    {
        transform.DOLocalRotate(rot, 0.5f);
    }

    /// <summary>
    /// Resets key to default state
    /// </summary>
    void IResetable.Reset()
    {
        var scene = SceneManager.GetActiveScene();
        Action act = (scene.name.Equals("Module1") || scene.name.Equals("Module2")) ? () => Set(MovementMode.AUTO) : act = () => Set(RobotData.Instance.MovementMode);
        act.Invoke();
    }

    /// <summary>
    /// Sets movement mode
    /// </summary>
    /// <param name="mode">Movement mode to set</param>
    private void Set(MovementMode mode)
    {
        RotateTowards(mode);
        keyMovementMode = mode;
        if (isPowerOn)
        {
            robotController.SwitchMovementMode(mode);
            onModeSwitch?.Invoke();
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
        if (RobotData.Instance.MovementMode != keyMovementMode)
        {
            robotController.SwitchMovementMode(keyMovementMode);
            onModeSwitch?.Invoke();
        }
    }
}
