using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Error message container data holder
/// </summary>
[CreateAssetMenu(fileName = "ErrorContainer", menuName = "ErrorDictionary/ErrorContainer", order = 1)]
[PreferBinarySerialization]
public class ErrorMessageContainer : ScriptableObject
{
    /// <summary>
    /// Error item class
    /// </summary>
    [System.Serializable]
    public class ErrorItem
    {
        /// <summary>
        /// Error code
        /// </summary>
        public string Code;
        /// <summary>
        /// Error messages
        /// </summary>
        [Multiline]
        public string Message;
    }
    /// <summary>
    /// List of errors
    /// </summary>
    [SerializeField]
    private List<ErrorItem> errorCodeMessages;
    /// <summary>
    /// Property for error list
    /// </summary>
    public List<ErrorItem> ErrorCodeMessages { get => errorCodeMessages; private set => errorCodeMessages = value; }

    /// <summary>
    /// Get message for error
    /// </summary>
    /// <param name="errorCode">Error code</param>
    /// <returns>Gets message of error</returns>
    public string GetErrorMessage(string errorCode)
    {
        string message = string.Empty;
        ErrorItem errorItem = ErrorCodeMessages.Find(x => x.Code == errorCode);
        if (errorItem != null)
        {
            return errorItem.Message;
        }
        else
        {
            Debug.LogError("NO ERROR WITH SPECIFIED CODE IN MESSAGE CONTAINER : " + errorCode);
        }
        return message;
    }
}
