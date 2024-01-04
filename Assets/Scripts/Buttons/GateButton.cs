using System.Collections;
using UnityEngine;

/// <summary>
/// Provides logic to GATE button - after pressing robot will stop on program ending and alarm from gate will be removed until starting program again
/// </summary>
public class GateButton : PhysicalButton
{

    /// <summary>
    /// Instance of RobotController
    /// </summary>
    private RobotController robotController = default;

    /// <summary>
    /// Instance of ProgramController
    /// </summary>
    private ProgramController programController = default;

    /// <summary>
    /// Reference to gate button light
    /// </summary>
    [SerializeField]
    private GameObject gateLight = default;

    /// <summary>
    /// Flag to check if robot has clearly stopped (stopped after ending program on request)
    /// </summary>
    private bool clearStop = false;

    /// <summary>
    /// Flag to check if (since last program end) GATE button was already pressed (== true) -> pressing it again will cancel clearStop request
    /// </summary>
    private bool alreadyPressed = false;


    /// <summary>
    /// Gets references to controllers and starts light
    /// </summary>
    private void Awake()
    {
        robotController = ControllersManager.Instance.GetController<RobotController>();
        programController = ControllersManager.Instance.GetController<ProgramController>();
        EnableLight();
    }

    /// <summary>
    /// Binds methode to action
    /// </summary>
    private void OnEnable()
    {
        programController.OnProgramRun += RobotResume;
    }

    /// <summary>
    /// Unbinds methode from action
    /// </summary>
    private void OnDisable()
    {
        programController.OnProgramRun -= RobotResume;
    }

    /// <summary>
    /// Asks robot to clear stop (stopping after program end on our request) - called on first button press
    /// </summary>
    private void RequestClearStop()
    {
        robotController.ShouldStopAfterProgram = true;
        robotController.OnProgramEnding += RobotClearlyStopped;
        StartBlinking();
    }

    /// <summary>
    /// Cancell clearStop request - called on second button press
    /// </summary>
    private void CancellClearStop()
    {
        robotController.ShouldStopAfterProgram = false;
        robotController.OnProgramEnding -= RobotClearlyStopped;
        clearStop = false;
        ErrorRequester.SetGateAlarm(clearStop);
        EnableLight();
    }

    /// <summary>
    /// Called after robot clearStop (stopping after program end on our request), disables gate alarm and sets necessary flags
    /// </summary>
    private void RobotClearlyStopped()
    {
        robotController.ShouldStopAfterProgram = false;
        robotController.OnProgramEnding -= RobotClearlyStopped;
        clearStop = true;
        ErrorRequester.SetGateAlarm(clearStop);
        alreadyPressed = false;
        EndBlinking();
    }

    /// <summary>
    /// Called when robot resumes running after clearStop (stopping after program end on our request), enables gate alarm and sets necessary flags
    /// </summary>
    private void RobotResume()
    {
        if (clearStop)
        {
            EnableLight();
            clearStop = false;
            ErrorRequester.SetGateAlarm(clearStop);
        }
    }

    /// <summary>
    /// Coroutine to blink gate light 
    /// </summary>
    private IEnumerator startBlinking()
    {
        while (true)
        {
            gateLight.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            gateLight.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Function that should be called every time robot movement mode is switched
    /// </summary>
    public void ChangedMovementMode()
    {
        clearStop = false;
        alreadyPressed = false;
        ErrorRequester.SetGateAlarm(clearStop);     //remember that checking "gate open" error checks if movement type is set to AUTO

        switch (RobotData.Instance.MovementMode)
        {
            case MovementMode.T1:
            case MovementMode.T2:
                if (robotController.ShouldStopAfterProgram)
                {
                    robotController.ShouldStopAfterProgram = false;
                    robotController.OnProgramEnding -= RobotClearlyStopped;
                }
                EndBlinking();
                break;

            case MovementMode.AUTO:
                EnableLight();
                break;
        }
    }

    /// <summary>
    /// Turns on gate light
    /// </summary>
    public void EnableLight()
    {
        StopAllCoroutines();
        gateLight?.SetActive(true);
    }

    /// <summary>
    /// Makes gate light blinking
    /// </summary>
    public void StartBlinking()
    {
        StartCoroutine(startBlinking());
    }

    /// <summary>
    /// Turns off gate light
    /// </summary>
    public void EndBlinking()
    {
        StopAllCoroutines();
        gateLight?.SetActive(false);
    }

    /// <summary>
    /// Uses button
    /// </summary>
    [ContextMenu("Press")]
    public void Press()
    {
        OnPressed?.Invoke();
        if (!clearStop && RobotData.Instance.MovementMode == MovementMode.AUTO)
        {
            if (!alreadyPressed)
            {
                alreadyPressed = true;
                RequestClearStop();
            }
            else
            {
                alreadyPressed = false;
                CancellClearStop();
            }
        }
    }
}
