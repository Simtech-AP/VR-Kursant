using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// Enumeration allowing switching image type
/// </summary>
public enum ImageEncoding
{
    JPG = 0,
    PNG = 1
}

/// <summary>
/// Main class used in sending texture to trainer application
/// </summary>
public class TextureSender : MonoBehaviour
{
    /// <summary>
    /// Source texture (from camera)
    /// </summary>
    Texture2D source;
    /// <summary>
    /// Listener used in sending texture
    /// </summary>
    private TcpListener listner;
    /// <summary>
    /// All connected clients receiving texture
    /// </summary>
    private List<TcpClient> clients = new List<TcpClient>();
    /// <summary>
    /// Should the sending stop?
    /// </summary>
    private bool stop = false;
    /// <summary>
    /// Port to send texture on
    /// </summary>
    public int port = 5000;
    /// <summary>
    /// Currently selected encoding
    /// </summary>
    public ImageEncoding encoding = ImageEncoding.JPG;
    /// <summary>
    /// Standard message byte length
    /// </summary>
    [Header("Must be the same in sender and receiver")]
    public int messageByteLength = 24;
    /// <summary>
    /// Are we ready to get new frame?
    /// </summary>
    bool readyToGetFrame = true;
    /// <summary>
    /// Template variable to store whole frame
    /// </summary>
    byte[] frameBytesLength;
    /// <summary>
    /// Received bytes in message
    /// </summary>
    byte[] imageBytes;

    /// <summary>
    /// Set up sending texture in background
    /// </summary>
    public void Initialize()    {
        Application.runInBackground = true;
        StartCoroutine(initAndWaitForTexture());
    }

    /// <summary>
    /// Set texture to send
    /// </summary>
    /// <param name="t">Texture to send</param>
    public void SetSourceTexture(Texture2D t)
    {
        source = t;
    }

    /// <summary>
    /// Clears new byte array and copies received data to it
    /// </summary>
    /// <param name="byteLength"></param>
    /// <param name="fullBytes"></param>
    private void byteLengthToFrameByteArray(int byteLength, byte[] fullBytes)
    {
        Array.Clear(fullBytes, 0, fullBytes.Length);
        byte[] bytesToSendCount = BitConverter.GetBytes(byteLength);
        bytesToSendCount.CopyTo(fullBytes, 0);
    }

    /// <summary>
    /// Starts listener and waits for texture
    /// </summary>
    /// <returns></returns>
    private IEnumerator initAndWaitForTexture()
    {
        while (source == null)
        {
            yield return null;
        }
        listner = new TcpListener(IPAddress.Any, port);
        listner.Start();
        StartCoroutine(senderCOR());
    }

    /// <summary>
    /// Sets up coroutine on end of a frame
    /// </summary>
    private WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

    /// <summary>
    /// Main send coroutine
    /// </summary>
    /// <returns>Wait time for next frame</returns>
    private IEnumerator senderCOR()
    {
        bool isConnected = false;
        TcpClient client = null;
        NetworkStream stream = null;
        Loom.RunAsync(() =>
        {
            while (!stop)
            {
                client = listner.AcceptTcpClient();
                clients.Add(client);
                isConnected = true;
                stream = client.GetStream();
            }
        });
        while (!isConnected)
        {
            yield return null;
        }
        readyToGetFrame = true;
        frameBytesLength = new byte[messageByteLength];
        while (!stop)
        {
            yield return endOfFrame;
            imageBytes = EncodeImage();
            byteLengthToFrameByteArray(imageBytes.Length, frameBytesLength);
            readyToGetFrame = false;
            Loom.RunAsync(() =>
            {
                stream.Write(frameBytesLength, 0, frameBytesLength.Length);
                stream.Write(imageBytes, 0, imageBytes.Length);
                readyToGetFrame = true;
            });
            while (!readyToGetFrame)
            {
                yield return null;
            }
        }
    }

    /// <summary>
    /// Encides sended image to JPG or PNG
    /// </summary>
    /// <returns></returns>
    private byte[] EncodeImage()
    {
        if (encoding == ImageEncoding.PNG) return source.EncodeToPNG();
        return source.EncodeToJPG();
    }

    /// <summary>
    /// Stops sending when application quits
    /// </summary>
    private void OnApplicationQuit()
    {
        if (listner != null)
        {
            listner.Stop();
        }
        foreach (TcpClient c in clients)
            c.Close();
    }
}