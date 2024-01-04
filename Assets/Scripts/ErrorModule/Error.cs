using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Error structure 
/// </summary>
[Serializable]
[PreferBinarySerialization]
public class Error
{
    /// <summary>
    /// Occurence class
    /// </summary>
    [Serializable]
    public class Occurrence
    {
        /// <summary>
        /// Enum for error occurence status
        /// </summary>
        [Serializable]
        public enum OccurrenceStatus { Raised = 0, Unraised = 1, Reset = 2 }
        /// <summary>
        /// Error time status
        /// </summary>
        [SerializeField]
        private string statusTime;
        /// <summary>
        /// Status variable
        /// </summary>
        [SerializeField]
        private OccurrenceStatus status;
        /// <summary>
        /// Date time of errror
        /// </summary>
        private DateTime dateTime;
        /// <summary>
        /// Date time property
        /// </summary>
        public DateTime DateTime { get => dateTime; private set => SetOccurenceDateTime(value); }
        /// <summary>
        /// Occurence status property
        /// </summary>
        public OccurrenceStatus Status { get => status; private set => status = value; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Occurrence()
        {
            statusTime = string.Empty;
        }

        /// <summary>
        /// Constructor with date time
        /// </summary>
        /// <param name="_dateTime">current date time</param>
        public Occurrence(DateTime _dateTime)
        {
            DateTime = _dateTime;
            statusTime = string.Empty;
        }

        /// <summary>
        /// Sets current occurence with specific date time
        /// </summary>
        /// <param name="_dateTime">current date time</param>
        public void SetOccurenceDateTime(DateTime _dateTime)
        {
            this.dateTime = _dateTime;
            this.statusTime += _dateTime.ToString();
        }

        /// <summary>
        /// Sets status of occurence error
        /// </summary>
        /// <param name="status">Status of activated error</param>
        public void SetOccurenceStatus(OccurrenceStatus status)
        {
            Status = status;
        }

    }

    /// <summary>
    /// Error ID
    /// </summary>
    [SerializeField]
    private string id;
    /// <summary>
    /// Error code
    /// </summary>
    [SerializeField]
    private string code;
    /// <summary>
    /// Error message
    /// </summary>
    [SerializeField]
    [Multiline]
    private string message;
    /// <summary>
    /// Flag of Error
    /// </summary>
    [SerializeField]
    private bool active;
    /// <summary>
    /// If true - raising errors will have "unraised" state (this WILL NOT call "On Un Raised Event", ONLY "On Raised Event")
    /// </summary>
    [SerializeField]
    private bool automaticUnraisedFlag = default;
    /// <summary>
    /// Specific error occurences
    /// </summary>
    [SerializeField]
    private List<Occurrence> occurences;

    /// <summary>
    /// Error message property
    /// </summary>
    public string Message { get => message; private set => SetErrorMessage(value); }
    /// <summary>
    /// Error ID property
    /// </summary>
    public string ID { get => id; private set => SetErrorID(value); }
    /// <summary>
    /// Error Active flag property
    /// </summary>
    public bool Active { get => active; private set => active = value; }
    /// <summary>
    /// Automatic Unraised flag property
    /// </summary>
    public bool AutomaticUnraisedFlag { get => automaticUnraisedFlag; private set => automaticUnraisedFlag = value; }
    /// <summary>
    /// Error Code property
    /// </summary>
    public string Code { get => code; private set => code = value; }
    /// <summary>
    /// Error occurences property
    /// </summary>
    public List<Occurrence> Occurences { get => occurences; private set => occurences = value; }

    /// <summary>
    /// All events that are registered to Error Raising callback
    /// </summary>
    [Space(10)]
    [Header("Raised actions")]
    public UnityEvent OnRaisedEvent;
    /// <summary>
    /// All events that are registered o Error Unraising callback
    /// </summary>
    [Space(10)]
    [Header("Unraised actions")]
    public UnityEvent OnUnRaisedEvent;
    /// <summary>
    /// All events that are registered o Error Reset callback
    /// </summary>
    [Space(10)]
    [Header("Reset actions")]
    public UnityEvent OnResetEvent;

    /// <summary>
    /// Error object constructor
    /// </summary>
    /// <param name="_id">Error id to set</param>
    /// <param name="_code">Error code to set</param>
    /// <param name="_message">Error message to set</param>
    /// <param name="_dateTime">Error date time to set</param>
    /// <param name="_active">Error activated flag to set</param>
    public Error(string _id, string _code, string _message, DateTime _dateTime, bool _active)
    {
        Message = _message;
        ID = _id;
        Active = _active;
        Code = _code;
        occurences = new List<Occurrence>();
        Occurences.Add(new Occurrence(_dateTime));
        AutomaticUnraisedFlag = false;
    }

    /// <summary>
    /// Error object constructor
    /// </summary>
    /// <param name="_id">Error id to set</param>
    /// <param name="_code">Error code to set</param>
    /// <param name="_message">Error message to set</param>
    public Error(string _id, string _code, string _message)
    {
        ID = _id;
        Code = _code;
        Message = _message;
        AutomaticUnraisedFlag = false;
    }

    /// <summary>
    /// Sets error message
    /// </summary>
    /// <param name="_message">Error information message</param>
    public void SetErrorMessage(string _message)
    {
        message = _message;
    }

    /// <summary>
    /// Sets error id
    /// </summary>
    /// <param name="_id">Error specification id</param>
    public void SetErrorID(string _id)
    {
        id = _id;
    }

    /// <summary>
    /// Sets error id
    /// </summary>
    /// <param name="_id">Error specification id</param>
    public void SetErrorID(int _id)
    {
        id = _id.ToString();
    }

    /// <summary>
    /// Adds new date time for exisiting error
    /// </summary>
    /// <param name="_dateTime">Date time of error behvaiour</param>
    public void AddErrorDateTime(DateTime _dateTime)
    {
        Occurences[Occurences.Count - 1].SetOccurenceDateTime(_dateTime);
    }

    /// <summary>
    /// Checks if error is active
    /// </summary>
    /// <returns>Error active status</returns>
    public bool IsActive()
    {
        return Active;
    }

    /// <summary>
    /// Sets error state with occurence
    /// </summary>
    /// <param name="status">Flag for error activation status</param>
    public void SetStateWithOccurence(Occurrence.OccurrenceStatus status)
    {
        active = status == 0 ? true : false;
        Occurences[Occurences.Count - 1].SetOccurenceStatus(status);
    }

    /// <summary>
    /// Sets error state with occurence
    /// </summary>
    /// <param name="status">Flag for error activation status</param>
    public void SetStateWithoutOccurence(Occurrence.OccurrenceStatus status)
    {
        active = status == 0 ? true : false;
    }

    /// <summary>
    /// Adds new error occurence
    /// </summary>
    public void AddOccurnace()
    {
        Occurences.Add(new Occurrence());
    }

    /// <summary>
    /// Gets last error occurence
    /// </summary>
    /// <returns>Last occurence of error object</returns>
    public Occurrence GetLastOccurence()
    {
        if (Occurences.Count > 0)
            return Occurences[Occurences.Count - 1];
        else
            return null;
    }

    /// <summary>
    /// Compares two objects
    /// </summary>
    /// <param name="obj">Object to compare with</param>
    /// <returns>Whether objects are equal</returns>
    public override bool Equals(object obj)
    {
        return code.Equals(((Error)obj).code);
    }

    /// <summary>
    /// Compares two errors by their code
    /// </summary>
    /// <param name="_code">Code to compare with</param>
    /// <returns>Whether objects are equal</returns>
    public bool Equals(string _code)
    {
        return code.Equals(_code);
    }

    /// <summary>
    /// Gets Hash Code of object
    /// </summary>
    /// <returns>Object hash code</returns>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <summary>
    /// Checks equality of two Errors
    /// </summary>
    /// <param name="e1">Left hand-side error</param>
    /// <param name="e2">Right hand-side error</param>
    /// <returns>Whether erros are equal</returns>
    public static bool operator ==(Error e1, Error e2)
    {
        if (ReferenceEquals(e1, null))
        {
            return ReferenceEquals(e2, null);
        }
        if (ReferenceEquals(e2, null))
        {
            return ReferenceEquals(e1, null);
        }
        return e1.Equals(e2);
    }

    /// <summary>
    /// Checks unequality of two Errors
    /// </summary>
    /// <param name="e1">Left hand-side error</param>
    /// <param name="e2">Right hand-side error</param>
    /// <returns>Whether erros are equal</returns>
    public static bool operator !=(Error e1, Error e2)
    {
        return !(e1 == e2);
    }
}
