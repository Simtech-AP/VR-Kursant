using UnityEngine;
using DarkRift.Client.Unity;
using UnityEngine.Events;
using System.Net;
using DarkRift;
using DarkRift.Client;
using System;

/// <summary>
/// Controller for connection control including server listening and message sending
/// </summary>
public class ConnectionController : Controller
{
    /// <summary>
    /// Client used to connect to server
    /// </summary>
    [SerializeField]
    private UnityClient client = default;
    /// <summary>
    /// Is client currently connected?
    /// </summary>
    private bool connected;
    /// <summary>
    /// Stream controller used to create and send cemara data
    /// </summary>
    [SerializeField]
    private StreamController streamController = default;
    /// <summary>
    /// Callback used when finishing search for server
    /// </summary>
    private UnityEvent OnFinishedLookup = new UnityEvent();
    /// <summary>
    /// Callback used when disconnecting from server
    /// </summary>
    private UnityEvent OnDisconnected = new UnityEvent();
    /// <summary>
    /// Callback used when receiving message from server
    /// </summary>
    public Action<string> OnMessageRecieved = null;
    /// <summary>
    /// Current server IP
    /// </summary>
    private IPAddress serverIP;

    /// <summary>
    /// Main method used to searching and connecting to server
    /// </summary>
    public void ConnectToServerApplication()
    {
        OnFinishedLookup.AddListener(() => ConnectToServer(serverIP));
        SearchForServer(OnFinishedLookup);
    }

    /// <summary>
    /// Look up servers on local network and use callback when finished
    /// For debug purposes we currently are connecting to localhost
    /// </summary>
    /// <param name="callback">Callback method to call when finished lookup</param>
    private void SearchForServer(UnityEvent callback)
    {
        if (!client) return;
        //DEBUG-----
        serverIP = IPAddress.Parse("127.0.0.1");
        //----------
        callback.Invoke();
    }

    /// <summary>
    /// Allows to connect to specified server by ip and port
    /// </summary>
    /// <param name="ip">IP of thje server to connect to</param>
    /// <param name="port">Port of the server</param>
    private void ConnectToServer(IPAddress ip, ushort port = 4296)
    {
        if (!client) return;
        try
        {
            client.Connect(ip, port, true);
            OnDisconnected.AddListener(ResetConnectedState);
            client.Client.Disconnected += (object sender, DarkRift.Client.DisconnectedEventArgs e) => { OnDisconnected.Invoke(); };
            client.MessageReceived += ProcessMessage;
            SetUpStream();
            connected = true;
            StateModel.OnModuleChanged.AddListener(SendModuleChange);
            StateModel.OnScenarioChanged.AddListener(SendScenarioChange);
            StateModel.OnStepChanged.AddListener(SendStepChange);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// Process message sent by server
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Arguments of a message</param>
    private void ProcessMessage(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage() as Message)
        {
            using (DarkRiftReader reader = message.GetReader())
            {
                string data = reader.ReadString();
                OnMessageRecieved.Invoke(data);
            }
        }
    }

    /// <summary>
    /// Is client connected to server?
    /// </summary>
    /// <returns>Bool indicating if client is connected to server</returns>
    public bool IsConnected()
    {
        return connected;
    }

    /// <summary>
    /// Start stream for server
    /// </summary>
    private void SetUpStream()
    {
        streamController.SetUpSending();
    }

    /// <summary>
    /// Reset connected state when disconnecting
    /// </summary>
    private void ResetConnectedState()
    {
        StateModel.OnModuleChanged.RemoveListener(SendModuleChange);
        StateModel.OnScenarioChanged.RemoveListener(SendScenarioChange);
        StateModel.OnStepChanged.RemoveListener(SendStepChange);
        connected = false;
    }

    /// <summary>
    /// Sends message to server indicating change in module number
    /// </summary>
    /// <param name="module">Number of a module to send</param>
    public void SendModuleChange(Module module)
    {
        int moduleNumber = FindObjectOfType<ModuleController>().GetCurrentModuleNumber();
        DarkRiftWriter writer = DarkRiftWriter.Create();
        writer.Write("module" + moduleNumber);
        using (Message message = Message.Create(0, writer))
            client.SendMessage(message, SendMode.Reliable);
    }

    /// <summary>
    /// Sends message to server indicating change in scenario number
    /// </summary>
    /// <param name="scenario">Number of a scenario to send</param>
    public void SendScenarioChange(Scenario scenario)
    {
        int scenarioNumber = StateModel.currentModule.GetCurrentScenarioNumber();
        DarkRiftWriter writer = DarkRiftWriter.Create();
        writer.Write("scenario" + scenarioNumber);
        using (Message message = Message.Create(0, writer))
            client.SendMessage(message, SendMode.Reliable);
    }

    /// <summary>
    /// Sends message to server indicating change in step number
    /// </summary>
    /// <param name="step">Number of a step to send</param>
    public void SendStepChange(Step step)
    {
        int stepNumber = StateModel.currentScenario.GetCurrentStepNumber();
        DarkRiftWriter writer = DarkRiftWriter.Create();
        writer.Write("step" + stepNumber);
        using (Message message = Message.Create(0, writer))
            client.SendMessage(message, SendMode.Reliable);
    }

    /// <summary>
    /// Sends message to server that a module is finished by user
    /// </summary>
    public void SignalModuleFinished()
    {
        DarkRiftWriter writer = DarkRiftWriter.Create();
        writer.Write("finished");
        using (Message message = Message.Create(0, writer))
            client.SendMessage(message, SendMode.Reliable);
    }

    /// <summary>
    /// Sends message to server that user need assistance
    /// </summary>
    public void RequestAssistance()
    {
        DarkRiftWriter writer = DarkRiftWriter.Create();
        writer.Write("request");
        using (Message message = Message.Create(0, writer))
            client.SendMessage(message, SendMode.Reliable);
    }

    /// <summary>
    /// Sends full list of scenario titles to trainer application
    /// </summary>
    /// <param name="titlesMessage">Concatenated string containing all scenarios titles</param>
    public void SendScenariosTitles(string titlesMessage)
    {
        DarkRiftWriter writer = DarkRiftWriter.Create();
        writer.Write(titlesMessage);
        using (Message message = Message.Create(0, writer))
            client.SendMessage(message, SendMode.Reliable);
    }

    /// <summary>
    /// Sends list of languages to trainer application
    /// </summary>
    /// <param name="titlesMessage">Concatenated string containing all scenarios titles</param>
    public void SendLanguages(string languages)
    {
        DarkRiftWriter writer = DarkRiftWriter.Create();
        writer.Write(languages);
        using (Message message = Message.Create(0, writer))
            client.SendMessage(message, SendMode.Reliable);
    }

    /// <summary>
    /// Sends testing and debug data to traner application
    /// </summary>
    /// <param name="id">ID of debug data allowing to segregate and assign sent data</param>
    /// <param name="data">Contains data shown in trainer app</param>
    public void SendDebugData(int id, string data)
    {
        DarkRiftWriter writer = DarkRiftWriter.Create();
        writer.Write(id + "," + data);
        using (Message message = Message.Create(0, writer))
            client.SendMessage(message, SendMode.Reliable);
    }
}
