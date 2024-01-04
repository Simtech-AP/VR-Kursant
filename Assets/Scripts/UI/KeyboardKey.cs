using TMPro;
using UnityEngine;

/// <summary>
/// Class for representing a single keyboard key
/// </summary>
public class KeyboardKey : MonoBehaviour
{
    /// <summary>
    /// Reference to key over this key
    /// </summary>
    public KeyboardKey upKey;
    /// <summary>
    /// Reference to key below this key
    /// </summary>
    public KeyboardKey downKey;
    /// <summary>
    /// Reference to key to the right of this key
    /// </summary>
    public KeyboardKey rightKey;
    /// <summary>
    /// Reference to key to the left of this key
    /// </summary>
    public KeyboardKey leftKey;

    /// <summary>
    /// Gets a string which key is representing
    /// </summary>
    /// <returns>String used in entering input</returns>
    public string GetKeyString()
    {
        return GetComponentInChildren<TextMeshProUGUI>().text;
    }
}
