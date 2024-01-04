using System;
using System.Linq;
using UnityEngine;

public class StepModeTask : ExamTask
{
    private bool wasNewProgramCreated = false;
    private int newPointsAdded = 0;
    private int moveInstructionsExecuted = 0;
    private PendantController pendantController;

    private void OnEnable()
    {
        pendantController = ControllersManager.Instance.GetController<PendantController>();
    }

    public void checkCreatedNewProgramCondition()
    {
        wasNewProgramCreated = true;
        testModule.GetPointByName("Zad.5: Utworzenie nowego programu: ").SetPoints(40);
    }

    public void checkNewPointsCondition()
    {
        // newPointsAdded++;

        // if (newPointsAdded <= 3)
        // {
        //     testModule.GetPointByName("Zad.5: Dodanie punktu: ").SetPoints(10 * newPointsAdded);
        // }

        // checkNewPointUniqueness();
    }

    private void checkNewPointUniqueness()
    {
        // var uniquePointCount = RobotData.Instance.LoadedProgram.SavedPoints.Distinct().Count();
        // Debug.Log(uniquePointCount);
        // uniquePointCount = Mathf.Clamp(uniquePointCount, 0, 3);

        // testModule.GetPointByName("Zad.5: Każdy z trzech punktów jest w innym miejscu: ").SetPoints(10 * (uniquePointCount - 1));
    }

    public void checkOnPlayConditions()
    {
        if (ErrorRequester.HasAllErrorsReset() && ErrorRequester.HasResetAlarmErrors())
        {
            checkForStepModeCondition();
            checkForPointsOnPlayCondition();
        }

    }

    private void checkForStepModeCondition()
    {
        if (RobotData.Instance.StepMode == StepMode.STEP
                    && RobotData.Instance.CurrentRunningInstruction.type == InstructionType.MOVE
                    && moveInstructionsExecuted < 3)
        {
            moveInstructionsExecuted++;
            // testModule.GetPointByName("Zad.5: Uruchomienie programu w trybie krokowym: ").SetPoints(10 * moveInstructionsExecuted);
            testModule.GetPointByName("Zad.5: Uruchomienie programu w trybie krokowym: ").SetPoints(10);
        }
        else if (RobotData.Instance.StepMode == StepMode.NORMAL)
        {
            testModule.GetPointByName("Zad.5: Uruchomienie programu w trybie ciągłym: ").SetPoints(-20);
        }
    }

    private void checkForPointsOnPlayCondition()
    {
        if (RobotData.Instance.LoadedProgram.SavedPoints.Count < 3)
        {
            testModule.GetPointByName("Zad.5: Uruchomienie programu z mniejszą liczbą punktów niż 3: ").SetPoints(-10);
        }
    }

    private void checkPointConditions()
    {
        var moveInstructions = RobotData.Instance.LoadedProgram.Instructions.Where(x => x.type == InstructionType.MOVE).ToList();
        var pointIndexesInProgram = moveInstructions.Select(x => ((MoveInstruction)x).pointNumber).Distinct().ToList();

        var scoredPointCount = Mathf.Clamp(pointIndexesInProgram.Count, 0, 3);
        testModule.GetPointByName("Zad.5: Dodanie punktu: ").SetPoints(10 * scoredPointCount);

        var pointsInProgram = pointIndexesInProgram.Select(x => RobotData.Instance.LoadedProgram.SavedPoints[x]).ToList();
        var uniquePointsCount = pointsInProgram.Distinct().ToList().Count;

        uniquePointsCount = Mathf.Clamp(uniquePointsCount, 0, 3);

        testModule.GetPointByName("Zad.5: Każdy z trzech punktów jest w innym miejscu: ").SetPoints(10 * (uniquePointsCount - 1));
    }

    public override void EndTask()
    {
        checkPointConditions();
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.H))
    //     {
    //         checkPointConditions();
    //     }
    // }

}