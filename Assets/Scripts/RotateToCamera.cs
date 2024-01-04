using UnityEngine;

/// <summary>
/// Helper class for rotating object to face camera
/// </summary>
public class RotateToCamera : MonoBehaviour
{
    /// <summary>
    /// Reference to player camera
    /// </summary>
    private Transform playerCamera;

    [SerializeField]
    private Transform subject = default;


    /// <summary>
    /// Sets reference to camera
    /// </summary>
    void Start()
    {
        playerCamera = Camera.main.transform;
    }
    /// <summary>
    /// Rotate to camera every physics update
    /// </summary>
    void LateUpdate()
    {
        subject.rotation = Quaternion.LookRotation(subject.position - playerCamera.position, Vector3.up);
    }
}
