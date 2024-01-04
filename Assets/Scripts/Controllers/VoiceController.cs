using Adrenak.UniVoice;
using UnityEngine;

/// <summary>
/// Controls sending and connecting to voice server
/// </summary>
public class VoiceController : Controller
{
    /// <summary>
    /// Source used in transmitting voice from trainer application
    /// </summary>
    [SerializeField]
    private AudioSource voiceSource = default;
    /// <summary>
    /// Main voice object for transmitting
    /// </summary>
    private Voice voice;

    /// <summary>
    /// Connects to voice server 
    /// </summary>
    [ContextMenu("Create")]
    public void ConnectVoice()
    {
        voice = Voice.New(voiceSource);
        voice.Create("Simtech", status => Debug.Log("Room creation status: " + status));
    }

    /// <summary>
    /// Starts transmitting voice to trainer application
    /// </summary>
    [ContextMenu("Start")]
    public void StartSending()
    {
        voice.Speaking = true;
    }

    /// <summary>
    /// Stops sending voice to trainer application
    /// </summary>
    [ContextMenu("Stop")]
    public void StopSending()
    {
        voice.Speaking = false;
    }
}
