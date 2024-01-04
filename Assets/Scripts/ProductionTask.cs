using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Task containing logic of starting production
/// </summary>
public class ProductionTask : ExamTask
{
    /// <summary>
    /// List of buttons on scene needed for task
    /// </summary>
    private List<GameObject> buttons;
    /// <summary>
    /// Alarm reset buttons on scene
    /// </summary>
    private List<GameObject> alarmButtons;

    /// <summary>
    /// Sets up references to buttons and adds listeners to them
    /// </summary>
    private void OnEnable()
    {
        buttons = InteractablesManager.Instance.GetAllInteractableButton(ButtonType.START);
        alarmButtons = InteractablesManager.Instance.GetAllInteractableButton(ButtonType.ALARM_RESET);
        for (int i = 0; i < buttons.Count; ++i)
        {
            buttons[i].GetComponent<PhysicalButton>().OnPressed.AddListener(SetRobotStart);
        }
        for (int i = 0; i < alarmButtons.Count; ++i)
        {
            alarmButtons[i].GetComponent<PhysicalButton>().OnPressed.AddListener(CheckErrors);
        }
    }

    /// <summary>
    /// Checks if any errors accured when task is ongoing
    /// </summary>
    private void CheckErrors()
    {
        if (RobotData.Instance.MovementMode == MovementMode.AUTO)
        {
            testModule.Data.Points.Find(x => x.Name.Equals("Zad.3: Przełączenie robota w AUTO przed próbą skasowania błędów i naciśnięciem START: ")).SetPoints(20);
        }
        if (RobotData.Instance.MovementMode == MovementMode.AUTO && CellStateData.cellEntranceState == CellEntranceState.Closed)
        {
            testModule.Data.Points.Find(x => x.Name.Equals("Zad.3: Skasowanie błędów, gdy bramka jest zamknięta i robot jest w AUTO: ")).SetPoints(20);
        }
    }

    /// <summary>
    /// Checks if parts of task has been fulfilled
    /// </summary>
    private void SetRobotStart()
    {
        if (RobotData.Instance.MovementMode != MovementMode.AUTO)
            testModule.Data.Points.Find(x => x.Name.Equals("Zad.3: Naciśnięcie START podczas, gdy robot nie jest w AUTO: ")).SetPoints(-25);

        if (CellStateData.cellEntranceState != CellEntranceState.Closed)
            testModule.Data.Points.Find(x => x.Name.Equals("Zad.3: Naciśnięcie START podczas, gdy bramka jest otwarta: ")).SetPoints(-30);


        if (RobotData.Instance.MovementMode == MovementMode.AUTO && CellStateData.cellEntranceState == CellEntranceState.Closed && !ErrorRequester.HasAnyErrors())
        {
            testModule.Data.Points.Find(x => x.Name.Equals("Zad.3: Przełączenie robota w AUTO przed próbą skasowania błędów i naciśnięciem START: ")).SetPoints(35);
            testModule.Data.Points.Find(x => x.Name.Equals("Zad.3: Skasowanie błędów, gdy bramka jest zamknięta i robot jest w AUTO: ")).SetPoints(35);
        }
    }

    /// <summary>
    /// Finishes task
    /// Removes listeners from events
    /// </summary>
    public override void EndTask()
    {
        for (int i = 0; i < buttons.Count; ++i)
        {
            buttons[i].GetComponent<PhysicalButton>().OnPressed.RemoveListener(SetRobotStart);
        }

        for (int i = 0; i < alarmButtons.Count; ++i)
        {
            alarmButtons[i].GetComponent<PhysicalButton>().OnPressed.RemoveListener(CheckErrors);
        }

        var cellState = testModule.Data.Points.Find(x => x.Name.Equals("Zad.3: Zamknięcie bramki: "));
        if (CellStateData.cellEntranceState == CellEntranceState.Closed)
            cellState.SetPoints(10);

        if (RobotData.Instance.IsRunning)
            testModule.Data.Points.Find(x => x.Name.Equals("Zad.3: Uruchomienie produkcji przyciskiem START po spełnieniu warunków: ")).SetPoints(30);
    }

    /// <summary>
    /// Checks if any of errors in task has been made
    /// </summary>
    private void Update()
    {
        if (CellStateData.cellEntranceState == CellEntranceState.Moving && CellStateData.padLockState == PadLockState.OnDoor)
        {
            testModule.Data.Points.Find(x => x.Name.Equals("Zad.3: Próba zamknięcia bramki przed zdjęciem kłódki: ")).SetPoints(-15);
        }
    }
}
