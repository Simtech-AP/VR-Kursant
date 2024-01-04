using UnityEngine;

public enum DirectionType
{
    LEFT,
    RIGHT,
    UP,
    DOWN,
    FORWARD,
    BACK
}

public class EndPointDirectionChecker : StepEnabler
{
    [Header("End Point Direction Checker")]
    [SerializeField] private DirectionType direction;
    [SerializeField] private int angleThreshold;
    private GameObject endPoint;

    protected override void initialize()
    {
        endPoint = ControllersManager.Instance.GetController<RobotController>().CurrentRobot.RobotEndPoint;
    }

    protected override void onUpdate()
    {
        CheckForDesiredDirection();
    }

    public void CheckForDesiredDirection()
    {
        var endPointForward = endPoint.transform.forward;
        float angle = 180f;
        switch (direction)
        {
            case DirectionType.FORWARD:
                angle = Vector3.Angle(Vector3.forward, endPointForward);
                break;
            case DirectionType.BACK:
                angle = Vector3.Angle(Vector3.back, endPointForward);
                break;
            case DirectionType.RIGHT:
                angle = Vector3.Angle(Vector3.right, endPointForward);
                break;
            case DirectionType.LEFT:
                angle = Vector3.Angle(Vector3.left, endPointForward);
                break;
            case DirectionType.UP:
                angle = Vector3.Angle(Vector3.up, endPointForward);
                break;
            case DirectionType.DOWN:
                angle = Vector3.Angle(Vector3.down, endPointForward);
                break;
        }

        if (angle < angleThreshold)
        {
            Enabled = true;
        }
        else
        {
            Enabled = false;
        }
    }

}