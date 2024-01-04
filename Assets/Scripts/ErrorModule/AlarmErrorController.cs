using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Controller for alarm errors
/// </summary>
public class AlarmErrorController : ErrorController
{
    /// <summary>
    /// Previously active movement mode
    /// </summary>
    private MovementMode prevMode;
    /// <summary>
    /// On Security errors additional action
    /// </summary>
    public static new Action OnErrorReset;


    private AlarmResetButton[] alarmResetButtons = default;
    /// <summary>
    /// List of cabinet reset buttons on scene
    /// </summary>
    private CabinetResetButton[] cabinetResetButtons = default;

    /// <summary>
    /// Sets up variables
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        prevMode = RobotData.Instance.MovementMode;
        alarmResetButtons = InteractablesManager.Instance.GetAllInteractableBehaviour<AlarmResetButton>();
        cabinetResetButtons = InteractablesManager.Instance.GetAllInteractableBehaviour<CabinetResetButton>();
    }

    /// <summary>
    /// Sets up listener
    /// </summary>
    private void OnEnable()
    {
        OnErrorReset += ErrorController.OnErrorReset;
        RobotData.OnUpdatedData += ResetInternalError;
        alarmResetButtons.ToList().ForEach(x => x.OnPressed.AddListener(clickAlarmsReset));
        cabinetResetButtons.ToList().ForEach(x => x.OnPressed.AddListener(clickAlarmsReset));
    }

    /// <summary>
    /// Clears listener
    /// </summary>
    private void OnDisable()
    {
        OnErrorReset -= ErrorController.OnErrorReset;
        RobotData.OnUpdatedData -= ResetInternalError;
        alarmResetButtons.ToList().ForEach(x => x.OnPressed.RemoveListener(clickAlarmsReset));
        cabinetResetButtons.ToList().ForEach(x => x.OnPressed.RemoveListener(clickAlarmsReset));
    }

    /// <summary>
    /// Resetes error riased from robot
    /// </summary>
    /// <param name="obj">Reference to robot data</param>
    private void ResetInternalError(RobotData obj)
    {
        if (obj.MovementMode != prevMode)
        {
            prevMode = obj.MovementMode;
            if (prevMode == MovementMode.AUTO)
            {
                ErrorRequester.ResetError("A-1001");
                if (currentErrorCounter <= 0 && ErrorRequester.HasAllErrorsReset())
                    ResetErrors();
            }
        }
    }

    /// <summary>
    /// Resets errors on this error controller
    /// </summary>
    /// <returns>Have we resetted all errors?</returns>
    public override bool ResetErrors()
    {
        currentErrorCounter = 0;
        for (int i = 0; i < errorList.Count; ++i)
        {
            Error.Occurrence ocur;
            ocur = errorList[i].GetLastOccurence();
            if (ocur != null)
            {
                if (ocur.Status == Error.Occurrence.OccurrenceStatus.Raised)
                {
                    ResetError(i);
                }
            }
        }
        OnErrorReset?.Invoke();
        ErrorController.OnErrorReset.Invoke();
        return true;
    }

    /// <summary>
    /// Sets all errors to Unraised status
    /// </summary>
    private void SetErrorsUnraised()
    {
        for (int i = 0; i < errorList.Count; ++i)
        {
            Error.Occurrence ocur;
            ocur = errorList[i].GetLastOccurence();
            if (ocur != null)
            {
                ocur.SetOccurenceStatus(Error.Occurrence.OccurrenceStatus.Unraised);
            }
        }
    }

    private void clickAlarmsReset()
    {
        ErrorRequester.ResetAllAlarmErrors();
    }
}
