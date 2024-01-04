using QuickOutline;
using UnityEngine;

/// <summary>
/// Helper class for checking if we reached a singularity
/// </summary>
public class AxisSingularityChecker : MonoBehaviour
{
    /// <summary>
    /// Reference to visual axis helper
    /// </summary>
    private RobotVisualAxis robotVisualAxis;
    /// <summary>
    /// Reference to fifth axis of robot
    /// </summary>
    private Transform fithAxis;
    /// <summary>
    /// Reference to fourth axis of robot
    /// </summary>
    private Transform fourthAxis;
    /// <summary>
    /// Reference to outline object 
    /// </summary>
    [HideInInspector]
    public Outline outline;
    /// <summary>
    /// Reference to specified robot axis
    /// </summary>
    //TODO: This is used only to get fifth axis, refactor
    [HideInInspector]
    public GameObject axis;
    /// <summary>
    /// Reference to continue frame
    /// </summary>
    [SerializeField]
    private ContinueListener continueListener = default;
    /// <summary>
    /// Reference to material switch object
    /// </summary>
    [SerializeField]
    private MaterialSwitcher materialSwitcher = default;
    /// <summary>
    /// Have we encountered singularity?
    /// </summary>
    private bool singularityEncounter = false;

    /// <summary>
    /// Sets up references
    /// </summary>
    private void Awake()
    {
        robotVisualAxis = FindObjectOfType<RobotController>().CurrentRobot.GetComponent<RobotVisualAxis>();
    }

    /// <summary>
    /// Gets references for robot axes
    /// </summary>
    public void SetUpAxisSingularity()
    {
        fithAxis = robotVisualAxis.GetAxis(5).transform.parent;
        fourthAxis = robotVisualAxis.GetAxis(4).transform.parent;
    }

    /// <summary>
    /// Finished step
    /// </summary>
    public void EndStep()
    {
        axis = robotVisualAxis.GetAxis(5);
        materialSwitcher.SwitchMaterial(axis.transform, Color.cyan);
        continueListener.EnableContinuation();
    }

    /// <summary>
    /// Detects if we have envountered singularity
    /// </summary>
    private void Update()
    {
        if (Mathf.Abs(fithAxis.transform.localEulerAngles.z - fourthAxis.transform.localEulerAngles.z) < 5f && !singularityEncounter)
        {
            EndStep();
            singularityEncounter = true;
            ErrorRequester.RaiseError("R-1004");
        }
        else if(Mathf.Abs(fithAxis.transform.localEulerAngles.z - fourthAxis.transform.localEulerAngles.z) > 5f)
        {
            singularityEncounter = false;
        }
    }
}
