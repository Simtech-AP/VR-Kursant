using UnityEngine;

/// <summary>
/// Object following pivot 6 of robot and reflecting its position in local space
/// </summary>
public class FollowPivot6 : MonoBehaviour
{
    /// <summary>
    /// Reference to pivot 6 of robot
    /// </summary>
    [SerializeField]
    private Transform pivot6 = default;
    /// <summary>
    /// Refence to default rotation of robot, meaning rotation 0,0,0 of pivot 6
    /// </summary>
    [SerializeField]
    private Transform referenceRotationPoint = default;
    /// <summary>
    /// Relative rotation of pivot 6 in reference to referenceRotationPoint
    /// </summary>
    [SerializeField]
    private Vector3 relativeRotation = default;
    /// <summary>
    /// Offset of ending of tool attached to robot
    /// </summary>
    [SerializeField]
    public float Offset = 0.85f;
    /// <summary>
    /// Reference to robot on scene
    /// </summary>
    private Robot robot;
    /// <summary>
    /// Forward vecotr of robot 6th pivot
    /// </summary>
    private Vector3 forward = Vector3.zero;

    /// <summary>
    /// Sets offset according to set length of tool attached to robot
    /// </summary>
    private void Start()
    {
        var matrixRobot = FindObjectOfType<MatrixRobotController>();
        if (matrixRobot != null)
            Offset = matrixRobot.lambda6;
    }

    public void LinkRobot(Robot robot)
    {
        this.robot = robot;
    }

    /// <summary>
    /// Continously updates relative position and rotation of this object to reference posion and rotation of robot
    /// </summary>
    private void Update()
    {
        if (robot as MatrixRobot)
            forward = -pivot6.transform.forward;
        if (robot as JointRobot)
            forward = -pivot6.transform.right;
        transform.position = pivot6.position + forward * Offset;
        transform.rotation = pivot6.rotation;
        relativeRotation = referenceRotationPoint.localEulerAngles - transform.localEulerAngles;
    }

    /// <summary>
    /// Gets current position of 6th pivot
    /// </summary>
    /// <param name="offset">Offset of pivot point</param>
    /// <returns>Current pivot position inclufing offset</returns>
    public Vector3 GetPosition(float offset)
    {
        Offset = offset;
        forward = -pivot6.transform.right;
        return pivot6.position + forward * Offset;
    }

    public Quaternion GetRotation()
    {
        return Quaternion.Euler(transform.eulerAngles) * Quaternion.AngleAxis(90, Vector3.right) * Quaternion.AngleAxis(-90, Vector3.up);
    }
}
