using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// Maintains logic of a pendant
/// </summary>
public class PendantController : Controller
{
    /// <summary>
    /// Reference to user inerface object of pendant
    /// </summary>
    [SerializeField]
    private PendantUI pendantUI = default;
    /// <summary>
    /// Reference to program controller
    /// </summary>
    [SerializeField]
    private ProgramController programController;
    /// <summary>
    /// Reference to data for pendant
    /// </summary>
    private PendantData data;
    /// <summary>
    /// Currently selected part of an instruction
    /// </summary>
    private int selectedPart = 0;
    public int SelectedPart { get => selectedPart; set => selectedPart = value; }

    [SerializeField]
    private GameObject greenLight;

    [SerializeField]
    private GameObject redLight;

    private Instruction savedInstruction;

    public UnityEventInt OnLineIndexChanged = default;

    public static Action OnProgramDeleted = delegate { };

    /// <summary>
    /// Gets data for pendant
    /// Starts delayed initialization of UI 
    /// </summary>
    private void Start()
    {
        data = PendantData.Instance;
        StartCoroutine(DelayedInit());
    }

    private void OnEnable()
    {
        ErrorController.OnErrorOccured += ErrorToggle;
        AlarmErrorController.OnErrorReset += EnableMoveToggle;
        RobotErrorController.OnErrorReset += ErrorReset;
    }

    private void ErrorReset()
    {
        redLight.SetActive(false);
    }

    private void EnableMoveToggle()
    {
        redLight.SetActive(false);
        greenLight.SetActive(true);
    }

    private void ErrorToggle(ErrorController errorContr, Error error)
    {
        redLight.SetActive(true);
        greenLight.SetActive(false);
        if (error.Code.Equals("R-1003"))
        {
            JointRobotAngles jointRobot = FindObjectOfType<RobotController>().CurrentRobot.GetComponent<JointRobotAngles>();
            ShowError("ERROR: LIMIT REACHED ON " + jointRobot.GetPivotOnLimit().name.ToUpper());
        }
    }

    private void OnDisable()
    {
        ErrorController.OnErrorOccured += ErrorToggle;
        AlarmErrorController.OnErrorReset += EnableMoveToggle;
        RobotErrorController.OnErrorReset += ErrorReset;
    }

    /// <summary>
    /// Records point to program memory
    /// </summary>
    public void RecordPoint()
    {
        if (data.EditedProgram.Instructions[data.CurrentInstructionIndex].type == InstructionType.MOVE)
        {
            ControllersManager.Instance.GetController<RobotController>().CurrentRobot.UpdateRobotTarget();

            var pointIndex = ((MoveInstruction)data.EditedProgram.Instructions[data.CurrentInstructionIndex]).pointNumber;
            ControllersManager.Instance.GetController<ProgramController>().ChangePoint(pointIndex, new Point()
            {
                position = RobotData.Instance.CurrentTarget.position,
                rotation = RobotData.Instance.CurrentTarget.rotation,
                jointAngles = RobotData.Instance.CurrentTarget.jointAngles
            });

            pendantUI.ShowFeebackMessage("Recorded new coordinates for point [" + pointIndex + "].");
        }


    }

    /// <summary>
    /// Change currently selected instruction index
    /// </summary>
    /// <param name="up">Should the counter go up?</param>
    public void ChangeInstructionIndex(bool up)
    {
        if (up)
        {
            if (data.CurrentInstructionIndex < data.EditedProgram.Instructions.Count - 1)
            {
                data.CurrentInstructionIndex++;
                selectedPart = 0;
                OnLineIndexChanged.Invoke(data.CurrentInstructionIndex);
            }
        }
        else
        {
            if (data.CurrentInstructionIndex > 0)
            {
                data.CurrentInstructionIndex--;
                selectedPart = 0;
                OnLineIndexChanged.Invoke(data.CurrentInstructionIndex);
            }
        }
    }

    /// <summary>
    /// Removes currently selected instruction from program
    /// </summary>
    public void RemoveInstruction()
    {
        programController.RemoveInstruction(data.CurrentInstructionIndex);
        selectedPart = 1;
        SelectPart(false);
    }

    /// <summary>
    /// Comments or uncomments currently selected instruction from program
    /// </summary>
    public void ToggleInstructionCommentStatus()
    {
        programController.ToggleInstructionCommentStatus();
    }


    /// <summary>
    /// Stops program execution
    /// </summary>
    public void StopProgram()
    {
        programController.StopProgram(true);
    }

    /// <summary>
    /// Starts running program
    /// </summary>
    public void StartProgram()
    {
        programController.RunProgram(data.EditedProgram);
    }

    public void MoveTo()
    {
        programController.MoveTo();
    }

    /// <summary>
    /// Inserts instruction at the selected index of instruction
    /// </summary>
    /// <param name="instruction">Instruction to insert</param>
    public void InsertInstructionAbove(Instruction instruction)
    {
        programController.InsertInstruction(instruction, data.CurrentInstructionIndex, EditInstructionType.INSERT_ABOVE);
        SelectPart(false);
    }

    /// <summary>
    /// Inserts instruction at the selected index of instruction
    /// </summary>
    /// <param name="instruction">Instruction to insert</param>
    public void InsertInstructionUnder(Instruction instruction)
    {
        programController.InsertInstruction(instruction, data.CurrentInstructionIndex, EditInstructionType.INSERT_UNDER);
        SelectPart(false);
    }

    public void AddProgram(ProgramUIData uiData)
    {
        Program newProgram = new Program(uiData.Name.text, uiData.Description.text, new List<Instruction>(1) { new DelayInstruction() }, new List<Point>());
        programController.AddProgram(newProgram);
        this.data.CurrentInstructionIndex = 0;
        programController.EditProgram(ref newProgram);
    }

    public void DuplicateProgram(Program toDuplicate, ProgramUIData data)
    {
        var duplicated = new Program(toDuplicate);
        duplicated.Name = data.Name.text;
        duplicated.Description = data.Description.text;

        programController.AddProgram(duplicated);
        pendantUI.ShowFeebackMessage("Duplicated program [" + toDuplicate.Name + "] into [" + duplicated.Name + "].");
        this.data.CurrentInstructionIndex = 0;
        programController.EditProgram(ref duplicated);
    }

    public void SetCurrentProgram(Program program)
    {
        this.data.CurrentInstructionIndex = 0;
        programController.EditProgram(ref program);
    }

    public void RenameCurrentProgram(ProgramUIData uiData)
    {
        programController.RenameProgram(uiData.Name.text, uiData.Description.text);
    }

    /// <summary>
    /// Shows error on top bar on screen
    /// </summary>
    /// <param name="error">Text of an error</param>
    public void ShowError(string error)
    {
        pendantUI.ShowFeebackMessage(error, true);
    }

    public void ClearError()
    {
        if (ErrorRequester.HasAllErrorsReset())
            pendantUI.ShowFeebackMessage(string.Empty);
    }

    public void ClearError(string message)
    {
        if (ErrorRequester.HasAllErrorsReset())
            pendantUI.ShowFeebackMessage(message);
    }

    /// <summary>
    /// Changes selected part of an instruction
    /// </summary>
    /// <param name="goRight">Should select next part?</param>
    public void SelectPart(bool goRight)
    {
        var currentInstruction = data.CurrentInstruction;
        currentInstruction.isPartEditedFirstTime = true;

        if (goRight && selectedPart < currentInstruction.maxSelectablePartIndex)
        {
            selectedPart++;
        }
        else if (!goRight && selectedPart > 0)
        {
            selectedPart--;
        }

        if (selectedPart > 0)
        {
            pendantUI.SelectText(currentInstruction.SelectPart(selectedPart).Item1 + pendantUI.TextStartIndex + 6, currentInstruction.SelectPart(selectedPart).Item2 + pendantUI.TextStartIndex + 6);
        }
        else
        {
            pendantUI.SelectText(pendantUI.TextStartIndex, pendantUI.TextStartIndex + 5);
        }
    }

    /// <summary>
    /// Sends input to instruction for processing
    /// </summary>
    /// <param name="input">Inputted sequence</param>
    public void ProcessInputOnInstruction(string input)
    {
        var currentInstruction = data.EditedProgram.Instructions[data.CurrentInstructionIndex];
        currentInstruction.ProcessInput(input);
        if (selectedPart > 0)
        {
            pendantUI.SelectText(currentInstruction.SelectPart(selectedPart).Item1 + pendantUI.TextStartIndex + 6, currentInstruction.SelectPart(selectedPart).Item2 + pendantUI.TextStartIndex + 6);
            StartCoroutine(UpdateUI());
        }
        else
        {
            pendantUI.SelectText(pendantUI.TextStartIndex, pendantUI.TextStartIndex + 5);
        }
    }

    private IEnumerator UpdateUI()
    {
        pendantUI.UpdateProgramText(data);
        yield return new WaitForEndOfFrame();
        var currentInstruction = data.EditedProgram.Instructions[data.CurrentInstructionIndex];
        if (selectedPart > 0)
        {
            pendantUI.SelectText(currentInstruction.SelectPart(selectedPart).Item1 + pendantUI.TextStartIndex + 6, currentInstruction.SelectPart(selectedPart).Item2 + pendantUI.TextStartIndex + 6);
        }
        else
        {
            pendantUI.SelectText(pendantUI.TextStartIndex, pendantUI.TextStartIndex + 5);
        }
    }

    public void CopyCurrentInstruction()
    {
        savedInstruction = InstructionFactory.CloneInstruction(data.EditedProgram.Instructions[data.CurrentInstructionIndex]);
        pendantUI.ShowFeebackMessage("Copied line [" + (data.CurrentInstructionIndex + 1) + "].");
    }

    public void PasteToCurrentInstruction()
    {
        if (savedInstruction)
        {
            SetCurrentInsctruction(savedInstruction);
            pendantUI.ShowFeebackMessage("Pasted into line [" + (data.CurrentInstructionIndex + 1) + "].");
        }
    }

    public void CutoutCurrentInstruction()
    {
        savedInstruction = data.EditedProgram.Instructions[data.CurrentInstructionIndex];
        DeleteCurrentInstruction();
        pendantUI.ShowFeebackMessage("Cut line [" + (data.CurrentInstructionIndex + 1) + "].");

    }

    public void DeleteCurrentInstruction()
    {
        Program current = data.EditedProgram;
        current.Instructions.RemoveAt(data.CurrentInstructionIndex);
        data.CurrentInstructionIndex = Mathf.Clamp(data.CurrentInstructionIndex, 0, data.EditedProgram.Instructions.Count - 1);
        programController.EditProgram(ref current);
        pendantUI.ShowFeebackMessage("Deleted line [" + (data.CurrentInstructionIndex + 1) + "].");
    }

    public void SetCurrentInsctruction(Instruction savedInstruction)
    {
        Program current = data.EditedProgram;
        Instruction newInstruction = InstructionFactory.CloneInstruction(savedInstruction);
        current.Instructions[data.CurrentInstructionIndex] = newInstruction;
        programController.EditProgram(ref current);
    }

    public List<Program> GetPrograms()
    {
        return PendantData.Instance.SavedPrograms.ToList();
    }

    public void DeleteProgram(Program program)
    {
        var result = programController.RemoveProgram(program);

        if (result)
        {
            pendantUI.ShowFeebackMessage("Deleted program [" + program.Name + "].");
            PendantController.OnProgramDeleted();
        }
        else
        {
            pendantUI.ShowFeebackMessage("Cannot delete current program");
        }

    }

    /// <summary>
    /// Delays initialization of markdown for full program parsing
    /// </summary>
    /// <returns>Delay before selecting</returns>
    private IEnumerator DelayedInit()
    {
        yield return new WaitForSeconds(0.05f);
        ChangeInstructionIndex(false);
    }

    /// <summary>
    /// Initalize only required compoments for independent controller
    /// </summary>
    protected override void InitalizeControllers()
    {
        programController = ControllersManager.Instance.GetController<ProgramController>();
    }

    public void ResetSelectedPart()
    {
        selectedPart = 0;
    }

    public void ResetCanvas()
    {
        pendantUI.ResetCanvas();
    }
}
