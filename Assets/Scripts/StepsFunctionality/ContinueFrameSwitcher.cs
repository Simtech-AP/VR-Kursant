using UnityEngine;

/// <summary>
/// Allows changing position of canvas on scene
/// </summary>
public class ContinueFrameSwitcher : MonoBehaviour
{
    /// <summary>
    /// Reference to position changer
    /// </summary>
    private StepPositionController contineFramePositionController;


    /// <summary>
    /// Sets up references
    /// </summary>
    private void Awake()
    {
        contineFramePositionController = ControllersManager.Instance.GetController<StepPositionController>();
    }

    /// <summary>
    /// Sets new frame canvas position
    /// </summary>
    /// <param name="position">Position of canvas</param>
    public void SetCanvasPosition(int position = 0)
    {
        contineFramePositionController.SwitchContinueFramePosition((CanvasType)position);
    }
}
