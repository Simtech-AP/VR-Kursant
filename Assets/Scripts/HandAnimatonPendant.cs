using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to animate hand on pendant
/// </summary>
// TODO: Should we ever need this? This probably should be done only in HandAnimation class
public class HandAnimatonPendant : MonoBehaviour
{
    /// <summary>
    /// References to default right and left hand objects
    /// </summary>
    public GameObject defaultRightHand = default, defaultLeftHand = default;
    /// <summary>
    /// References to points in top, bottom, right and left endge points of touch screen
    /// </summary>
    public Transform rightHandTopLeft = default, rightHandTopRight = default, rightHandBottomLeft = default, rightHandBottomRight = default;
    /// <summary>
    /// Reference to touch screen governing object
    /// </summary>
    public TouchSerial touchSerial = default;
    /// <summary>
    /// Default position of hand
    /// </summary>
    private Vector3 defaultPosition = Vector3.zero;
    /// <summary>
    /// Default rotations of bone transforms
    /// </summary>
    private List<Quaternion> defaultRotations = new List<Quaternion>();
    /// <summary>
    /// Default positions of bone transforms
    /// </summary>
    private List<Vector3> defaultPositions = new List<Vector3>();

    /// <summary>
    /// Sets default rotations and positions of bones in default hands
    /// </summary>
    private void Start()
    {
        foreach (Transform t in defaultRightHand.GetComponentsInChildren<Transform>(true))
        {
            defaultRotations.Add(t.localRotation);
        }
        foreach (Transform t in defaultRightHand.GetComponentsInChildren<Transform>(true))
        {
            defaultPositions.Add(t.localPosition);
        }
        defaultPosition = defaultRightHand.transform.localPosition;
    }

    /// <summary>
    /// Continously updates position and rotation of bones and hand according to sent data
    /// </summary>
    private void Update()
    {
        if (touchSerial.state.touching)
        {
            var stateX = touchSerial.state.X / 1000f;
            var stateY = touchSerial.state.Y / 1000f;

            defaultRightHand.transform.localPosition = new Vector3(
                   (rightHandBottomLeft.transform.localPosition +
                   (rightHandBottomLeft.transform.localPosition - rightHandTopLeft.transform.localPosition) * stateY).x,
                   0,
                   (rightHandBottomLeft.transform.localPosition +
                   (rightHandBottomLeft.transform.localPosition - rightHandTopLeft.transform.localPosition) * stateX).z);
        }
        else
        {
            defaultRightHand.transform.localPosition = defaultPosition;
            for (int i = 0; i < defaultRightHand.GetComponentsInChildren<Transform>(true).Length; i++)
            {
                defaultRightHand.GetComponentsInChildren<Transform>(true)[i].localRotation = defaultRotations[i];
            }
            for (int i = 0; i < defaultRightHand.GetComponentsInChildren<Transform>(true).Length; i++)
            {
                defaultRightHand.GetComponentsInChildren<Transform>(true)[i].localPosition = defaultPositions[i];
            }
        }
    }
}
