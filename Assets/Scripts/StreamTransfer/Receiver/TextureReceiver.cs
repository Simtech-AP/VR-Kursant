using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// Main class in charge of recieving and showing received texture
/// </summary>
public class TextureReceiver : MonoBehaviour
{
    /// <summary>
    /// Port number
    /// </summary>
    public int port = 5000;
    /// <summary>
    /// IP to connect to
    /// </summary>
    public string IP = "127.0.0.1";
    /// <summary>
    /// Default tcp client
    /// </summary>
    TcpClient client;
    /// <summary>
    /// Texture to show received image on
    /// </summary>
    [HideInInspector]
    public Texture2D texture;
    /// <summary>
    /// Should the receiving stop?
    /// </summary>
    private bool stop = false;
    /// <summary>
    /// Standard message byte length
    /// </summary>
    [Header("Must be the same in sender and receiver")]
    public int messageByteLength = 24;

    /// <summary>
    /// Initialize async Loomer method
    /// </summary>
    public void Initialize()    {
        Application.runInBackground = true;
        client = new TcpClient();
        Loom.RunAsync(() =>
        {
            client.Connect(IPAddress.Parse(IP), port);
            imageReceiver();
        });
    }

    /// <summary>
    /// Read incoming message
    /// </summary>
    private void imageReceiver()
    {
        Loom.RunAsync(() =>
        {
            while (!stop)
            {
                int imageSize = readImageByteSize(messageByteLength);
                readFrameByteArray(imageSize);
            }
        });
    }

    /// <summary>
    /// Convert length to readable Int32
    /// </summary>
    /// <param name="frameBytesLength"></param>
    /// <returns></returns>
    private int frameByteArrayToByteLength(byte[] frameBytesLength)
    {
        int byteLength = BitConverter.ToInt32(frameBytesLength, 0);
        return byteLength;
    }

    /// <summary>
    /// Read image and return its length
    /// </summary>
    /// <param name="size">Size of a message</param>
    /// <returns></returns>
    private int readImageByteSize(int size)
    {
        bool disconnected = false;
        NetworkStream serverStream = client.GetStream();
        byte[] imageBytesCount = new byte[size];
        var total = 0;
        do
        {
            var read = serverStream.Read(imageBytesCount, total, size - total);
            if (read == 0)
            {
                disconnected = true;
                break;
            }
            total += read;
        } while (total != size);
        int byteLength;
        if (disconnected)
        {
            byteLength = -1;
        }
        else
        {
            byteLength = frameByteArrayToByteLength(imageBytesCount);
        }
        return byteLength;
    }

    /// <summary>
    /// Read and load received image
    /// </summary>
    /// <param name="size">Size of an image</param>
    private void readFrameByteArray(int size)
    {
        bool disconnected = false;
        NetworkStream serverStream = client.GetStream();
        byte[] imageBytes = new byte[size];
        var total = 0;
        do
        {
            var read = serverStream.Read(imageBytes, total, size - total);
            if (read == 0)
            {
                disconnected = true;
                break;
            }
            total += read;
        } while (total != size);
        bool readyToReadAgain = false;
        if (!disconnected)
        {
            Loom.QueueOnMainThread(() =>
            {
                loadReceivedImage(imageBytes);
                readyToReadAgain = true;
            });
        }
        while (!readyToReadAgain)
        {
            System.Threading.Thread.Sleep(1);
        }
    }

    /// <summary>
    /// Show received image
    /// </summary>
    /// <param name="receivedImageBytes">Received image in bytes</param>
    private void loadReceivedImage(byte[] receivedImageBytes)
    {
        if (texture) texture.LoadImage(receivedImageBytes);
    }

    /// <summary>
    /// Set texture to show image on
    /// </summary>
    /// <param name="t"></param>
    public void SetTargetTexture(Texture2D t)
    {
        texture = t;
    }

    /// <summary>
    /// Stop reveiving when application exits
    /// </summary>
    private void OnApplicationQuit()
    {
        StopReceiving();
    }

    /// <summary>
    /// Stop receiving
    /// </summary>
    public void StopReceiving()
    {
        stop = true;
        if (client != null)
        {
            client.Close();
        }
    }
}