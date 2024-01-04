using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class allowing for changing position of object according to step needs
/// </summary>
public class StepPositionController : Controller
{
    /// <summary>
    /// Set of Vectors describing instruction frame position and rotation
    /// </summary>
    [System.Serializable]
    public class FrameTransform
    {
        /// <summary>
        /// Position of frame
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// Rotation of frame
        /// </summary>
        public Vector3 rotation;

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="_pos">Set position of frame</param>
        /// <param name="_rot">Set rotation of frame</param>
        public FrameTransform(Vector3 _pos, Vector3 _rot)
        {
            position = _pos;
            rotation = _rot;
        }
    }

    /// <summary>
    /// Position of next step button
    /// </summary>
    [SerializeField]
    private Transform buttonInsidePosition = default;
    /// <summary>
    /// Starting position of next step button
    /// </summary>
    [SerializeField]
    private Transform buttonOutsidePosition = default;
    /// <summary>
    /// Reference to next step button object
    /// </summary>
    [SerializeField]
    private UserCockpitController userCockpit = default;
    /// <summary>
    /// Reference to tutorial frame
    /// </summary>
    [SerializeField]
    private Transform uiFrame = default;

    /// <summary>
    /// Dictionary of canvas positions
    /// </summary>
    private Dictionary<CanvasType, FrameTransform> positions = new Dictionary<CanvasType, FrameTransform>
    {
        {CanvasType.INSIDE_FRONT, new FrameTransform(new Vector3(2.9f, 1.4f, -6.6f), Vector3.zero)},
        {CanvasType.OUTSIDE_FRONT, new FrameTransform(new Vector3(-2.8f, 1.4f, -6.6f), Vector3.zero)},
        {CanvasType.INSIDE_LEFT, new FrameTransform(new Vector3(5f, 1.4f, -1.87f), new Vector3(0, -90f, 0))}
    };

    /// <summary>
    /// Sets starting position of button
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
    }


    private void SetCanvasAndButtonPosition(CanvasType canvasType)
    {
        uiFrame.transform.localPosition = positions[canvasType].position;
        uiFrame.transform.localEulerAngles = positions[canvasType].rotation;
    }

    /// <summary>
    /// Switches frame positionaccording to parameter
    /// </summary>
    /// <param name="_canvasType">Enum of position of frame</param>
    public void SwitchContinueFramePosition(CanvasType _canvasType)
    {
        switch (_canvasType)
        {
            case CanvasType.INSIDE_FRONT:
                SetCanvasAndButtonPosition(CanvasType.INSIDE_FRONT);
                break;
            case CanvasType.OUTSIDE_FRONT:
                SetCanvasAndButtonPosition(CanvasType.OUTSIDE_FRONT);
                break;
            case CanvasType.INSIDE_LEFT:
                SetCanvasAndButtonPosition(CanvasType.INSIDE_LEFT);
                break;
        }
    }
}
