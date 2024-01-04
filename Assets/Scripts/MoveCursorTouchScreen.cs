using System.Collections;
using UnityEngine;

/// <summary>
/// Touch screen cursor visualization - probably obosolete now
/// </summary>
// TODO: Check if we even use it now anywhere, since the pendant is with sensors this probably has no place in project
public class MoveCursorTouchScreen : MonoBehaviour
{
    private TouchSerial serialData;
    [SerializeField]
    private BoxCollider bounds = default;
    [SerializeField]
    private float xExtents = default, yExtents = default;
    private Vector3 velocity;
    private bool locked;
    private float timer = 0f;
    private float inputDelay = 0.4f;
    public bool leftHand = false;
    private string lastButton = "";
    [SerializeField]
    private GameObject hand = default;
    [SerializeField]
    private GameObject defaultHand = default;
    private bool coroutineStarted = false;

    void Start()
    {
        serialData = FindObjectOfType<TouchSerial>();
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (leftHand)
            GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", Color.red);
        if (serialData.state.touching && serialData.state.X != 0 && serialData.state.Y != 0 && serialData.state.leftHand == leftHand)
        {
            if (coroutineStarted)
            {
                StopAllCoroutines();
                coroutineStarted = false;
            }
            GetComponentInChildren<MeshRenderer>().enabled = true;
            defaultHand.SetActive(false);
            hand.SetActive(true);
            Vector3 position = new Vector3();
            position.z = bounds.transform.localPosition.z + xExtents * -((serialData.state.X - 500) / 400f);
            position.x = bounds.transform.localPosition.x + yExtents * -((serialData.state.Y - 500) / 400f);
            position.y = bounds.bounds.center.y;
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(position.x, 0.046f, position.z), ref velocity, 0.02f);
            if (serialData.pressed && timer >= inputDelay)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, -transform.up, out hit, 1f, 1 << 10))
                {
                    // FindObjectOfType<InputController>().PressButton(hit.collider.name);
                    lastButton = hit.collider.name;
                }
                if (hit.collider && !(hit.collider.name.Contains("+") || hit.collider.name.Contains("-")))
                {
                    serialData.pressed = false;
                    timer = 0f;
                }
            }
        }
        else if (!serialData.state.touching && lastButton != "" && serialData.state.leftHand == leftHand)
        {
            if (coroutineStarted)
            {
                StopAllCoroutines();
                coroutineStarted = false;
            }
            // FindObjectOfType<InputController>().ReleaseButton(lastButton);
            lastButton = "";
        }
        else if (!serialData.state.touching && serialData.state.leftHand == leftHand)
        {
            if (!coroutineStarted)
                StartCoroutine(LeaveHandVisibleForTime());
            //GetComponentInChildren<MeshRenderer>().enabled = false;
            //defaultHand.SetActive(true);
            //hand.SetActive(false);
        }
    }

    private IEnumerator LeaveHandVisibleForTime()
    {
        coroutineStarted = true;
        yield return new WaitForSeconds(inputDelay);
        GetComponentInChildren<MeshRenderer>().enabled = false;
        defaultHand.SetActive(true);
        hand.SetActive(false);
        coroutineStarted = false;
    }
}
