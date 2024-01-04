using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Custom event class for events with bool type parameter
/// </summary>
public sealed class BoolEvent : UnityEvent<bool>
{ }

/// <summary>
/// Class to handle the behavior of the stop button
/// </summary>
public class StopButton : PhysicalButton
{
    /// <summary>
    /// Event run when button is pressed
    /// </summary>
    public new BoolEvent OnPressed = new BoolEvent();
    /// <summary>
    /// Reference to program controller on scene
    /// </summary>
    private ProgramController programController;

    /// <summary>
    /// Sets default reference to progarm controller
    /// </summary>
    private void Awake()
    {
        programController = ControllersManager.Instance.GetController<ProgramController>();
    }

    /// <summary>
    /// Bind program controller to stop pressed button
    /// </summary>
    private void OnEnable()
    {
        OnPressed.AddListener(programController.StopProgram);
    }

    /// <summary>
    /// Unbind program controller to stop pressed button
    /// </summary>
    private void OnDisable()
    {
        OnPressed.RemoveListener(programController.StopProgram);
    }

    /// <summary>
    /// Uses button
    /// </summary>
    [ContextMenu("Press")]
    public void Press()
    {
        OnPressed.Invoke(true);         //to stop immediately
        // OnPressed.Invoke(false);     //to stop at the end of instruction/program
        base.OnPressed.Invoke();
    }
}
