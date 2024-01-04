using UnityEngine;

/// <summary>
/// Controls error dispatching and requesting
/// </summary>
[DisallowMultipleComponent]
public class ErrorRequester : MonoBehaviour, IResetable
{
    /// <summary>
    /// Handling robot error
    /// </summary>
    private static RobotErrorHandler robotErrorHandler;
    /// <summary>
    /// Handling security error
    /// </summary>
    private static SecurityErrorHandler securityErrorHandler;
    /// <summary>
    /// Handling alarm error
    /// </summary>
    private static AlarmErrorHandler alarmErrorHandler;
    /// <summary>
    /// Robot error controller object
    /// </summary>
    private static RobotErrorController robotError = default;
    /// <summary>
    /// Secuirty error controller object
    /// </summary>
    private static SecurityErrorController securityError = default;
    /// <summary>
    /// Alarm error controller object
    /// </summary>
    private static AlarmErrorController alarmError = default;

    /// <summary>
    /// Checks if any controller has at least one error
    /// </summary>
    /// <returns>Status of errors in controllers</returns>
    internal static bool HasAnyErrors()
    {
        return robotErrorHandler.ErrorController.IsAnyErrorActive() || securityErrorHandler.ErrorController.IsAnyErrorActive();
    }

    /// <summary>
    /// Checks if security error controller has at least one error not reset
    /// </summary>
    /// <returns>Status of reset errors in controller</returns>
    internal static bool HasResetSecurityErrors()
    {
        return securityErrorHandler.ErrorController.AreErrorsReset();
    }

    /// <summary>
    /// Checks if robot errors controller has at least one error not reset
    /// </summary>
    /// <returns>Status of reset errors in controller</returns>
    internal static bool HasResetRobotErrors()
    {
        return robotErrorHandler.ErrorController.AreErrorsReset();
    }

    /// <summary>
    /// Checks if alarm errors controller has at least one error not reset
    /// </summary>
    /// <returns>Status of reset errors in controller</returns>
    internal static bool HasResetAlarmErrors()
    {
        // return alarmErrorHandler.ErrorController.AreErrorsReset();
        return alarmErrorHandler.ErrorController.AreErrorsReset() && HasResetRobotErrors();     //30.03.2021 - decision were made to change robotErrors status from "error" to "alarms", although it was used so many times, that for avoiding bugs we are just changing already used methodes
    }

    /// <summary>
    /// Checks if any controller has at least one error not reset
    /// </summary>
    /// <returns>Status of reset errors in all controllers</returns>
    internal static bool HasAllErrorsReset()
    {
        // return HasResetRobotErrors() && HasResetSecurityErrors();
        return HasResetSecurityErrors();                                                         //30.03.2021 - decision were made to change robotErrors status from "error" to "alarms", although it was used so many times, that for avoiding bugs we are just changing already used methodes
    }

    /// <summary>
    /// Called at script loading
    /// </summary>
    private void Awake()
    {
        robotErrorHandler = GetComponentInChildren<RobotErrorHandler>();
        securityErrorHandler = GetComponentInChildren<SecurityErrorHandler>();
        alarmErrorHandler = GetComponentInChildren<AlarmErrorHandler>();

        robotError = GetComponentInChildren<RobotErrorController>();
        securityError = GetComponentInChildren<SecurityErrorController>();
        alarmError = GetComponentInChildren<AlarmErrorController>();
    }

    /// <summary>
    /// Request specific error raise action
    /// </summary>
    /// <param name="errorCode">Code of error to raise</param>
    public static void RaiseError(string errorCode)
    {
        if (errorCode.StartsWith("R-"))
        {
            robotErrorHandler.RaiseErrorCode(errorCode);
        }
        else if (errorCode.StartsWith("S-"))
        {
            securityErrorHandler.RaiseErrorCode(errorCode);
        }
        else if (errorCode.StartsWith("A-"))
        {
            alarmErrorHandler.RaiseErrorCode(errorCode);
        }
    }

    /// <summary>
    /// Request specific error unraise action
    /// </summary>
    /// <param name="errorCode">Code of error to unraise</param>
    public static void UnraiseError(string errorCode)
    {
        if (errorCode.StartsWith("R-"))
        {
            robotErrorHandler.UnraiseErrorCode(errorCode);
        }
        else if (errorCode.StartsWith("S-"))
        {
            securityErrorHandler.UnraiseErrorCode(errorCode);
        }
        else if (errorCode.StartsWith("A-"))
        {
            alarmErrorHandler.UnraiseErrorCode(errorCode);
        }
    }

    /// <summary>
    /// Request specific error to be reset
    /// </summary>
    /// <param name="errorCode">Code of error</param>
    public static void ResetError(string errorCode)
    {
        if (errorCode.StartsWith("R-"))
        {
            robotErrorHandler.ResetErrorCode(errorCode);
        }
        else if (errorCode.StartsWith("S-"))
        {
            securityErrorHandler.ResetErrorCode(errorCode);
        }
        else if (errorCode.StartsWith("A-"))
        {
            alarmErrorHandler.ResetErrorCode(errorCode);
        }
    }

    /// <summary>
    /// Update on all robot controllers
    /// </summary>
    private void Update()
    {
        securityError.Tick();
        robotError.Tick();
        alarmError.Tick();
    }

    /// <summary>
    /// Reset all errors in robot controller
    /// </summary>
    public static void ResetAllRobotErrors()
    {
        robotError.ResetErrors();
    }

    /// <summary>
    /// Resaet all secuirty errors
    /// </summary>
    public static void ResetAllSecurityErrors()
    {
        securityError.ResetErrors();
    }

    /// <summary>
    /// Reset all alarm errors
    /// </summary>
    public static void ResetAllAlarmErrors()
    {
        alarmError.ResetErrors();
    }

    /// <summary>
    /// Raises test error
    /// </summary>
    [ContextMenu("PRESS")]
    public void RaiseError()
    {
        RaiseError("R-1001");
    }

    /// <summary>
    /// Reset all errors
    /// </summary>
    public static void ResetLocalErrors()
    {
        if (HasResetSecurityErrors())
        {
            robotError.ResetErrors();
            alarmError.ResetErrors();
        }
        else
        {
            Debug.LogWarning("FAILED RESETING ERRORS");
        }
    }

    /// <summary>
    /// Resets state to initial 
    /// </summary>
    void IResetable.Reset()
    {
        UnraiseAllErrors();
    }

    /// <summary>
    /// Sets all errors to Unraised state
    /// </summary>
    private static void UnraiseAllErrors()
    {
        securityErrorHandler.UnraiseAllErrors();
        robotErrorHandler.UnraiseAllErrors();
        alarmErrorHandler.UnraiseAllErrors();
    }

    /// <summary>
    /// Enables (if gateState == false) or disables gate alarm (opening gate while in AUTO movement type). Example: if gateState will be true, then opening gate WON'T rise an error
    /// </summary>
    public static void SetGateAlarm(bool gateState)
    {
        securityError.ClearStop = gateState;
    }
}
