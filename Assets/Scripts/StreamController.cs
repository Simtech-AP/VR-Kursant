using UnityEngine;

/// <summary>
/// Controls streaming of a screen to trainer application
/// </summary>
public class StreamController : MonoBehaviour
{
    /// <summary>
    /// Sender object to use
    /// </summary>
    [SerializeField]
    private StreamSender streamSender = default;
    /// <summary>
    /// Reciever object, can e used to show other trainees screens
    /// </summary>
    [SerializeField]
    private StreamReceiver streamReceiver = default;

    /// <summary>
    /// Sets up sending to trainer application
    /// </summary>
    public void SetUpSending()
    {
        streamSender.StartSending();
    }

    /// <summary>
    /// Start receiving camera image from specified ip
    /// </summary>
    /// <param name="ip">Provided IP</param>
    public void StartReceiving(string ip)
    {
        streamReceiver.StartReceiving(ip, 8001);
    }

    /// <summary>
    /// Stop receiving screen from other trainee
    /// </summary>
    public void StopReceiving()
    {
        streamReceiver.StopReceiving();
    }
}
