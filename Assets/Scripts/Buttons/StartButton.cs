using UnityEngine;

/// <summary>
/// Class to handle the behavior of the start button
/// </summary>
public class StartButton : PhysicalButton, IResetable
{
    /// <summary>
    /// Reference to program controller
    /// </summary>
    private ProgramController programController = default;


    /// <summary>
    /// Sets default reference to progarm controller
    /// </summary>
    private void Awake()
    {
        programController = ControllersManager.Instance.GetController<ProgramController>();
    }

    /// <summary>
    /// Bind methods to events
    /// </summary>
    private void OnEnable()
    {
        OnPressed.AddListener(programController.RunProgram);
    }

    /// <summary>
    /// Unbind methods from events
    /// </summary>
    private void OnDisable()
    {
        OnPressed.RemoveListener(programController.RunProgram);
    }

    /// <summary>
    /// Uses button
    /// </summary>
    [ContextMenu("PRESS")]
    public void Press()
    {
        OnPressed?.Invoke();
    }

    /// <summary>
    /// Reset button to initial state
    /// </summary>
    void IResetable.Reset()
    {
        programController.RunProgram();
    }
}
