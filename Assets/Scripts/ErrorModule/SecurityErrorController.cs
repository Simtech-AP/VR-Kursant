using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Valve.VR.InteractionSystem;

/// <summary>
/// Controller for secuirty errors
/// </summary>
public class SecurityErrorController : ErrorController
{
    /// <summary>
    /// EStop buttons
    /// </summary>
    private EStop[] eStops;
    /// <summary>
    /// List of all buttons used to reset security errors
    /// </summary>
    private SecurityResetButton[] securityResetButtons;
    /// <summary>
    /// List of all buttons used to reset alarm errors
    /// </summary>
    private AlarmResetButton[] alarmResetButtons;
    /*
        /// <summary>
        /// List of cabinet reset buttons on scene
        /// </summary>
        // private CabinetResetButton[] cabinetResetButtons = default;
    */
    /// <summary>
    /// Has the alarm reset button been pressed?
    /// </summary>
    private bool alarmsResetPressed = false;

    /// <summary>
    /// On Security errors additional action
    /// </summary>
    public static new Action OnErrorReset;

    /// <summary>
    /// If true - opening security gate while on AUTO movement type WON'T rise an error
    /// </summary>
    public bool ClearStop { get; set; } = false;

    /// <summary>
    /// Called when script is loaded
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        eStops = InteractablesManager.Instance.GetAllInteractableBehaviour<EStop>();
        securityResetButtons = InteractablesManager.Instance.GetAllInteractableBehaviour<SecurityResetButton>();
        alarmResetButtons = InteractablesManager.Instance.GetAllInteractableBehaviour<AlarmResetButton>();
        // cabinetResetButtons = InteractablesManager.Instance.GetAllInteractableBehaviour<CabinetResetButton>();
    }

    /// <summary>
    /// Called when script is enabled
    /// </summary>
    private void OnEnable()
    {
        OnErrorReset += ErrorController.OnErrorReset;

        RobotData.OnUpdatedData += SwitchedToNewMovementMode;

        for (int i = 0; i < eStops.Length; i++)
        {
            var code = "S-1001-" + i.ToString();
            eStops[i].OnPressed.AddListener(() => RaiseESTOP(code));
            eStops[i].OnReleased.AddListener(() => UnraiseESTOP(code));

        }

        securityResetButtons.ForEach(x => x.OnPressed.AddListener(clickSafetyRest));
        //alarmResetButtons.ForEach(x => x.OnPressed.AddListener(clickAlarmsReset));
    }

    /// <summary>
    /// Checks is all security errors have been eliminated
    /// </summary>
    private async void ClickSafety()
    {
        alarmsResetPressed = false;
        alarmResetButtons.ForEach(x => x.OnPressed.AddListener(() => alarmsResetPressed = true));
        // cabinetResetButtons.ForEach(x => x.OnPressed.AddListener(() => alarmsResetPressed = true));
        CancellationTokenSource source = new CancellationTokenSource();

        try
        {
            await AlarmsPressed(source);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        if (alarmsResetPressed)
        {
            ErrorRequester.ResetAllSecurityErrors();
            alarmResetButtons.ForEach(x => x.OnPressed.Invoke());
            // cabinetResetButtons.ForEach(x => x.OnPressed.Invoke());
        }
    }

    private void clickSafetyRest()
    {
        ErrorRequester.ResetAllSecurityErrors();
    }

    /// <summary>
    /// Asynchronous method checking if alarm resets have been pressed
    /// </summary>
    /// <param name="source">Cancellation element to check</param>
    /// <returns>Handle to Task object</returns>
    private async System.Threading.Tasks.Task AlarmsPressed(CancellationTokenSource source)
    {
        OnErrorOccured += (ErrorController eController, Error error) => { source.Cancel(); };

        while (!alarmsResetPressed)
        {
            if (source.IsCancellationRequested)
                throw new TaskCanceledException();

            await System.Threading.Tasks.Task.Yield();
        }
    }

    /// <summary>
    /// Unraise ESTOP
    /// </summary>
    private void UnraiseESTOP(string withCode = "S-1001")
    {
        if (CellStateData.EStopStates.All(x => x == EStopButtonState.Released))
        {
            ErrorRequester.UnraiseError("S-1001");
        }

        ErrorRequester.UnraiseError(withCode);

    }

    /// <summary>
    /// Raise ESTOP
    /// </summary>
    private void RaiseESTOP(string withCode = "S-1001")
    {
        ErrorRequester.RaiseError(withCode);
    }

    /// <summary>
    /// Called when script is disabled
    /// </summary>
    private void OnDisable()
    {
        OnErrorReset -= ErrorController.OnErrorReset;

        RobotData.OnUpdatedData -= SwitchedToNewMovementMode;
        for (int i = 0; i < eStops.Length; i++)
        {
            var code = "S-1001-" + i.ToString();
            eStops[i].OnPressed.RemoveListener(() => RaiseESTOP(code));
            eStops[i].OnReleased.RemoveListener(() => UnraiseESTOP(code));

        }

        securityResetButtons.ForEach(x => x.OnPressed.RemoveListener(clickSafetyRest));
        //alarmResetButtons.ForEach(x => x.OnPressed.RemoveListener(clickAlarmsReset));
    }

    /// <summary>
    /// Switch to new movement mode handling error
    /// </summary>
    /// <param name="data">Reference to robot data object</param>
    private void SwitchedToNewMovementMode(RobotData data)
    {
        if (data.MovementMode == MovementMode.T1)
        {
            ErrorRequester.UnraiseError("S-1002");
        }
    }

    /// <summary>
    /// Resets errors in this error controller
    /// </summary>
    /// <returns>Have we resetted all errors?</returns>
    public override bool ResetErrors()
    {
        if (base.ResetErrors())
        { OnErrorReset?.Invoke(); return true; }
        else
        { return false; }
    }

    /// <summary>
    /// Tick method
    /// </summary>
    public override void Tick()
    {
        if (RobotData.Instance.MovementMode == MovementMode.AUTO && !ClearStop)
        {
            if (CellStateData.cellEntranceState != CellEntranceState.Closed)
            {
                ErrorRequester.RaiseError("S-1002");
            }
            else if (CellStateData.cellEntranceState == CellEntranceState.Closed && EStopsUnpressed())
            {
                ErrorRequester.UnraiseError("S-1002");
            }
        }
    }

    /// <summary>
    /// Checks if all estops are unpressed
    /// </summary>
    /// <returns>ESTOP pressed status</returns>
    private bool EStopsUnpressed()
    {
        for (int i = 0; i < CellStateData.EStopStates.Count; ++i)
        {
            if (CellStateData.EStopStates[i] == EStopButtonState.Pressed)
                return false;
        }

        return true;
    }
}
