/// <summary>
/// Task using robot
/// </summary>
public class RobotTask : ExamTask
{
    /// <summary>
    /// Reference to robot on scene
    /// </summary>
    private Robot robot;
    /// <summary>
    /// Is any deadman pressed?
    /// </summary>
    private bool deadManPressed = false;
    /// <summary>
    /// Is reset errors button pressed?
    /// </summary>
    private bool resetPressed = false;

    /// <summary>
    /// Sets up reference to robot
    /// </summary>
    private void OnEnable()
    {
        robot = FindObjectOfType<RobotController>().CurrentRobot;
    }

    /// <summary>
    /// Sets the flag according to pressed deadman switch
    /// </summary>
    /// <param name="status">Status of flag to set</param>
    public void SetDeadManStatus(bool status)
    {
        deadManPressed = status;
    }

    /// <summary>
    /// Sets the error reset flag
    /// </summary>
    public void SetResetStatus()
    {
        if (deadManPressed)
            resetPressed = true;
    }

    /// <summary>
    /// Finishes task with specified point values
    /// </summary>
    public override void EndTask()
    {
        if (deadManPressed)
            testModule.Data.Points.Find(x => x.Name.Equals("Zad.2: Zazbrojenie robota przyciskiem czuwaka: ")).SetPoints(15);
        else
            testModule.Data.Points.Find(x => x.Name.Equals("Zad.2: Próba poruszania robotem bez zazbrojonego czuwaka: ")).SetPoints(-10);

        if (resetPressed)
            testModule.Data.Points.Find(x => x.Name.Equals("Zad.2: Skasowanie błędów: ")).SetPoints(15);
        else
            testModule.Data.Points.Find(x => x.Name.Equals("Zad.2: Próba poruszania robotem z czuwakiem, ale bez skasowania błędów: ")).SetPoints(-10);
    }

    /// <summary>
    /// Checks flags and states of robot according to task needs
    /// </summary>
    private void Update()
    {
        if (robot.IsMoving)
        {
            if (RobotData.Instance.MovementMode == MovementMode.T1)
                testModule.Data.Points.Find(x => x.Name.Equals("Zad.2: Poruszanie robotem w trybie T1: ")).SetPoints(20);
            else if (RobotData.Instance.MovementMode == MovementMode.T2)
                testModule.Data.Points.Find(x => x.Name.Equals("Zad.2: Poruszanie robotem w trybie T2: ")).SetPoints(-10);

            if (RobotData.Instance.MovementType == MovementType.Joint)
                testModule.Data.Points.Find(x => x.Name.Equals("Zad.2: Przełączenie układu na JOINT za pierwszym razem, przed ruchem: ")).SetPoints(30);
            else if (RobotData.Instance.MovementType != MovementType.Joint)
                testModule.Data.Points.Find(x => x.Name.Equals("Zad.2: Poruszanie robotem w innym układzie niż JOINT: ")).SetPoints(-20);
        }
    }
}
