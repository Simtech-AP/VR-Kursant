using UnityEngine;
using TMPro;

/// <summary>
/// Class in charge of showing hints on pendant screen
/// </summary>
// TODO: Ask Piotrek and Bartek if we even need this one
public class PendantHint : MonoBehaviour
{
    /// <summary>
    /// Canvas for program instructions
    /// </summary>
    [SerializeField]
    private GameObject programCanvas = default;
    /// <summary>
    /// Canvas for hints
    /// </summary>
    [SerializeField]
    private GameObject hintCanvas = default;
    /// <summary>
    /// Main text object for hints
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI hintText = default;

    /// <summary>
    /// Sets text on screen to specified string
    /// </summary>
    /// <param name="hintText">Text to show on hint canvas</param>
    public void ShowHint(string hintText)
    {
        programCanvas.SetActive(false);
        hintCanvas.SetActive(true);
        this.hintText.text = hintText;
    }
}
