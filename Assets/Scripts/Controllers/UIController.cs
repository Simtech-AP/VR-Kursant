using UnityEngine;
using TMPro;

/// <summary>
/// User interface controller in charge of flat interface
/// </summary>
public class UIController : Controller
{
    /// <summary>
    /// Object indicating that trainer is watching
    /// </summary>
    [SerializeField]
    private GameObject watchCanvas = default;
    /// <summary>
    /// Text showing currently loaded module
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI moduleText = default;
    /// <summary>
    /// Continue indicator visible when possible to go to next step
    /// </summary>
    [SerializeField]
    private GameObject continueIndicator = default;

    /// <summary>
    /// Enable or disable watching indicator
    /// </summary>
    /// <param name="isOn">Should the indicator be on?</param>
    public void SetWatchingIndicator(bool isOn)
    {
        watchCanvas.SetActive(isOn);
    }

    /// <summary>
    /// Update currently shown module number
    /// </summary>
    /// <param name="moduleNumber">Updated module number</param>
    public void SetModuleText(int moduleNumber)
    {
        moduleText.text = "Moduł " + moduleNumber;
    }

    /// <summary>
    /// Enables continuation indicator
    /// </summary>
    public void EnableContinueIndicator()
    {
        continueIndicator.SetActive(true);
    }

    /// <summary>
    /// Disables continuation indicator
    /// </summary>
    public void DisableContinueIndicator()
    {
        continueIndicator.SetActive(false);
    }
}
