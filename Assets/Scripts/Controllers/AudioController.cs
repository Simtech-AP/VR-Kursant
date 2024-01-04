using UnityEngine;

/// <summary>
/// Controller allowing to control audio and sounds on scene
/// </summary>
public class AudioController : Controller
{
    /// <summary>
    /// Sound of a hall ambience
    /// </summary>
    [SerializeField]
    private AudioSource ambience = default;

    /// <summary>
    /// Used to start audio when starting application
    /// </summary>
    void Start()
    {
        ambience.Play();
    }
}
