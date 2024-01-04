using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

/// <summary>
/// Class handling all user interface visualization
/// </summary>
public class PendantUI : MonoBehaviour
{
    /// <summary>
    /// Reference to connection controller
    /// </summary>
    [SerializeField]
    private ConnectionController connectionController;
    private FlowController flowController;
    /// <summary>
    /// Robot movement speed shown on a pendant screen
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI movementSpeed = default;
    /// <summary>
    /// Robot movement type shown on a pendant screen
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI movementType = default;
    /// <summary>
    /// Robot movement mode shown on pendant screen
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI movementMode = default;
    /// <summary>
    /// Program name lodaed to edit on pendant
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI editedProgramName = default;
    /// <summary>
    /// Text displaying currently running line of program on robot
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI currentlyRunningLine = default;
    /// <summary>
    /// Current status of loaded program
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI programStatus = default;
    /// <summary>
    /// Message box available on a pendant, shows messages from trainer
    /// </summary>
    [SerializeField]
    private GameObject messageBox = default;
    [SerializeField]
    private GameObject mainScreenBox = default;
    /// <summary>
    /// Program text shown on a pendant screen
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI program = default;
    /// <summary>
    /// Alternative text used for markdown of instructions and parts
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI programSelect = default;
    /// <summary>
    /// Error text handler shown at the top of the pendant screen
    /// </summary>
    [SerializeField]
    private TimedMessageHandler feebackHandler = default;
    /// <summary>
    /// Text used to display current point TCP positions
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI pointPositions = default;
    /// <summary>
    /// Text used to display orientation position of TCP setup
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI orientationPositions = default;
    /// <summary>
    /// Text used to show direct TCP position
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI directPositions = default;
    /// <summary>
    /// Current status of ccordinates shown on pentant UI
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI cooridantesStatus = default;
    [SerializeField]
    private TextMeshProUGUI programList = default;
    /// <summary>
    /// Slider showing current battery percentage
    /// </summary>
    [SerializeField]
    private Slider batteryLevelSlider = default;
    /// <summary>
    /// Fill of battery slider, used to show color changing when battery discharges
    /// </summary>
    [SerializeField]
    private Image pendantBatteryFill = default;
    [SerializeField] private Image trackerBatteryFill = default;

    [SerializeField] private Image trackerChargingIcon = default;
    [SerializeField] private TextMeshProUGUI trackerBatteryText = default;
    [SerializeField] private TextMeshProUGUI pendantBatteryText = default;
    [SerializeField] private Image trackerConnectionIcon = default;


    [SerializeField]
    private TextMeshProUGUI stepMode = default;
    /// <summary>
    /// Text of currently edited program without markups
    /// </summary>
    private string editedProgramText = default;
    /// <summary>
    /// Current starting index of instruction text
    /// </summary>
    //TODO: Not so good, should be hidden or fully dynamic
    public int TextStartIndex { get; set; } = 0;
    /// <summary>
    /// Slider showing current tracker battery percentage
    /// </summary>
    [SerializeField]
    private Slider trackerLevelSlider = default;
    [SerializeField] private ProgramDebugMarkerHandler debugMarkerHandler;

    public List<NamedTMPItem> TMPInputFields;

    /// <summary>
    /// Get referencecs to controllers
    /// Binds updating UI with correct events
    /// </summary>
    private void Start()
    {
        flowController = FindObjectOfType<FlowController>();
        connectionController = ControllersManager.Instance.GetController<ConnectionController>();
        connectionController.OnMessageRecieved += ShowMessage;
        PendantData.OnUpdatedData += UpdateUI;
        PendantData.OnUpdatedData += UpdateIndexVisual;
        PendantData.OnUpdatedData += UpdateProgramText;
        PendantData.OnUpdatedData += UpdateProgramList;
        RobotData.OnUpdatedData += UpdateTCPPositions;
        RobotData.OnUpdatedData += UpdateTCPOrientations;
        RobotData.OnUpdatedData += UpdateTCPDirect;
        RobotData.OnUpdatedData += UpdateUI;
        RobotData.OnUpdatedData += UpdateCoords;
        RobotData.OnUpdatedData += debugMarkerHandler.OnRobotDataUpdate;

        ResetCanvas();
    }

    public void ResetCanvas()
    {
        // Hack around the bug that causes menu and program list to not open on first button click
        mainScreenBox.SetActive(false);
        mainScreenBox.SetActive(true);
    }


    private void UpdateProgramList(PendantData obj)
    {
        var programs = obj.SavedPrograms;
        string output = string.Empty;
        for (int i = 0; i < programs.Count; ++i)
        {
            output += programs[i].Name + "  " + programs[i].Description + "\n";
        }
        programList.text = output;
    }

    /// <summary>
    /// Sets visual slider to show battery level of pendant
    /// And text percentages as well
    /// </summary>
    public void ShowPendantBatteryLevel(int level)
    {
        batteryLevelSlider.value = level / 100f;
        pendantBatteryFill.color = GetBatteryBarColor(level);
        pendantBatteryText.text = (level).ToString("0");
    }

    public void ShowTrackerBatteryLevel(int level)
    {
        trackerLevelSlider.value = level / 100f;
        trackerBatteryFill.color = GetBatteryBarColor(level);
        trackerBatteryText.text = (level).ToString("0");
    }

    private Color GetBatteryBarColor(float forValue)
    {
        float r = 0.7f - (forValue / 100f) * 0.4f;
        float g = 0.3f + (forValue / 100f) * 0.4f;
        Color c = new Color(r, g, 0.3f, 1);
        return c;
    }

    public void ShowTrackerConnectionError(bool status)
    {
        trackerConnectionIcon.enabled = status;
    }

    /// <summary>
    /// Shows message sent by trainer
    /// </summary>
    /// <param name="message">String containing message</param>
    public void ShowMessage(string message)
    {
        if (message.Contains("message"))
        {
            messageBox.transform.DOScaleX(1f, 0.4f);
            messageBox.GetComponentInChildren<TextMeshProUGUI>().text = message.Remove(0, 8);
        }
    }

    internal void ShowTrackerCharging(bool chargingStatus)
    {
        trackerChargingIcon.enabled = chargingStatus;
    }

    /// <summary>
    /// Closes message box
    /// </summary>
    public void CloseMessage()
    {
        messageBox.transform.DOScaleX(0f, 0.4f);
    }

    /// <summary>
    /// Shows error on pendant UI
    /// </summary>
    public void ShowFeebackMessage(string message, bool persistent = true)
    {
        this.feebackHandler.SetTextField(message, persistent);
    }

    /// <summary>
    /// Marks seleceted part of an instruction
    /// </summary>
    /// <param name="startIndex">Stating index of markdown</param>
    /// <param name="endIndex">Ending index of a markdown</param>
    public void SelectText(int startIndex, int endIndex)
    {
        program.text = editedProgramText;
        programSelect.text = editedProgramText;
        program.text = program.text.Insert(endIndex, "</mark>");
        program.text = program.text.Insert(startIndex, "<mark=#1C3BFF>");
        programSelect.text = programSelect.text.Insert(endIndex, "</color>");
        programSelect.text = programSelect.text.Insert(startIndex, "<color=#D2EDD3>");
    }

    /// <summary>
    /// Allows change of selected instruction 
    /// </summary>
    public void UpdateIndexVisual(PendantData data)
    {
        StartCoroutine(UpdateIndexVisualCor(data));
    }

    /// <summary>
    /// Coroutine allowing event to update text and then selects proper part of instruction
    /// </summary>
    private IEnumerator UpdateIndexVisualCor(PendantData data)
    {
        yield return new WaitForEndOfFrame();
        int index = 0;
        for (int i = 0; i < data.CurrentInstructionIndex; i++)
        {
            index = editedProgramText.IndexOf("\n", index + 2);
        }
        if (index == -1) yield break;
        TextStartIndex = index == 0 ? index : index + 1;
        int selectedPart = 0;
        if (data.EditedProgram.Instructions[data.CurrentInstructionIndex] != null)
            data.EditedProgram.Instructions[data.CurrentInstructionIndex].selectedPart = selectedPart;
        SelectText(index, index + 6);
    }

    /// <summary>
    /// Updates UI according to current robot data 
    /// </summary>
    /// <param name="data">Robot data object</param>
    public void UpdateUI(RobotData data)
    {
        movementSpeed.text = (data.RobotSpeed * 100).ToString("N0") + " %";
        //switch (data.MovementMode)
        //{
        //    case MovementMode.T1:
        //        movementSpeed.text = (data.RobotSpeed * 100).ToString("N0") + " %";
        //        break;
        //    case MovementMode.T2:
        //        movementSpeed.text = (data.RobotSpeed * 100).ToString("N0") + " %";
        //        break;
        //}
        movementType.text = data.MovementType.ToString().ToUpper();
        movementMode.text = data.MovementMode.ToString().ToUpper();
        programStatus.text = data.IsRunning ? "RUNNING" : "STOPPED";
        currentlyRunningLine.text = "LINE " + data.CurrentRunningInstructionIndex;
        debugMarkerHandler.UpdateMarkerPosition(data.CurrentRunningInstructionIndex);
        debugMarkerHandler.UpdateRunningStatus(data.IsRunning);
        // debugArrow.localPosition = new Vector3(-57.1f, 38 - data.CurrentRunningInstruction * 5.2f, -0.1f);
        stepMode.text = data.StepMode.ToString();
    }

    /// <summary>
    /// Updates progra name using data updated in events
    /// </summary>
    public void UpdateUI(PendantData data)
    {
        editedProgramName.text = data.EditedProgram.Name;
        debugMarkerHandler.OnPendantUIUpdate();
    }

    /// <summary>
    /// Updates text visible in UI with markups
    /// </summary>
    public void UpdateProgramText(PendantData data)
    {
        var _currentText = "";
        int _lineIndex = 1;
        int _indentationLevel = 0;
        foreach (Instruction i in data.EditedProgram.Instructions)
        {
            if (_lineIndex < 10)
                _currentText += "   " + _lineIndex + ": ";
            else
                _currentText += "  " + _lineIndex + ": ";

            switch (i.lineIndentationEffect)
            {
                case IndentationEffect.NONE:
                    i.IndentationLevel = _indentationLevel;
                    break;
                case IndentationEffect.BLOCK_ENDER:
                    _indentationLevel--;
                    i.IndentationLevel = _indentationLevel;
                    break;
                case IndentationEffect.BLOCK_OPENER:
                    i.IndentationLevel = _indentationLevel;
                    _indentationLevel++;
                    break;
                case IndentationEffect.BLOCK_INTERSECTOR:
                    i.IndentationLevel = _indentationLevel - 1;
                    break;
            }

            _currentText += String.Concat(Enumerable.Repeat("  ", i.IndentationLevel));

            _currentText += i.GetInstructionText();
            _currentText += "\n";
            _lineIndex++;
        }
        editedProgramText = _currentText;
    }

    /// <summary>
    /// Updates text for TCP position on UI
    /// </summary>
    private void UpdateTCPPositions(RobotData data)
    {
        string text = pointPositions.text;
        string[] lines = text.Split('\n');
        for (int i = 0; i < data.TCPPoint.Length; ++i)
        {
            lines[i] = "P" + i.ToString() + " " +
                "X:" + data.TCPPoint[i].Item1.x +
                "Y:" + data.TCPPoint[i].Item1.y +
                "Z:" + data.TCPPoint[i].Item1.z +
                "   " +
                "RX:" + data.TCPPoint[i].Item2.x +
                " RY:" + data.TCPPoint[i].Item2.y +
                " RZ:" + data.TCPPoint[i].Item2.z +
                "\n";
        }
        text = string.Empty;
        foreach (string line in lines)
            text += line;
        text += "\n";

        pointPositions.text = text;
    }

    /// <summary>
    /// Updates TCP direct positions on UI
    /// </summary>
    private void UpdateTCPDirect(RobotData data)
    {
        string text = directPositions.text;
        string[] lines = text.Split('\n');
        for (int i = 0; i < data.TCPPoint.Length; ++i)
        {
            lines[i] = "P" + i.ToString() +
                "X: " + data.TCPPoint[i].Item1.x + "mm " +
                "Y: " + data.TCPPoint[i].Item1.y + "mm " +
                "Z: " + data.TCPPoint[i].Item1.z + "mm " +
                "\n";
        }
        text = string.Empty;
        foreach (string line in lines)
            text += line;
        text += "\n";

        directPositions.text = text;
    }

    /// <summary>
    /// Updates TCP orientation position on UI
    /// </summary>
    private void UpdateTCPOrientations(RobotData data)
    {
        string text = orientationPositions.text;
        string[] lines = text.Split('\n');
        string[] prefix = new string[] { "ORIGIN: ", "X AXIS: ", "Y AXIS: " };
        for (int i = 0; i < data.TCPPoint.Length; ++i)
        {
            lines[i] = prefix[i] +
                "X: " + data.TCPPoint[i].Item2.x + "mm " +
                "Y: " + data.TCPPoint[i].Item2.y + "mm " +
                "Z: " + data.TCPPoint[i].Item2.z + "mm " +
                "\n";
        }
        text = string.Empty;
        foreach (string line in lines)
            text += line;
        text += "\n";

        orientationPositions.text = text;
    }

    /// <summary>
    /// Updates real time coordinates on UI
    /// </summary>
    private void UpdateCoords(RobotData data)
    {
        string text = string.Empty;

        var multiplier = 1;

        Vector3 sourcePosition = Vector3.zero;
        Vector3 sourceRotation = Vector3.zero;
        List<Quaternion> sourceAngles = new List<Quaternion>() { Quaternion.identity, Quaternion.identity, Quaternion.identity, Quaternion.identity, Quaternion.identity, Quaternion.identity };

        if (RobotData.Instance.MovementMode != MovementMode.AUTO && !RobotData.Instance.IsRunning)
        {
            sourcePosition = RobotData.Instance.CurrentTarget.position;
            sourceRotation = RobotData.Instance.CurrentTarget.rotation;
            sourceAngles = RobotData.Instance.CurrentTarget.jointAngles;
        }

        var xPos = (float)Math.Round(-sourcePosition.x, 2) * multiplier;
        var yPos = (float)Math.Round(sourcePosition.y, 2) * multiplier;
        var zPos = (float)Math.Round(sourcePosition.z, 2) * multiplier;
        var xRot = (float)Math.Round(sourceRotation.x, 2);
        var yRot = (float)Math.Round(sourceRotation.y, 2);
        var zRot = (float)Math.Round(sourceRotation.z, 2);

        var j1 = (float)Math.Round(sourceAngles[0].eulerAngles.z, 2);
        var j2 = (float)Math.Round(sourceAngles[1].eulerAngles.y, 2);
        var j3 = (float)Math.Round(sourceAngles[2].eulerAngles.y, 2);
        var j4 = (float)Math.Round(sourceAngles[3].eulerAngles.x, 2);
        var j5 = (float)Math.Round(sourceAngles[4].eulerAngles.y, 2);
        var j6 = (float)Math.Round(sourceAngles[5].eulerAngles.x, 2);


        text += "X: " + (xPos.ToString("0.00") + " m ").PadLeft(10) + "   " + "J1: " + (j1.ToString("0.00") + " deg").PadLeft(11) + "\n";
        text += "Y: " + (yPos.ToString("0.00") + " m ").PadLeft(10) + "   " + "J2: " + (j2.ToString("0.00") + " deg").PadLeft(11) + "\n";
        text += "Z: " + (zPos.ToString("0.00") + " m ").PadLeft(10) + "   " + "J3: " + (j3.ToString("0.00") + " deg").PadLeft(11) + "\n";
        text += "W: " + (xRot.ToString("0.00") + " deg").PadLeft(10) + "  " + "J4: " + (j4.ToString("0.00") + " deg").PadLeft(11) + "\n";
        text += "P: " + (yRot.ToString("0.00") + " deg").PadLeft(10) + "  " + "J5: " + (j5.ToString("0.00") + " deg").PadLeft(11) + "\n";
        text += "R: " + (zRot.ToString("0.00") + " deg").PadLeft(10) + "  " + "J6: " + (j6.ToString("0.00") + " deg").PadLeft(11) + "\n";

        text += "\n";
        cooridantesStatus.text = text;
    }

}
