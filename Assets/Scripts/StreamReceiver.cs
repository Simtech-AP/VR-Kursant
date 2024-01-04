using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Allows recieving of other trainees screen
/// </summary>
public class StreamReceiver : MonoBehaviour
{
    /// <summary>
    /// Main image on scene to receive screen
    /// </summary>
    [SerializeField]
    private RawImage streamImage = default;
    /// <summary>
    /// Texture receiver to process received image
    /// </summary>
    [SerializeField]
    private TextureReceiver receiver = default;

    /// <summary>
    /// Start receiving image from other trainee
    /// </summary>
    /// <param name="ip">IP of other trainee</param>
    /// <param name="port">Specified port of senders client</param>
    public void StartReceiving(string ip, int port)
    {
        receiver.IP = ip;
        receiver.port = port;
        receiver.SetTargetTexture((Texture2D)streamImage.texture);
        receiver.Initialize();
    }

    /// <summary>
    /// Stop receiving screen
    /// </summary>
    public void StopReceiving()
    {
        receiver.StopReceiving();
        receiver.texture = new Texture2D(1, 1);
    }
}
