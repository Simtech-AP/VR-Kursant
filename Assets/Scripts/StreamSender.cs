using System.Collections;
using UnityEngine;

/// <summary>
/// Send camera to trainer application
/// </summary>
public class StreamSender : MonoBehaviour
{
    /// <summary>
    /// Camera from which image to send
    /// </summary>
    [SerializeField]
    private Camera cameraToSend = default;
    /// <summary>
    /// Texture Sender to process image form camera 
    /// </summary>
    [SerializeField]
    private TextureSender sender = default;
    /// <summary>
    /// Texture for temporary storage of camera image
    /// </summary>
    private Texture2D sendTexture;
    /// <summary>
    /// Is the sending initialized?
    /// </summary>
    private bool initialized;

    /// <summary>
    /// Start sending image using specified camera
    /// </summary>
    public void StartSending()
    {
        sendTexture = new Texture2D((int)cameraToSend.targetTexture.width, (int)cameraToSend.targetTexture.width);
        sender.SetSourceTexture(sendTexture);
        sender.Initialize();
        initialized = true;
        StartCoroutine(SendTexture());
    }

    /// <summary>
    /// Update camera texture every frame
    /// </summary>
    IEnumerator SendTexture()
    {
        while (initialized)
        {
            yield return new WaitForSeconds(0.15f);
            RenderTexture.active = cameraToSend.targetTexture;
            sendTexture.ReadPixels(new Rect(0, 0, cameraToSend.targetTexture.width, cameraToSend.targetTexture.height), 0, 0, false);
        }
    }
}
