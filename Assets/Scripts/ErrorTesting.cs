using UnityEngine;

/// <summary>
/// Debug class for error testing
/// </summary>
public class ErrorTesting : MonoBehaviour
{
    /// <summary>
    /// Starts asynchronous error testing
    /// </summary>
    private async void Start()
    {
        await System.Threading.Tasks.Task.Delay(4000);

        ErrorRequester.RaiseError("R-1001");
        ErrorRequester.RaiseError("R-1002");
        await System.Threading.Tasks.Task.Delay(4000);
        ErrorRequester.ResetError("R-1001");
        ErrorRequester.ResetError("R-1002");
        await System.Threading.Tasks.Task.Delay(4000);

        Debug.Log("Reseting errors");
        ErrorRequester.ResetAllRobotErrors();

        Debug.Log("Saftey error");
        ErrorRequester.RaiseError("S-1001");
        await System.Threading.Tasks.Task.Delay(4000);
        ErrorRequester.UnraiseError("S-1001");
        await System.Threading.Tasks.Task.Delay(4000);
        ErrorRequester.ResetError("S-1001");

        ErrorRequester.ResetAllSecurityErrors();
        ErrorRequester.ResetAllRobotErrors();
    }
}
