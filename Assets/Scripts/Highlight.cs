using UnityEngine;

/// <summary>
/// Component allowing object to be highlighted
/// </summary>
public class Highlight : MonoBehaviour
{
    /// <summary>
    /// Highlight object to enable
    /// </summary>
    [SerializeField]
    private GameObject highlight = default;

    /// <summary>
    /// Switch highlight according to bool
    /// </summary>
    /// <param name="isOn">Should the highlight be on?</param>
    public void SetHighlight(bool isOn)
    {
        highlight.SetActive(isOn);
    }
}
