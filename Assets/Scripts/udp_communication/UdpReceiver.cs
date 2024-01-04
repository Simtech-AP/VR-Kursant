using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

/// <summary>
/// Class allowing for receiving data from pendant
/// </summary>
public class UdpReceiver : MonoBehaviour
{
    /// <summary>
    /// Port of pendant connection
    /// </summary>
    public int port;
    /// <summary>
    /// Client object for receiving messages
    /// </summary>
    private UdpClient udpClient;
    /// <summary>
    /// Loomer used to run udp receiver on separate thread
    /// </summary>
    private Thread udpLoomer;

    /// <summary>
    /// Run UDP server on separate thread
    /// </summary>
    private void Start()
    {
        udpLoomer = Loom.RunAsync(() =>
        {
            listenerForTCP();
        });
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged += (x) =>
        {
            if (x == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                if (udpLoomer != null)
                    udpLoomer.Abort();
                if (udpClient != null)
                    udpClient.Close();
                Debug.Log(string.Format("[{0}] Exiting playmode.", GetType().Name));
            }
        };
#endif
    }

    /// <summary>
    /// Clean and stop udp server on quitting application
    /// </summary>
    private void OnApplicationQuit()
    {
        if (udpLoomer != null)
            udpLoomer.Abort();
        if (udpClient != null && udpClient.Client != null && udpClient.Client.Connected)
            udpClient.Close();
    }

    /// <summary>
    /// Main listener method
    /// Receive messages and run callbacks
    /// </summary>
    private void listenerForTCP()
    {
        Loom.QueueOnMainThread(() => FindObjectOfType<TcpReceiver>().ConnectionStatus(true));
        udpClient = new UdpClient();
        IPEndPoint localEp = new IPEndPoint(IPAddress.Any, port);
        try
        {
            udpClient.Client.Bind(localEp);
        }
        catch (Exception err)
        {
            Loom.QueueOnMainThread(new Action(() => Debug.Log(err.Message)));
        }
        IPAddress multicastaddress = IPAddress.Parse("224.0.0.1");
        udpClient.JoinMulticastGroup(multicastaddress);

        while (true)
        {
            byte[] robmessage = null;
            try
            {
                robmessage = udpClient.Receive(ref localEp);

            }
            catch (Exception err)
            {
                Debug.Log("recieve data error " + err);
                if (err.Message.Contains("disposed")) break;
            }
            if (robmessage != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    var tcp = FindObjectOfType<TcpReceiver>();
                    if (tcp.pilotIpEnd == 0)
                    {
                        tcp.StartListening(localEp.Address.ToString());
                    }
                    else
                    {
                        if (tcp.pilotIpEnd == robmessage[1])
                        {
                            tcp.StartListening(localEp.Address.ToString());
                        }
                    }
                });
            }
        }
    }
}