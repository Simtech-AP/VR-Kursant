using UnityEngine.Events;

/// <summary>
/// Single bind allowing to process input and interact with application
/// </summary>
[System.Serializable]
public class InputBind
{
    /// <summary>
    /// Name of an input, doubles as an input id
    /// </summary>
    public string inputName;
    /// <summary>
    /// Event invoked when input was called
    /// </summary>
    public UnityEvent OnPress;
    /// <summary>
    /// Event invoked when input is released
    /// </summary>
    public UnityEvent OnRelease;
}
