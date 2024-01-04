using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DelayedEvent : MonoBehaviour
{
    [SerializeField]
    private float delayTime = default;

    [SerializeField]
    private bool shouldRunOnAwake = default;

    [SerializeField]
    private UnityEvent timedEvent = default;

    private Coroutine delayProcess = default;

    private void Awake()
    {
        if (shouldRunOnAwake)
        {
            Run();
        }
    }

    public void Run()
    {
        delayProcess = StartCoroutine(delayProcedure());
    }

    public void Cancel()
    {
        StopCoroutine(delayProcess);
    }

    private IEnumerator delayProcedure()
    {
        yield return new WaitForSeconds(delayTime);

        timedEvent.Invoke();
    }

}