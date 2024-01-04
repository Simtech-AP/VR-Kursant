using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;

/// <summary>
/// Error list class for control console screen
/// </summary>
public class ErrorList : MonoBehaviour
{
    /// <summary>
    /// Reference to text of error list
    /// </summary>
    [SerializeField]
    private Transform errorListContainer = default;
    /// <summary>
    /// Template object for every error shown on screen
    /// </summary>
    [SerializeField]
    private GameObject errorUIPrefab = default;
    /// <summary>
    /// All errors shown on UI
    /// </summary>
    private List<ErrorUI> errorsUI = default;
    /// <summary>
    /// Reference to full error screen rect
    /// </summary>
    [SerializeField]
    private RectTransform errorScreen = default;
    /// <summary>
    /// Reference to full camera screen rect
    /// </summary>
    [SerializeField]
    private RectTransform cameraScreen = default;
    /// <summary>
    /// Error screen hide local position
    /// </summary>
    [SerializeField]
    private Vector3 hidePosition = new Vector3(353, -200, -0.1f);
    /// <summary>
    /// Error screen position change duration time
    /// </summary>
    [SerializeField]
    private float duration = 0.3f;
    /// <summary>
    /// Text showing count of errors in UI
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI errorsAmount = default;
    /// <summary>
    /// Reference place to show new errors
    /// </summary>
    private Vector3 openPosition = Vector3.zero;
    /// <summary>
    /// References to each of error controller on scene
    /// </summary>
    private Dictionary<Type, int> controllersErrors = new Dictionary<Type, int>();

    /// <summary>
    /// Initalize open position for error screen
    /// </summary>
    private void Awake()
    {
        openPosition = errorScreen.transform.localPosition;
        errorsUI = new List<ErrorUI>();
    }

    /// <summary>
    /// Sets up error controllers to list
    /// </summary>
    private void Start()
    {
        controllersErrors.Add(FindObjectOfType<SecurityErrorController>().GetType(), 0);
        controllersErrors.Add(FindObjectOfType<RobotErrorController>().GetType(), 0);
        controllersErrors.Add(FindObjectOfType<AlarmErrorController>().GetType(), 0);
    }

    /// <summary>
    /// Binds methods to events
    /// </summary>
    private void OnEnable()
    {
        ErrorController.OnErrorOccured += AddErrorToText;
        AlarmErrorController.OnErrorReset += ResetAlarmText;
        RobotErrorController.OnErrorReset += ResetRobotText;
        SecurityErrorController.OnErrorReset += ResetSecurityText;
    }

    /// <summary>
    /// Unbinds methods form events
    /// </summary>
    private void OnDisable()
    {
        ErrorController.OnErrorOccured -= AddErrorToText;
        AlarmErrorController.OnErrorReset -= ResetAlarmText;
        RobotErrorController.OnErrorReset -= ResetRobotText;
        SecurityErrorController.OnErrorReset -= ResetSecurityText;
    }

    /// <summary>
    /// Resets text for specified security errors
    /// </summary>
    private void ResetSecurityText()
    {
        controllersErrors[typeof(SecurityErrorController)] = 0;
        DestroySpecficErrorsUI("S-");
        UpdateErrorCount();
    }

    /// <summary>
    /// Resets text for specified robot errors
    /// </summary>
    private void ResetRobotText()
    {
        controllersErrors[typeof(RobotErrorController)] = 0;
        DestroySpecficErrorsUI("R-");
        UpdateErrorCount();
    }

    /// <summary>
    /// Resets text for specified alarm errors
    /// </summary>
    private void ResetAlarmText()
    {
        controllersErrors[typeof(AlarmErrorController)] = 0;
        DestroySpecficErrorsUI("A-");
        UpdateErrorCount();
    }

    /// <summary>
    /// Clears specifie errors
    /// </summary>
    /// <param name="startCode">Code of errors to clear</param>
    private void DestroySpecficErrorsUI(string startCode)
    {
        var errors = errorsUI.FindAll(x => x.Code.StartsWith(startCode));
        if (errors.Count > 0)
            for (int i = 0; i < errors.Count; ++i)
            {
                var error = errors[i];
                errorsUI.Remove(error);
                Destroy(error.gameObject);
            }
    }

    /// <summary>
    /// Updates text error list with current error list
    /// </summary>
    /// <param name="errors">List of all errors</param>
    public void SetErrorListText(List<Error> errors)
    {
        for (int i = 0; i < errors.Count; ++i)
        {
            AddElement(errors[i]);
        }
    }

    /// <summary>
    /// Adds new error to list
    /// </summary>
    /// <param name="error">Error to add to list</param>
    private void AddElement(Error error)
    {
        var obj = Instantiate(errorUIPrefab, errorListContainer);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        UpdateError(error, obj.GetComponent<ErrorUI>());
        errorsUI.Add(obj.GetComponent<ErrorUI>());
    }

    /// <summary>
    /// Adds error to error text
    /// </summary>
    /// <param name="eController">Reference to Error Controller</param>
    /// <param name="error">Error object to add</param>
    public void AddErrorToText(ErrorController eController, Error error)
    {
        var searchedError = errorsUI.Find(x => x.Code.Equals(error.Code));
        if (!searchedError)
        {
            AddElement(error);
        }
        else
        {
            UpdateError(error, searchedError);
        }

        if (error.GetLastOccurence().Status == Error.Occurrence.OccurrenceStatus.Raised && eController as SecurityErrorController)
            ++controllersErrors[eController.GetType()];
        else if (eController as RobotErrorController || eController as AlarmErrorController)
            ++controllersErrors[eController.GetType()];

        UpdateErrorCount();
    }

    /// <summary>
    /// Updates specified error visual info
    /// </summary>
    /// <param name="error">Error to get infor from</param>
    /// <param name="searchedError">Visual error object</param>
    private void UpdateError(Error error, ErrorUI searchedError)
    {
        searchedError.Info.text = UpdateErrorText(error);
        searchedError.Status.text = GetStatusText(error);
        searchedError.Code = error.Code;
    }

    /// <summary>
    /// Updates text with current error counts
    /// </summary>
    private void UpdateErrorCount()
    {
        errorsAmount.text = "\n<color=red><b>" + "Robot Errors: " + controllersErrors[typeof(RobotErrorController)] + "</b></color>" +
            "\n<color=red><b>" + "Alarm Errors: " + controllersErrors[typeof(AlarmErrorController)] + "</b></color>" +
            "\n<color=red><b>" + "Security Errors: " + controllersErrors[typeof(SecurityErrorController)] + "</b></color>";
    }

    /// <summary>
    /// Parses error to text on console screen
    /// </summary>
    /// <param name="error">Error to parse</param>
    private string UpdateErrorText(Error error)
    {
        string currentText = string.Empty;
        currentText += "<#ffd899>Date: <i>" + error.GetLastOccurence().DateTime.ToString() + " </i></color> " +
                                " \n<color=yellow>Code: <b>" + error.Code + " </b></color>" +
                                " <#bbf4fa>Message: " + error.Message + " </color>";
        return currentText;
    }

    /// <summary>
    /// Depending on error type edit color of a text
    /// </summary>
    /// <param name="error">Error recieved</param>
    /// <returns>Processed string for error screen text</returns>
    private string GetStatusText(Error error)
    {
        string output = string.Empty;
        switch (error.GetLastOccurence().Status)
        {
            case Error.Occurrence.OccurrenceStatus.Raised:
                output += "<color=red>";
                break;
            case Error.Occurrence.OccurrenceStatus.Unraised:
                output += "<color=orange>";
                break;
            case Error.Occurrence.OccurrenceStatus.Reset:
                output += "<color=green>";
                break;
        }
        output += "Status: <b>" + error.GetLastOccurence().Status.ToString() + "</b></color>";
        return output;
    }

    /// <summary>
    /// Clears text from error list
    /// </summary>
    public void ClearErrorListText()
    {
        for (int i = 0; i < errorsUI.Count; ++i)
        {
            Destroy(errorsUI[i]);
        }
    }

    /// <summary>
    /// Enables or disables visibility of error list
    /// </summary>
    public void ToggleErrorList()
    {
        if (gameObject.activeInHierarchy)
        {
            HideScreen();
        }
        else
        {
            OpenScreen();
        }
    }

    /// <summary>
    /// Opens error list screen
    /// </summary>
    private void OpenScreen()
    {
        cameraScreen.DOScaleX(1f, duration / 2);
        gameObject.SetActive(true);
        errorScreen.DOLocalMove(openPosition, duration).OnUpdate(() =>
        {
            errorScreen.DOScale(1, duration / 2);
        });
    }

    /// <summary>
    /// Hides error list screen
    /// </summary>
    private void HideScreen()
    {
        errorScreen.DOLocalMove(hidePosition, duration).OnUpdate(() =>
        {
            errorScreen.DOScale(0, duration / 2);
            cameraScreen.DOScaleX(1.735f, duration / 2);
        }).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
