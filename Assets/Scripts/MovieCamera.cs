using UnityEngine;

public class MovieCamera : MonoBehaviour
{
    private Transform mainCamera = default;
    private Vector3 positionVelocity = default;

    void Start()
    {
        mainCamera = Camera.main.transform;
    }

    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, mainCamera.position, ref positionVelocity, 0.2f);
        transform.rotation = Quaternion.Slerp(transform.rotation, mainCamera.rotation, 0.1f);
    }
}
