using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Error handling abstract class for error controllers
/// </summary>
[DisallowMultipleComponent]
public abstract class ErrorHandler : MonoBehaviour
{
    /// <summary>
    /// Error controller object which is handled by Handler
    /// </summary>
    [SerializeField]
    private ErrorController errorController = default;
    /// <summary>
    /// Error messages container
    /// </summary>
    [SerializeField]
    private ErrorMessageContainer errorMessageContainer = default;
    /// <summary>
    /// List of specific error functionality for errors from controllers
    /// </summary>
    public List<Tuple<Error, Func<Error, Error.Occurrence.OccurrenceStatus, bool>>> errorTuples = new List<Tuple<Error, Func<Error, Error.Occurrence.OccurrenceStatus, bool>>>();
    /// <summary>
    /// Gets error controller
    /// </summary>
    public ErrorController ErrorController { get => errorController; }

    /// <summary>
    /// Binds errors from error raising actions into error tuples container
    /// </summary>
    public abstract void RefreshActionsForErrors();

    /// <summary>
    /// Invoke specific error actions
    /// </summary>
    /// <param name="errorCode">Error code</param>
    /// <param name="status">Current error status</param>
    protected abstract void ErrorInvoke(string errorCode, Error.Occurrence.OccurrenceStatus status);

    /// <summary>
    /// Unraises specific error
    /// </summary>
    /// <param name="errorCode">Error code to unraise</param>
    public virtual void UnraiseErrorCode(string errorCode)
    {
        try
        {
            if (ErrorController.HandleError(errorCode, errorMessageContainer.GetErrorMessage(errorCode), DateTime.Now, Error.Occurrence.OccurrenceStatus.Unraised))
                ErrorInvoke(errorCode, Error.Occurrence.OccurrenceStatus.Unraised);
        }
        catch (ErrorOperationException)
        {
            return;
        }
    }

    /// <summary>
    /// Resets specific error
    /// </summary>
    /// <param name="errorCode">Error code to reset</param>
    public virtual void ResetErrorCode(string errorCode)
    {
        try
        {
            if (ErrorController.HandleError(errorCode, errorMessageContainer.GetErrorMessage(errorCode), DateTime.Now, Error.Occurrence.OccurrenceStatus.Reset))
                ErrorInvoke(errorCode, Error.Occurrence.OccurrenceStatus.Reset);
        }
        catch (ErrorOperationException)
        {
            return;
        }
    }

    /// <summary>
    /// Raises specific error
    /// </summary>
    /// <param name="errorCode">Error code to raise</param>
    public virtual void RaiseErrorCode(string errorCode)
    {
        try
        {
            if (ErrorController.HandleError(errorCode, errorMessageContainer.GetErrorMessage(errorCode), DateTime.Now, Error.Occurrence.OccurrenceStatus.Raised))
                ErrorInvoke(errorCode, Error.Occurrence.OccurrenceStatus.Raised);
        }
        catch (ErrorOperationException)
        {
            return;
        }
    }

    /// <summary>
    /// Unraise all errors
    /// </summary>
    public virtual void UnraiseAllErrors()
    {
        for (int i = 0; i < errorMessageContainer.ErrorCodeMessages.Count; ++i)
        {
            UnraiseErrorCode(errorMessageContainer.ErrorCodeMessages[i].Code);
        }
    }

    /// <summary>
    /// Load errors messages from scriptable objects
    /// </summary>
    [ContextMenu("Load Error Messages")]
    public virtual void LoadErrorMessages()
    {
        ErrorController.ErrorList.Clear();
        for (int i = 0; i < errorMessageContainer.ErrorCodeMessages.Count; ++i)
        {
            ErrorController.ErrorList.Add(new Error(i.ToString(), errorMessageContainer.ErrorCodeMessages[i].Code, errorMessageContainer.ErrorCodeMessages[i].Message));
        }
    }
}
