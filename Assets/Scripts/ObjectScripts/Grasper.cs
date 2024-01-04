using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System.Linq;

/// <summary>
/// Class for grasper/holder object attached to robot
/// </summary>
public class Grasper : RobotTool
{
    /// <summary>
    /// Right side of grasper, hook
    /// </summary>
    [SerializeField]
    private GameObject rightSide = default;
    /// <summary>
    /// Left side of grasper, hook
    /// </summary>
    [SerializeField]
    private GameObject leftSide = default;
    /// <summary>
    /// Is the grasper closed?
    /// </summary>
    private bool closed = false;

    public bool Closed { get { return closed; } }

    /// <summary>
    /// Currently held object
    /// </summary>
    public GameObject currentTakenObject;
    /// <summary>
    /// Offset between closed and opened sides
    /// </summary>
    [SerializeField]
    private float openOffset = 0.1f;
    /// <summary>
    /// Default closed position of right side of grasper on axis Y
    /// </summary>
    private float closedPostionRight;
    /// <summary>
    /// Default closed position of left side of grasper on axis Y
    /// </summary>
    private float closedPositionLeft;
    /// <summary>
    /// Event to start audio clip on tool closing
    /// </summary>
    [SerializeField]
    private UnityEvent onToolClose = default;
    /// <summary>
    /// Event to start audio clip on tool opening
    /// </summary>
    [SerializeField]
    private UnityEvent onToolOpen = default;

    private bool inAction = false;

    public bool IsHoldingObject
    {
        get { return currentTakenObject != null; }
    }

    /// <summary>
    /// Sets default positions for closed positions of sides
    /// </summary>
    private void Start()
    {
        closedPositionLeft = leftSide.transform.localPosition.y;
        closedPostionRight = rightSide.transform.localPosition.y;
    }

    /// <summary>
    /// Uses tool according to current state
    /// </summary>
    public override void UseTool()
    {
        if (ErrorRequester.HasAllErrorsReset() && ErrorRequester.HasResetAlarmErrors())
        {
            if (closed)
            {
                ToolOn();
            }
            else
            {
                ToolOff();
            }
        }
    }

    /// <summary>
    /// Changes tool state to on and releases picked object
    /// </summary>
    public override void ToolOn()
    {
        if (inAction) return;

        if (!closed)
        {
            closed = true;
            rightSide.transform.DOLocalMoveY(closedPostionRight - openOffset, 0.5f);
            leftSide.transform.DOLocalMoveY(closedPositionLeft + openOffset, 0.5f);

            onToolClose.Invoke();

            StartCoroutine(CheckIfTookAnObject());
        }

    }



    /// <summary>
    /// Changes tool state to off and picks up object
    /// </summary>
    public override void ToolOff()
    {
        if (inAction) return;

        if (closed)
        {
            closed = false;

            rightSide.transform.DOLocalMoveY(closedPostionRight, 0.5f);
            leftSide.transform.DOLocalMoveY(closedPositionLeft, 0.5f);

            onToolOpen.Invoke();

            if (currentTakenObject)
            {
                StartCoroutine(ReleaseObject());
            }
        }
    }

    /// <summary>
    /// After a delay checks if any item has been picked up bu grasper
    /// </summary>
    /// <returns>Enumerator handle</returns>
    private IEnumerator CheckIfTookAnObject()
    {
        inAction = true;
        yield return new WaitForSeconds(0.25f);

        List<GameObject> rightObjects = rightSide.GetComponentInChildren<GrasperSide>().objectsInGrasp;
        List<GameObject> leftObjects = leftSide.GetComponentInChildren<GrasperSide>().objectsInGrasp;

        List<GameObject> objectsInBothClamps = rightObjects.Intersect(leftObjects).ToList();

        if (objectsInBothClamps.Count > 0)
        {
            currentTakenObject = objectsInBothClamps[0];
            GetComponentInChildren<FixedJoint>().connectedBody = currentTakenObject.GetComponent<Rigidbody>();
            currentTakenObject.GetComponent<Rigidbody>().useGravity = false;
            currentTakenObject.GetComponent<ToolInteractedObjectMaterialAdjuster>().OnInteractionStarted();
            currentTakenObject.GetComponent<RobotInteractiveObject>().isInInteraction = true;
            currentTakenObject.layer = 16;
        }

        yield return new WaitForSeconds(0.25f);
        inAction = false;
    }

    private IEnumerator ReleaseObject()
    {
        inAction = true;
        yield return new WaitForSeconds(0.25f);

        GetComponentInChildren<FixedJoint>().connectedBody = null;
        currentTakenObject.GetComponent<RobotInteractiveObject>().isInInteraction = false;
        currentTakenObject.layer = 15;
        currentTakenObject.GetComponent<Rigidbody>().useGravity = true;
        currentTakenObject.GetComponent<ToolInteractedObjectMaterialAdjuster>().OnInteractionEnded();
        currentTakenObject.transform.parent = null;
        currentTakenObject = null;

        yield return new WaitForSeconds(0.25f);
        inAction = false;
    }
}
