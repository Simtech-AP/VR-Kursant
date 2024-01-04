/// <summary>
/// Gazable target for TCP point
/// </summary>
public class TCPGazableTarget : GazableTarget
{
    /// <summary>
    /// Reference to TCP pointer on scene
    /// </summary>
    public TCPPointer tcpPointer;
    /// <summary>
    /// Reference to TCP point 
    /// </summary>
    public TCPPoint tCPPoint;

    /// <summary>
    /// Enables objects and sets up gazable targets
    /// </summary>
    protected override void initialize()
    {
        tcpPointer.arrow.SetActive(true);
        tCPPoint.arrow.SetActive(true);

        objectsToGaze.Add(tcpPointer.gameObject);
        objectsToGaze.Add(tCPPoint.gameObject);

        EnableGazableTarget();
    }

    /// <summary>
    /// Enables gazable targets
    /// </summary>
    public void EnableGazableTarget()
    {
        SetUpGazables();
        gazables[0].finishedGazing.AddListener(() => tcpPointer.arrow.SetActive(false));
        gazables[1].finishedGazing.AddListener(() => tCPPoint.arrow.SetActive(false));
    }

    /// <summary>
    /// Disables gazables
    /// </summary>
    protected override void cleanUp()
    {
        tcpPointer.arrow.SetActive(false);
        tCPPoint.arrow.SetActive(false);
    }
}
