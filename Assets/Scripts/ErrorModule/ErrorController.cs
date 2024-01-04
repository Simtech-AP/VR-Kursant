using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller errors
/// </summary>
public abstract class ErrorController : Controller
{
    /// <summary>
    /// Error type 
    /// </summary>
    public enum ErrorType
    {
        CRUCIAL = 0,
        NORMAL = 1,
        ALARM = 2
    }
    /// <summary>
    /// List of errror for specific controller
    /// </summary>
    [SerializeField]
    protected List<Error> errorList;
    /// <summary>
    /// Errro List property
    /// </summary>
    public List<Error> ErrorList { get => errorList; }
    /// <summary>
    /// Error amount
    /// </summary>
    public int currentErrorCounter;
    /// <summary>
    /// Action invoked when an error has been activated
    /// </summary>
    public static Action<ErrorController, Error> OnErrorOccured;

    /// <summary>
    /// On Security errors additional action
    /// </summary>
    public static Action OnErrorReset;

    /// <summary>
    /// Called when script is loaded
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Links robot on scene to this object
    /// </summary>
    /// <param name="robot">Robot to link to</param>
    public virtual void LinkRobot(Robot robot) { }

    /// <summary>
    /// Tick method
    /// </summary>
    public virtual void Tick() { }

    /// <summary>
    /// Handle Error
    /// </summary>
    /// <param name="_code">Error code</param>
    /// <param name="_message">Error message</param>
    /// <param name="_dateTime">Error date time</param>
    /// <param name="status">Error status flag</param>
    public virtual bool HandleError(string _code, string _message, System.DateTime _dateTime, Error.Occurrence.OccurrenceStatus status)
    {
        if (!Exsits(_code))
        {
            throw new ErrorOperationException(string.Format("ERROR DOES NOT EXISTS IN DATABASE! CREATE ONE AND RESTART THE PROGRAM! INVALID ERROR CODE: {0}", _code));
        }
        else
        {
            Error errorItem = GetError(_code);
            if (errorItem.Occurences.Count > 1 && errorItem.Occurences[errorItem.Occurences.Count - 1].Status == status)
            {
                return false;
            }
            if (errorItem.AutomaticUnraisedFlag)
            {
                HandleErrorType(_code, _message, _dateTime, status, errorItem, ErrorType.NORMAL);
                return true;
            }
            else
            {
                HandleErrorType(_code, _message, _dateTime, status, errorItem, ErrorType.CRUCIAL);
                return true;
            }
        }

    }

    /// <summary>
    /// Handle Error
    /// </summary>
    /// <param name="_code">Error code</param>
    /// <param name="_message">Error message</param>
    /// <param name="_dateTime">Error date time</param>
    /// <param name="status">Error status flag</param>
    /// <param name="errorItem">Errror item</param>
    /// <param name="errorType">Error type</param>
    private void HandleErrorType(string _code, string _message, DateTime _dateTime, Error.Occurrence.OccurrenceStatus status, Error errorItem, ErrorType errorType)
    {
        if (errorType == ErrorType.NORMAL && errorItem.Occurences.Count > 1 && errorItem.Occurences[errorItem.Occurences.Count - 1].Status == Error.Occurrence.OccurrenceStatus.Unraised && status == Error.Occurrence.OccurrenceStatus.Raised)
        {
            OnErrorOccured?.Invoke(this, errorItem);
            return;
        }
        if ((!errorItem.IsActive() && status == 0) || (errorItem.IsActive() && status > 0))
        {
            IncrementErrorItems(status);
            errorItem.SetErrorMessage(_message);
            errorItem.AddOccurnace();
            errorItem.AddErrorDateTime(_dateTime);
            errorItem.SetStateWithOccurence(status);
            if (errorType == ErrorType.NORMAL)
            {
                errorItem.AddOccurnace();
                errorItem.AddErrorDateTime(_dateTime);
                errorItem.SetStateWithOccurence(Error.Occurrence.OccurrenceStatus.Unraised);
            }
            OnErrorOccured?.Invoke(this, errorItem);
        }
        else
        {
           throw new ErrorOperationException(string.Format("ERROR CODE: {0} HAS RAISED STATUS: {1}, ERROR CAN'T BE RAISED/UNRAISED AGAIN IF IT'S ALREADY RAISED/UNRAISED", _code, Convert.ToBoolean(status)));
        }
    }

    private void IncrementErrorItems(Error.Occurrence.OccurrenceStatus status)
    {
        currentErrorCounter += status == Error.Occurrence.OccurrenceStatus.Reset ? -1 : 0;
        currentErrorCounter += status == Error.Occurrence.OccurrenceStatus.Raised ? 1 : 0;
    }

    /// <summary>
    /// Check if controller can reset errors
    /// </summary>
    /// <returns>whether controller can reset all errors</returns>
    public bool CanResetErrorList()
    {
        for (int i = 0; i < errorList.Count; ++i)
        {
            Error.Occurrence ocur;
            ocur = errorList[i].GetLastOccurence();
            if (ocur != null)
            {
                if (ocur.Status == Error.Occurrence.OccurrenceStatus.Raised)
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Gets specific Error
    /// </summary>
    /// <param name="error">Error object</param>
    /// <returns>Error object</returns>
    public Error GetError(Error error)
    {
        Error er = null;
        for (int i = 0; i < errorList.Count; ++i)
        {
            if (errorList[i].Equals(error))
                er = errorList[i];
        }
        return er;
    }

    /// <summary>
    /// Gets specific error by code
    /// </summary>
    /// <param name="errorCode">Error code</param>
    /// <returns>Error object</returns>
    public Error GetError(string errorCode)
    {
        Error er = null;
        for (int i = 0; i < errorList.Count; ++i)
        {
            if (errorList[i].Equals(errorCode))
                er = errorList[i];
        }
        return er;
    }

    /// <summary>
    /// Check if error exisit
    /// </summary>
    /// <param name="errCode">Error code</param>
    /// <returns>Error object</returns>
    public bool Exsits(string errCode)
    {
        for (int i = 0; i < errorList.Count; ++i)
        {
            if (errorList[i].Equals(errCode))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Check if is any error active
    /// </summary>
    /// <returns>Whether there is any error active</returns>
    public bool IsAnyErrorActive()
    {
        for (int i = 0; i < errorList.Count; ++i)
        {
            if (errorList[i].Active)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if specified error is raised
    /// </summary>
    /// <param name="v">Code of error to check</param>
    /// <returns>State if the error is raised</returns>
    public bool IsErrorActive(string v)
    {
        Error error = null;
        for (int i = 0; i < errorList.Count; ++i)
        {
            if (errorList[i].Equals(v))
                error = errorList[i];
        }
        if (error != null)
        {
            return error.Active;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if errors are reset
    /// </summary>
    /// <returns>Wether errors are reset</returns>
    public bool AreErrorsReset()
    {
        for (int i = 0; i < errorList.Count; ++i)
        {
            Error.Occurrence ocur;
            ocur = errorList[i].GetLastOccurence();
            if (ocur != null)
            {
                if (ocur.Status == Error.Occurrence.OccurrenceStatus.Unraised || ocur.Status == Error.Occurrence.OccurrenceStatus.Raised)
                    return false;
            }
        }
        return currentErrorCounter == 0;
    }

    /// <summary>
    /// Reset all errors
    /// </summary>
    /// <returns></returns>
    public virtual bool ResetErrors()
    {
        if (CanResetErrorList())
        {
            currentErrorCounter = 0;
            for (int i = 0; i < errorList.Count; ++i)
            {
                Error.Occurrence ocur;
                ocur = errorList[i].GetLastOccurence();
                if (ocur != null)
                {
                    if (ocur.Status == Error.Occurrence.OccurrenceStatus.Unraised)
                    {
                        ResetError(i);
                    }
                }
            }
            return true;
        }
        else
        {
            Debug.LogError("SOME ERRORS ARE STILL NOT UNRAISED");
            return false;
        }
    }

    /// <summary>
    /// Resets specific error
    /// </summary>
    /// <param name="i">error index from list</param>
    protected void ResetError(int i)
    {
        errorList[i].AddOccurnace();
        errorList[i].AddErrorDateTime(DateTime.Now);
        errorList[i].SetStateWithOccurence(Error.Occurrence.OccurrenceStatus.Reset);
        errorList[i].OnResetEvent?.Invoke();
    }
}
