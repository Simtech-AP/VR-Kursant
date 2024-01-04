using UnityEngine;

public enum ToolState
{
    TOOL_ON,
    TOOL_OFF
}

public class ToolStateTarget : StepEnabler
{
    [Header("Tool State Target")]
    [SerializeField] private ToolState targetState;
    [SerializeField] private bool enforceStateChange;   //if true -> you have to change state at least once
    [SerializeField] private bool requireOnce;          //if true -> once you reach Enable = true, it won't change

    private Grasper grasper;
    private ToolState initialState;
    private bool wasStateChanged = false;

    protected override void initialize()
    {
        grasper = ControllersManager.Instance.GetController<RobotController>().CurrentRobot.RobotGrasper;
        initialState = grasper.Closed ? ToolState.TOOL_ON : ToolState.TOOL_OFF;
    }

    public void CheckForTargetState()
    {
        var currentState = grasper.Closed ? ToolState.TOOL_ON : ToolState.TOOL_OFF;

        if (!wasStateChanged && currentState != initialState)
        {
            wasStateChanged = true;
        }

        if (enforceStateChange && wasStateChanged && currentState == targetState)
        {
            Enabled = true;
        }
        else if (!enforceStateChange && currentState == targetState)
        {
            Enabled = true;
        }
        else if (!requireOnce)
        {
            Enabled = false;
        }
    }

    protected override void onUpdate()
    {
        CheckForTargetState();
    }
}