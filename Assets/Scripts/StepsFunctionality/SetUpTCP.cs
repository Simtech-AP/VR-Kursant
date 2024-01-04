using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows setting up TCP point
/// </summary>
public class SetUpTCP : MonoBehaviour
{
    /// <summary>
    /// Reference to attacher of positions
    /// </summary>
    [SerializeField]
    private PositionAttacher positionAttacher = default;
    /// <summary>
    /// Referenc to TCP point object on scene
    /// </summary>
    [SerializeField]
    private TCPPoint tcpPoint = default;
    /// <summary>
    /// Reference to TCP pointer on scene
    /// </summary>
    [SerializeField]
    private TCPPointer tcpPointer = default;
    /// <summary>
    /// Default TCP pointer position
    /// </summary>
    public Vector3 PointerPoisition = new Vector3(3.6f, 1f, -2.9f);

    /// <summary>
    /// List of points for tool for each robot
    /// </summary>
    private Dictionary<System.Type, Vector3> GrasperPoints = new Dictionary<System.Type, Vector3>()
    {
        { typeof(JointRobot), new Vector3(-0.3546f, 0.0182f, 0.020f) },
        { typeof(MatrixRobot), new Vector3(-0.178f, 0.216f, -3.983f) }
    };

    /// <summary>
    /// Sets up references and default values
    /// </summary>
    private void Awake()
    {
        Grasper grasper = FindObjectOfType<RobotController>().CurrentRobot.GetComponentInChildren<Grasper>();

         positionAttacher.SetAttachments(new List<Attachment>()
            {
                new Attachment(new List<Attacher>() {new Attacher(tcpPoint.gameObject, GrasperPoints[FindObjectOfType<Robot>().GetType()]) }, grasper.transform),
                new Attachment(new List<Attacher>() {new Attacher(tcpPointer.gameObject, PointerPoisition) }, null)
            });
    }
}
