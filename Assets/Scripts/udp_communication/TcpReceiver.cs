using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Collections;

/// <summary>
/// Structure to hold messages bound to input messages buttons
/// </summary>
[Serializable]
public struct MessageBind
{
    public int buttonId;
    public string button;
}

/// <summary>
/// Class allowing for receiving data from pendant
/// </summary>
public class TcpReceiver : MonoBehaviour
{
    /// <summary>
    /// Ip adress of pendant
    /// </summary>
    [HideInInspector]
    public string ip;
    /// <summary>
    /// Port of pendant connection
    /// </summary>
    public int port;
    /// <summary>
    /// Client object for receiving messages
    /// </summary>
    private TcpClient tcpClient;
    /// <summary>
    /// Received message
    /// </summary>
    public static byte[] message;
    /// <summary>
    /// List of reveived messages to process
    /// </summary>
    public static List<byte[]> messages;
    /// <summary>
    /// List of bound inputs to send to application
    /// </summary>
    public List<MessageBind> messagesToButtons = new List<MessageBind>();
    /// <summary>
    /// Reference to input controller
    /// </summary>
    private InputController inputController;
    /// <summary>
    /// Loomer used to run udp receiver on separate thread
    /// </summary>
    private Thread tcpLoomer;
    /// <summary>
    /// Reference to object with logic for touch screen
    /// </summary>
    private TouchSerial touchSerial;
    /// <summary>
    /// Reference to object with logic for touch sensors
    /// </summary>
    private TouchSensors touchSensors;
    /// <summary>
    /// Is the pendant connected?
    /// </summary>
    private bool connected = false;
    /// <summary>
    /// Is the pendant in error?
    /// </summary>
    private bool connectionError = false;
    /// <summary>
    /// Reference to UI object with connection error message
    /// </summary>
    [SerializeField]
    private GameObject connectionErrorUI = default;
    /// <summary>
    /// End of IP of pendant
    /// </summary>
    public int pilotIpEnd = 0;
    /// <summary>
    /// Are we using the pendant with touch screen?
    /// </summary>
    public bool touchScreenPendant = true;

    /// <summary>
    /// Sets up default values and references
    /// </summary>
    private void Start()
    {
        messages = new List<byte[]>();
        message = new byte[17];
        inputController = FindObjectOfType<InputController>();
        touchSerial = FindObjectOfType<TouchSerial>();
        touchSensors = FindObjectOfType<TouchSensors>();
    }

    /// <summary>
    /// Get reference for input controller, touch sensors and screen objects
    /// Set up listeners
    /// Run TCP server on separate thread
    /// </summary>
    public void StartListening(string ip)
    {
        if (connected) return;
        Debug.Log("Detected pendant IP: " + ip + ". Attempting to establish connection...");
        this.ip = ip;
        tcpClient = new TcpClient();
        tcpClient.ReceiveTimeout = 2500;
        tcpClient.Connect(IPAddress.Parse(ip), port);
        connected = true;
        tcpLoomer = Loom.RunAsync(() =>
        {
            listener();
        });
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged += (x) =>
        {
            if (x == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                if (tcpLoomer != null)
                    tcpLoomer.Abort();
                if (tcpClient != null)
                    tcpClient.Close();
                Debug.Log(string.Format("[{0}] Exiting playmode.", GetType().Name));
            }
        };
#endif
    }

    /// <summary>
    /// Clean and stop TCP server on quitting application
    /// </summary>
    private void OnApplicationQuit()
    {
        if (tcpClient != null)
            tcpClient.Close();
        if (tcpLoomer != null)
            tcpLoomer.Abort();
        connected = false;
    }

    /// <summary>
    /// Main listener method
    /// Receive messages and run callbacks
    /// </summary>
    private void listener()
    {
        byte[] data = new byte[2048];
        var mes = Encoding.UTF8.GetBytes("SimtechHELLO\r\n");
        try
        {
            tcpClient.Client.Send(mes);
        }
        catch (Exception E)
        {
            Loom.QueueOnMainThread(() => Debug.Log(E.Message));
        }
        while (true)
        {
            data = new byte[17];
            try
            {
                int result = tcpClient.Client.Receive(data);
            }
            catch (Exception err)
            {
                Loom.QueueOnMainThread(() => ConnectionStatus(true));
                connectionError = true;
                Loom.QueueOnMainThread(() => OnApplicationQuit());
                Loom.QueueOnMainThread(() => Debug.Log("recieve data error " + err));
                if (err.Message.Contains("disposed")) break;
                break;
            }
            if (data != new byte[17])
            {
                message = data;
                messages.Add(data);

                if (connectionError)
                {
                    Loom.QueueOnMainThread(() => ConnectionStatus(false));
                    connectionError = false;
                }
            }
        }
        Loom.QueueOnMainThread(() => Debug.Log("finihsed loop"));
        Loom.QueueOnMainThread(() => ConnectionStatus(true));
        connectionError = true;
        OnApplicationQuit();
        Loom.QueueOnMainThread(() => OnApplicationQuit());
    }

    /// <summary>
    /// Sets connection status and UI according to it
    /// </summary>
    /// <param name="error">Is the pendant in error?</param>
    public void ConnectionStatus(bool error)
    {
        connectionError = error;
        connectionErrorUI.SetActive(error);
        if (error && RobotData.Instance.MovementMode != MovementMode.AUTO)
            ErrorRequester.RaiseError("A-1001");
    }

    /// <summary>
    /// Checks if there are any messages to process
    /// </summary>
    private void Update()
    {
        if (messages.Count > 0)
        {
            try
            {
                ProcessMessage(messages[0]);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                // Debug.LogError("First message in queue is null, removing");
            }
            messages.RemoveAt(0);
        }
    }

    public void SendChargingMessage()
    {
        tcpClient.Client.Send(new byte[] { 30, 30, (byte)'\r' });
    }

    /// <summary>
    /// Processes received message
    /// </summary>
    /// <param name="message">Byte message received</param>
    private void ProcessMessage(byte[] message)
    {
        switch (message[0])
        {
            case 3:
            case 4:
                if (touchScreenPendant)
                {
                    touchSerial.OnMessageArrived(
                    new TouchScreenState()
                    {
                        X = BitConverter.ToInt16(message.ToArray(), 1),
                        Y = BitConverter.ToInt16(message.ToArray(), 3),
                        leftHand = message[0] == 3 ? false : true,
                        touching = true
                    });
                }
                break;
            case 5:
            case 6:
                if (touchScreenPendant)
                {
                    touchSerial.OnMessageArrived(
                    new TouchScreenState()
                    {
                        leftHand = message[0] == 5 ? false : true,
                        touching = false
                    });
                }
                break;
            case 9:
                FindObjectOfType<PendantUI>().ShowPendantBatteryLevel(message[1]);
                ControllersManager.Instance.GetController<TrackerController>().PerformChargingDecision(message[1]);
                break;
            case 20:
            case 21:
            case 22:
            case 23:
            case 24:
                FindObjectOfType<ConnectionController>().SendDebugData(message[0], Encoding.UTF8.GetString(message.ToArray(), 1, message.ToArray().Length - 2));
                break;
            case 240:
            case 241:
            case 242:
            case 243:
            case 244:
            case 245:
            case 246:
            case 247:
            case 248:
            case 249:
            case 1:
            case 2:
            case 10:
                ProcessSensorsAndButtons(message.ToArray());
                break;
        }
    }

    /// <summary>
    /// Activates proper methods to account for sensors and buttons 
    /// </summary>
    /// <param name="data">Message sent by pendant with sensors and buttonsdata</param>
    private void ProcessSensorsAndButtons(byte[] data)
    {
        if (data[0] != 10)
        {
            for (int i = 0; i < data.Length; i += 3)
            {
                foreach (MessageBind mb in messagesToButtons)
                {
                    if (data[i + 1] == mb.buttonId)
                    {
                        if (data[i] == 1)
                        {
                            inputController.UseButton(mb.button, true);
                        }
                        else if (data[i] == 2)
                        {
                            inputController.UseButton(mb.button, false);
                        }
                    }
                }
            }
        }
        else
        {
            touchSensors.ChangeSensorState(data);
        }
    }

    /// <summary>
    /// Message arrived method called when serial frames are sent
    /// </summary>
    /// <param name="data">Message sent by pendant connected on serial</param>
    public void OnMessageArrived(byte[] data)
    {
        message = data;
        ProcessMessage(message);
        ConnectionStatus(false);
        connectionError = false;
    }

    /// <summary>
    /// Sends messages to pendant
    /// </summary>
    /// <param name="id">ID of message to send</param>
    public void SendMessageId(int id)
    {
        tcpClient.Client.Send(new byte[] { (byte)id });
    }
}