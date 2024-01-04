using System;

/// <summary>
/// Handler for alarm error controller
/// </summary>
public class AlarmErrorHandler : ErrorHandler
{
    /// <summary>
    /// Called when script is loaded
    /// </summary>
    private void Awake()
    {
        RefreshActionsForErrors();
    }

    /// <summary>
    /// Binds errors from error raising actions into error tuples container
    /// </summary>
    public override void RefreshActionsForErrors()
    {
        foreach (Error error in ErrorController.ErrorList)
        {
            errorTuples.Add(new Tuple<Error, Func<Error, Error.Occurrence.OccurrenceStatus, bool>>(
                                                                error,
                                                                (psdError, flag) =>
                                                                {
                                                                    //custom implementation from robot point of view
                                                                    switch(flag)
                                                                    {
                                                                        case Error.Occurrence.OccurrenceStatus.Raised:
                                                                            psdError.OnRaisedEvent?.Invoke();
                                                                            break;
                                                                        case Error.Occurrence.OccurrenceStatus.Reset:
                                                                            psdError.OnResetEvent?.Invoke();
                                                                            break;
                                                                        case Error.Occurrence.OccurrenceStatus.Unraised:
                                                                            psdError.OnUnRaisedEvent?.Invoke();
                                                                            break;
                                                                    }
                                                                    return flag == 0;
                                                                }
                                                                ));
        }
    }

    /// <summary>
    /// Invoke specific error actions
    /// </summary>
    /// <param name="errorCode">Error code</param>
    /// <param name="errorstatus">Error actiavation flag</param>
    protected override void ErrorInvoke(string errorCode, Error.Occurrence.OccurrenceStatus errorstatus)
    {
        Error error = ErrorController.GetError(errorCode);
        var tuple = errorTuples.Find(x => x.Item1 == error);
        tuple?.Item2.Invoke(error, errorstatus);
    }
}
