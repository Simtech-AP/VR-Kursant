using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Allows user to gaze on objects
/// </summary>
public class Gazer : MonoBehaviour
{
    [System.Serializable]
    private class raycastData
    {
        public float distance = default;
        public LayerMask mask = default;
    }

    /// <summary>
    /// Currently gazed object
    /// </summary>
    private GameObject currentlyGazed = default;
    /// <summary>
    /// Helper temporary structiore to hold raycasted items
    /// </summary>
    private RaycastHit hit = default;
    /// <summary>
    /// Gazed image progress shown on screen
    /// </summary>
    [SerializeField]
    private Image gazingProgressImage = default;
    /// <summary>
    /// Gazing progess in 0.0-1.0
    /// </summary>
    private float gazingProgress = default;
    /// <summary>
    /// Gaze time needed to finish gazing
    /// </summary>
    [SerializeField]
    private float gazeTime = default;

    [SerializeField]
    private raycastData rayData = default;


    /// <summary>
    /// Set up default parent and transform
    /// </summary>
    void Start()
    {
        transform.parent = Camera.main.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void SetGazeTime(float time)
    {
        gazeTime = time;
    }

    public void ToggleGuiCircle(bool isOn)
    {
        gazingProgressImage.enabled = isOn;
    }

    /// <summary>
    /// Detect gazing and run specified events
    /// </summary>
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayData.distance, rayData.mask))
        {
            StartGazeProcess();
        }
        else
        {
            StopGazingProcess();
        }

        AdvanceGazingProcess();
    }

    private void StopGazingProcess()
    {
        if (currentlyGazed)
        {
            var gazable = currentlyGazed.GetComponent<Gazable>();


            if (gazable)
            {
                gazable.stoppedGazing.Invoke();
            }
            currentlyGazed = null;
            gazingProgressImage.fillAmount = 0f;
            gazingProgress = 0f;
        }
    }

    private void AdvanceGazingProcess()
    {
        if (currentlyGazed)
        {
            var gazable = currentlyGazed.GetComponent<Gazable>();

            gazingProgressImage.fillAmount = gazingProgress;
            gazingProgressImage.gameObject.transform.position = hit.point;
            gazingProgressImage.gameObject.transform.LookAt(transform.parent, Vector3.up);
            var scale = Vector3.Distance(transform.parent.position, hit.point) * 0.5f;
            gazingProgressImage.transform.localScale = new Vector3(scale, scale, scale);
            gazingProgress += Time.deltaTime / gazeTime;
            if (gazingProgress >= 1 && gazable)
            {
                gazable.finishedGazing.Invoke();
                gazingProgress = 0f;
            }
            else if (gazingProgress >= 1)
            {
                Destroy(currentlyGazed);
                gazingProgress = 0f;
            }
        }
    }

    private void StartGazeProcess()
    {

        if (currentlyGazed && currentlyGazed != hit.collider.gameObject)
        {
            var gazable = currentlyGazed.GetComponent<Gazable>();

            if (gazable)
            {
                gazable.stoppedGazing.Invoke();
            }

            currentlyGazed = hit.collider.gameObject;
            gazingProgress = 0f;

            if (gazable)
            {
                gazable.startedGazing.Invoke();
            }
        }
        else if (!currentlyGazed)
        {

            currentlyGazed = hit.collider.gameObject;
            var gazable = currentlyGazed.GetComponent<Gazable>();
            gazingProgress = 0f;

            if (gazable)
            {
                gazable.startedGazing.Invoke();
            }
        }
    }
}
