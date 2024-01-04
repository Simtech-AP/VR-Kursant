using System.Collections;
using UnityEngine;

public class RightHandUnlockController : StepEnabler
{
    [Header("Right Hand Unlock Controller")]
    [SerializeField] private float unlockWaitTime;
    [SerializeField] private float rightHandCheckWaitTime;
    /// <summary>
    /// Right hand object
    /// </summary>
    private GameObject rightHand = default;
    /// <summary>
    /// Checker to hlad right hand data
    /// </summary>
    private GameObject rightHandChecker = default;
    /// <summary>
    /// Timer for unlocking functionality waiting
    /// </summary>
    private WaitForSeconds unlockWaitTimer;
    /// <summary>
    /// Timer for right hand functionality waiting
    /// </summary>
    private WaitForSeconds rightHandCheckWaitTimer;


    /// <summary>
    /// Initalizes timers
    /// </summary>
    protected override void initialize()
    {
        unlockWaitTimer = new WaitForSeconds(unlockWaitTime);
        rightHandCheckWaitTimer = new WaitForSeconds(rightHandCheckWaitTime);
        rightHand = FindObjectOfType<RightHandController>().gameObject.GetComponentInChildren<InteractGlove>(true).gameObject;
        rightHandChecker = rightHand.AddComponent<RightHandCheker>().gameObject;
        rightHandChecker.GetComponent<RightHandCheker>().rightHandUnlock = this;
    }

    /// <summary>
    /// Unlock second instruction after a time
    /// </summary>
    /// <returns>Wait for half a second</returns>
    IEnumerator Unlock()
    {
        yield return unlockWaitTimer;
        rightHandChecker.SetActive(true);
    }

    /// <summary>
    /// Has the righ hand been activated? If so start counting time
    /// </summary>
    public void CheckRightHand()
    {
        StopAllCoroutines();
        StartCoroutine(CheckRightHandCor());
    }

    /// <summary>
    /// Reset counting time fot right hand
    /// </summary>
    public void ResetCounterForRightHand()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// Start counting right hand time
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckRightHandCor()
    {
        yield return rightHandCheckWaitTimer;
        if (rightHand.activeInHierarchy)
            ShowSecondPart();
    }

    /// <summary>
    /// Show second part of tutorial
    /// </summary>
    private void ShowSecondPart()
    {
        StartCoroutine(Unlock());
        Enabled = true;
        Destroy(rightHand.GetComponent<RightHandCheker>());
    }
}
