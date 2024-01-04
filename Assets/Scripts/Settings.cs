using UnityEngine;

/// <summary>
/// Data container for settings of an application
/// </summary>
[CreateAssetMenu(menuName = "Settings")]
public class Settings : ScriptableObject
{
    /// <summary>
    /// Volume of all sounds
    /// </summary>
    public float volume;
}
