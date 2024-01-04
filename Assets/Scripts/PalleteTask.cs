using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PalleteTaskPositionType
{
    PICK_UP_LOW,
    PICK_UP_HIGH,
    DROP_DOWN_HIGH,
    DROP_DOWN_LOW
}

[Serializable]
public struct PositionToPointBind
{
    public PalleteTaskPositionType positionType;
    public int asocciatedPoint;
}

[Serializable]
public class ProgramStructure
{
    public bool firstMoveCheck = false;
    public bool secondMoveCheck = false;
    public bool toolOnCheck = false;
    public bool secondAuxMoveCheck = false;
    public bool thirdMoveCheck = false;
    public bool fourthMoveCheck = false;
    public bool toolOffCheck = false;
    public bool fifthMoveCheck = false;

    public bool isWholeCorrect
    {
        private set { isWholeCorrect = value; }
        get
        {
            return firstMoveCheck
                    && secondMoveCheck
                    && toolOnCheck
                    && secondAuxMoveCheck
                    && thirdMoveCheck
                    && fourthMoveCheck
                    && toolOffCheck
                    && fifthMoveCheck;
        }
    }
}

public class PalleteTask : ExamTask
{

    [SerializeField] private HoverController pickUpAreaHoverLow;
    [SerializeField] private HoverController pickUpAreaHoverHigh;
    [SerializeField] private HoverController dropAreaHoverLow;
    [SerializeField] private HoverController dropAreaHoverHigh;

    public List<PositionToPointBind> positionToPointBinds = default;

    private ProgramController programController;
    private Grasper grasper;
    private bool grasperState = false;
    [SerializeField] private ProgramStructure programStructure = default;

    private Action onUpdate = delegate { };

    private void OnEnable()
    {
        programController = ControllersManager.Instance.GetController<ProgramController>();
        grasper = ControllersManager.Instance.GetController<RobotController>().CurrentRobot.RobotGrasper;

        onUpdate += checkAllPointsDeletionCondition;
        onUpdate += checkForToolOffActionCondition;
    }

    private void checkAllPointsDeletionCondition()
    {
        if (programController.GetMoveInstructionLinesCount() == 0)
        {
            testModule.GetPointByName("Zad.6: Usunięcie starych punktów w programie: ").SetPoints(10);
        }
    }

    public void checkForToolOffActionCondition()
    {
        if (grasper.IsHoldingObject == false && grasperState == true)
        {
            if (dropAreaHoverLow.Enabled)
            {
                testModule.GetPointByName("Zad.6: Prawidłowe odłożenie palety na przenośnik: ").SetPoints(10);
                onUpdate -= checkForToolOffActionCondition;
            }
            else
            {
                onUpdate -= checkForToolOffActionCondition;
            }
        }
    }

    public void onNewPointAdded()
    {
        var newPointIndex = RobotData.Instance.LoadedProgram.SavedPoints.Count - 1;

        if (pickUpAreaHoverHigh.Enabled)
        {
            positionToPointBinds.Add(new PositionToPointBind { positionType = PalleteTaskPositionType.PICK_UP_HIGH, asocciatedPoint = newPointIndex });
        }
        else if (dropAreaHoverHigh.Enabled)
        {
            positionToPointBinds.Add(new PositionToPointBind { positionType = PalleteTaskPositionType.DROP_DOWN_HIGH, asocciatedPoint = newPointIndex });
        }
        else if (dropAreaHoverLow.Enabled)
        {
            positionToPointBinds.Add(new PositionToPointBind { positionType = PalleteTaskPositionType.DROP_DOWN_LOW, asocciatedPoint = newPointIndex });
        }
        else if (pickUpAreaHoverLow.Enabled)
        {
            positionToPointBinds.Add(new PositionToPointBind { positionType = PalleteTaskPositionType.PICK_UP_LOW, asocciatedPoint = newPointIndex });
        }
    }

    private void checkProgramStructure()
    {

        var instructions = RobotData.Instance.LoadedProgram.Instructions;

        // Check if the first relevant move instruction goes to the high pick up point with JOINT movement
        programStructure.firstMoveCheck = checkAssociatedMoveInstructionCondition(instructions, -1, PalleteTaskPositionType.PICK_UP_HIGH, InstructionMovementType.JOINT, out int firstIndex);

        // Check if the next relevant move instruction goes to the low pick up point with LINEAR movement
        programStructure.secondMoveCheck = checkAssociatedMoveInstructionCondition(instructions, firstIndex, PalleteTaskPositionType.PICK_UP_LOW, InstructionMovementType.LINEAR, out int secondIndex);

        // Check if next istruction is TOOL ON
        programStructure.toolOnCheck = checkToolInstructionCondition(instructions, secondIndex + 1, InstructionToolAction.TOOLON);

        // Check if the next relevant move instruction goes to the high pick up point with LINEAR movement
        programStructure.secondAuxMoveCheck = checkAssociatedMoveInstructionCondition(instructions, secondIndex, PalleteTaskPositionType.PICK_UP_HIGH, InstructionMovementType.LINEAR, out int secondAuxIndex);

        // check if the next relevant instruction moves into high drop down point with JOINT movement
        programStructure.thirdMoveCheck = checkAssociatedMoveInstructionCondition(instructions, secondAuxIndex, PalleteTaskPositionType.DROP_DOWN_HIGH, InstructionMovementType.JOINT, out int thirdIndex);

        // check if the next relevant instruction moves into low drop down point with LINEAR movement
        programStructure.fourthMoveCheck = checkAssociatedMoveInstructionCondition(instructions, thirdIndex, PalleteTaskPositionType.DROP_DOWN_LOW, InstructionMovementType.LINEAR, out int fourthIndex);

        // Check if next istruction is TOOL OFF
        programStructure.toolOffCheck = checkToolInstructionCondition(instructions, fourthIndex + 1, InstructionToolAction.TOOLOFF);

        // check if the mext relevant instruction moves into high drop down point with LINEAR movement
        programStructure.fifthMoveCheck = checkAssociatedMoveInstructionCondition(instructions, fourthIndex, PalleteTaskPositionType.DROP_DOWN_HIGH, InstructionMovementType.LINEAR, out int fifthIndex);


        Debug.Log("First Move Check: " + programStructure.firstMoveCheck);
        Debug.Log("Second Move Check: " + programStructure.secondMoveCheck);
        Debug.Log("Tool ON Check: " + programStructure.toolOnCheck);
        Debug.Log("Second aux move check: " + programStructure.secondAuxMoveCheck);
        Debug.Log("Third Move Check: " + programStructure.thirdMoveCheck);
        Debug.Log("Fourth Move Check: " + programStructure.fourthMoveCheck);
        Debug.Log("Tool OFF Check: " + programStructure.toolOffCheck);
        Debug.Log("Fifth Move Check: " + programStructure.fifthMoveCheck);
    }

    private void setProgramStructureScores()
    {
        //apply test results 
        if (programStructure.firstMoveCheck)
        {
            testModule.GetPointByName("Zad.6: Punkt dojazdu robota nad paletę na przenośniku 1 jako JOINT: ").SetPoints(5);
        }
        else
        {
            testModule.GetPointByName("Zad.6: Brak punktu dojazdu nad paletę na przenośniku 1 jako JOINT: ").SetPoints(-10);
        }

        if (programStructure.secondMoveCheck)
        {
            testModule.GetPointByName("Zad.6: Punkt dojazdu robota do miejsca pobrania jako LIN: ").SetPoints(10);
        }
        else
        {
            testModule.GetPointByName("Zad.6: Brak punktu dojazdu LIN: ").SetPoints(-10);
        }

        if (programStructure.toolOnCheck)
        {
            testModule.GetPointByName("Zad.6: Dodanie instrukcji zamykającej chwytak TOOL ON w miejscu pobrania palety: ").SetPoints(10);
        }
        else
        {
            testModule.GetPointByName("Zad.6: Brak instrukcji w programie do zamykania chwytaka: ").SetPoints(-10);
        }

        if (programStructure.thirdMoveCheck)
        {
            testModule.GetPointByName("Zad.6: Przejazd z jednego przenośnika nad drugi jako JOINT: ").SetPoints(5);
        }
        else
        {
            testModule.GetPointByName("Zad.6: Brak JOINT z jednego przenośnika na drugi: ").SetPoints(-10);
        }

        if (programStructure.toolOffCheck)
        {
            testModule.GetPointByName("Zad.6: Dodanie instrukcji otwierającej chwytak TOOL OFF w miejscu odłożenia palety: ").SetPoints(10);
        }
        else
        {
            testModule.GetPointByName("Zad.6: Brak instrukcji w programie do otwierania chwytaka: ").SetPoints(-10);
        }

        if (programStructure.fifthMoveCheck)
        {
            testModule.GetPointByName("Zad.6: Punkt odjazdu robota z miejsca odłożenia jako LIN: ").SetPoints(10);
        }
        else
        {
            testModule.GetPointByName("Zad.6: Brak punktu odjazdu LIN: ").SetPoints(-10);
        }
    }

    private bool checkToolInstructionCondition(List<Instruction> instructions, int instructionIndex, InstructionToolAction requiredActionType)
    {
        try
        {
            var instructionAfterFirstAssociated = instructions[instructionIndex];
            return instructionAfterFirstAssociated.type == InstructionType.USE_TOOL && ((UseToolInstruction)instructionAfterFirstAssociated).actionType == requiredActionType;
        }
        catch
        {
            return false;
        }
    }

    private bool checkAssociatedMoveInstructionCondition(List<Instruction> instructions, int skipIndex, PalleteTaskPositionType requiredPositionType, InstructionMovementType requiredMovementType, out int instructionIndex)
    {
        try
        {
            var associatedMoveInstruction = (MoveInstruction)instructions.Skip(skipIndex + 1).ToList().Find(x =>
                   {
                       return x.type == InstructionType.MOVE && positionToPointBinds.Exists(y =>
                       {
                           return ((MoveInstruction)x).pointNumber == y.asocciatedPoint;
                       });
                   });

            instructionIndex = instructions.IndexOf(associatedMoveInstruction);
            var instructionAreaType = positionToPointBinds.Find(x => x.asocciatedPoint == associatedMoveInstruction.pointNumber).positionType;

            if (instructionAreaType != requiredPositionType || associatedMoveInstruction.movementType != requiredMovementType)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        catch
        {
            instructionIndex = skipIndex + 1;
            return false;
        }
    }

    public void checkOnPlayCondition()
    {
        checkProgramStructure();

        if (ErrorRequester.HasAllErrorsReset() && ErrorRequester.HasResetAlarmErrors())
        {
            if (RobotData.Instance.StepMode == StepMode.NORMAL)
            {
                // checkProgramStructure();
                testModule.GetPointByName("Zad.6: Uruchomienie programu w trybie ręcznym ciągłym: ").SetPoints(10);

                onUpdate += awaitRobotExecutionPause;
            }
        }
    }

    public void awaitRobotExecutionPause()
    {
        if (RobotData.Instance.IsRunning == false)
        {
            Debug.Log("xd");
            checkEntireProgramExecutionCondition();
            onUpdate -= awaitRobotExecutionPause;
        }
    }

    public void checkEntireProgramExecutionCondition()
    {
        // zaliczą przełożenie palety nawet jeśli procedura nie była w 100% poprawna?
        if (RobotData.Instance.CurrentRunningInstructionIndex == RobotData.Instance.LoadedProgram.Instructions.Count - 1 && programStructure.isWholeCorrect)
        {
            testModule.GetPointByName("Zad.6: Wykonanie jednego pełnego cyklu przekładania palety: ").SetPoints(20);
        }
    }


    private void Update()
    {
        onUpdate();
        grasperState = grasper.IsHoldingObject;

        // if (Input.GetKeyDown(KeyCode.L))
        // {
        //     checkProgramStructure();
        // }
    }

    public override void EndTask()
    {
        setProgramStructureScores();

        onUpdate -= checkAllPointsDeletionCondition;
        onUpdate -= checkForToolOffActionCondition;
    }
}
