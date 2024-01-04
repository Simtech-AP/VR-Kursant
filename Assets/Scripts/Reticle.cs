using UnityEngine;

/// <summary>
/// Class rendering a reticle at center of a screen
/// </summary>
public class Reticle : MonoBehaviour
{
    /// <summary>
    /// Structure holding raycast hit data
    /// </summary>
    private RaycastHit hit;
    /// <summary>
    /// Reference to players camera
    /// </summary>
    private Transform vrCamera;

    /// <summary>
    /// Sets up reference to camera
    /// </summary>
    private void Start()
    {
        vrCamera = Camera.main.transform;
    }

    /// <summary>
    /// Continously hits elements in front of camera
    /// </summary>
    void Update()
    {
        if (Physics.Raycast(vrCamera.position, vrCamera.forward, out hit, 20f, ~0, QueryTriggerInteraction.Ignore))
        {
            transform.position = hit.point;
            transform.localScale = Vector3.one * hit.distance * 0.001f;
        }
        else
        {
            transform.position = vrCamera.position + vrCamera.forward * 20f;
            transform.localScale = Vector3.one * 20f * 0.001f;
        }
    }
}
