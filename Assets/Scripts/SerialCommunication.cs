using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class goverining logic for serial port communication
/// </summary>
public class SerialCommunication : MonoBehaviour
{
    /// <summary>
    /// Structure helping separation of concurrent data stream
    /// </summary>
    [System.Serializable]
    public struct MessageLength
    {
        /// <summary>
        /// ID of message to check
        /// </summary>
        public int id;
        /// <summary>
        /// Lenght of message with specified ID
        /// </summary>
        public int length;
    }

    /// <summary>
    /// Should we close connection to device?
    /// </summary>
    private bool closeConnection = false;
    /// <summary>
    /// Serial port to communicate through
    /// </summary>
    private SerialPort serial = new SerialPort();
    /// <summary>
    /// Message received from serial port device
    /// </summary>
    public byte[] message = new byte[10];
    /// <summary>
    /// List of messages and their length
    /// </summary>
    public List<MessageLength> messagesLength = new List<MessageLength>();
    /// <summary>
    /// Reference to network receiver for decrption
    /// </summary>
    public TcpReceiver receiver = default;

    /// <summary>
    /// Sets up new serial port connection
    /// </summary>
    private void Start()
    {
        var names = SerialPort.GetPortNames();
        names = names.OrderBy(x => x).ToArray();
    }

    /// <summary>
    /// Continously gets data from serial port
    /// </summary>
    private void Update()
    {
        if (serial.IsOpen)
        {
            message = new byte[serial.BytesToRead];
            if (serial.BytesToRead > 0)
            {
                try
                {
                    serial.BaseStream.Read(message, 0, serial.BytesToRead);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Serial error: " + e.Message);
                }
            }
            if (message.Length > 0 && message[0] != 0)
            {
                int index = 0;
                while (index < message.Length)
                {
                    bool found = false;
                    foreach (MessageLength ml in messagesLength)
                    {
                        if (ml.id == message[0])
                        {
                            ProcessMessage(message.Skip(index).Take(ml.length).ToArray());
                            index += ml.length;
                            found = true;
                        }
                        break;
                    }
                    if (!found)
                    {
                        index++;
                    }
                }
            }
            if (closeConnection)
                serial.Close();
        }
    }

    /// <summary>
    /// Processes message sending it to method identical to network message processor
    /// </summary>
    /// <param name="message"></param>
    private void ProcessMessage(byte[] message)
    {
        if (receiver) receiver.OnMessageArrived(message);
    }
}
