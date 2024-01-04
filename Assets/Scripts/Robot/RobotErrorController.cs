using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

/// <summary>
/// Controller for robot errors
/// </summary>
public class RobotErrorController : ErrorController, IResetable
{
    /// <summary>
    /// Robot object
    /// </summary>
    [SerializeField]
    private RobotController robotController = default;
    /// <summary>
    /// Reference to collision detector
    /// </summary>
    private CollisionDetector collisionDetector = default;
    /// <summary>
    /// Timer delaying start of detection of collisions
    /// </summary>
    private float timer = 0f;
    /// <summary>
    /// Is the reset button pressed?
    /// </summary>
    private bool resetButtonPressed = false;
    /// <summary>
    /// List of alarm reset buttons objects on scene
    /// </summary>
    private AlarmResetButton[] alarmResetButtons = default;
    /// <summary>
    /// List of cabinet reset buttons on scene
    /// </summary>
    private CabinetResetButton[] cabinetResetButtons = default;

    /// <summary>
    /// On Security errors additional action
    /// </summary>
    public static new Action OnErrorReset;


    public override bool ResetErrors()
    {
        if (base.ResetErrors())
        { OnErrorReset?.Invoke(); return true; }
        else
        { return false; }
    }

    public override void LinkRobot(Robot robot)
    {
        base.LinkRobot(robot);
        collisionDetector = robot.GetComponent<CollisionDetector>();
        collisionDetector.CollisionDetected += RequestCollisionError;
    }

    /// <summary>
    /// Disables detection of collision at start of application
    /// Sets reference to visualization of axis
    /// Sets singularity detection
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        alarmResetButtons = InteractablesManager.Instance.GetAllInteractableBehaviour<AlarmResetButton>();
        cabinetResetButtons = InteractablesManager.Instance.GetAllInteractableBehaviour<CabinetResetButton>();
    }

    /// <summary>
    /// Sets up listeners 
    /// </summary>
    private void OnEnable()
    {
        OnErrorReset += ErrorController.OnErrorReset;

        alarmResetButtons.ForEach(x => x.OnPressed.AddListener(clickAlarmsReset));
        cabinetResetButtons.ForEach(x => x.OnPressed.AddListener(clickAlarmsReset));
    }

    /// <summary>
    /// Unbinds detected collision event to error method
    /// </summary>
    private void OnDisable()
    {
        OnErrorReset -= ErrorController.OnErrorReset;

        collisionDetector.CollisionDetected -= RequestCollisionError;
        alarmResetButtons.ForEach(x => x.OnPressed.RemoveListener(clickAlarmsReset));
        cabinetResetButtons.ForEach(x => x.OnPressed.RemoveListener(clickAlarmsReset));
    }

    /// <summary>
    /// Raises error from error requester
    /// </summary>
    private void RequestCollisionError()
    {
        if (RobotData.Instance.MovementMode == MovementMode.AUTO)
            ErrorRequester.RaiseError("R-1001");
        else
            ErrorRequester.RaiseError("R-1002");
    }

    /// <summary>
    /// Sets state if the alarm button was pressed
    /// </summary>
    public void SetAlarmButton()
    {
        resetButtonPressed = true;
    }

    /// <summary>
    /// Tick method
    /// </summary>
    public override void Tick()
    {
        timer += Time.deltaTime;
        if (timer > 2f)
        {
            collisionDetector.EnableDetection();
        }

        if (RobotData.Instance.MovementMode != MovementMode.AUTO)
        {
            if (!robotController.DeadmanLeftPushed && !robotController.DeadmanRightPushed)
            {
                ErrorRequester.RaiseError("A-1001");
                resetButtonPressed = false;
            }

            if (robotController && (robotController.DeadmanLeftPushed || robotController.DeadmanRightPushed))
            {
                if (resetButtonPressed)
                {
                    ErrorRequester.ResetError("A-1001");
                    resetButtonPressed = false;
                }
            }
        }

    }

    /// <summary>
    /// Increments counter for errors
    /// </summary>
    public void IncrementRobotErrorCounter()
    {
        currentErrorCounter++;
    }

    /// <summary>
    /// Resets this to default state
    /// </summary>
    void IResetable.Reset()
    {
        ResetErrors();
    }

    /// <summary>
    /// Disables collision detection of robot
    /// </summary>
    public void SwitchOffCollision()
    {
        collisionDetector.DisableDetection();
    }

    /// <summary>
    /// Enables collision detection of robot
    /// </summary>
    public void SwitchOnCollision()
    {
        collisionDetector.EnableDetection();
    }

    private void clickAlarmsReset()
    {
        ErrorRequester.ResetAllRobotErrors();
    }
}
