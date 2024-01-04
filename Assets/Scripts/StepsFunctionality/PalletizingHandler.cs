using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Handles palletization task
/// </summary>
public class PalletizingHandler : MonoBehaviour
{
    [Serializable]
    public class ConveyorData
    {
        public Transform conveyorBelt = default;
        public Conveyor conveyorLogic = default;
        public float operationDuration = default;
        public float operationSpeed = default;
        public float operationDelay = default;
        public Vector3 originPosition = default;

        public UnityEvent OnStart = default;
        public UnityEvent OnStop = default;

    }


    /// <summary>
    /// Reference to palette in scene
    /// </summary>
    [SerializeField]
    private GameObject targetObject = default;


    public ConveyorData leftConveyor = default;

    public ConveyorData rightConveyor = default;

    public void DeployObject(int sideIndex)
    {
        if (sideIndex == 0)
        {
            AttachObject(leftConveyor);
            StartCoroutine(deployOnConveyor(leftConveyor));
        }
        else
        {
            AttachObject(rightConveyor);
            StartCoroutine(deployOnConveyor(rightConveyor));
        }
    }

    public void RetractAndDeployObject()
    {
        StartCoroutine(retractAndDeployObject());
    }

    private void AttachObject(ConveyorData conveyor)
    {
        targetObject.transform.parent = conveyor.conveyorBelt;
        targetObject.transform.localPosition = conveyor.originPosition - new Vector3(0, 0, conveyor.conveyorBelt.localPosition.z);
        targetObject.transform.localRotation = Quaternion.Euler(-90, 0, 0);

        var r = targetObject.GetComponent<Rigidbody>();
        r.isKinematic = false;
        r.useGravity = true;
    }

    private IEnumerator retractAndDeployObject()
    {
        yield return StartCoroutine(retractConveyor(rightConveyor));

        AttachObject(leftConveyor);

        yield return StartCoroutine(deployOnConveyor(leftConveyor));

    }

    private IEnumerator deployOnConveyor(ConveyorData conveyor)
    {

        yield return new WaitForSeconds(conveyor.operationDelay);

        conveyor.conveyorLogic.Launch(conveyor.operationSpeed);
        conveyor.OnStart.Invoke();

        yield return new WaitForSeconds(conveyor.operationDuration);

        conveyor.conveyorLogic.Stop();
        conveyor.OnStop.Invoke();
    }

    private IEnumerator retractConveyor(ConveyorData conveyor)
    {
        yield return new WaitForSeconds(conveyor.operationDelay);

        conveyor.conveyorLogic.Launch(-conveyor.operationSpeed);
        conveyor.OnStart.Invoke();

        yield return new WaitForSeconds(conveyor.operationDuration);

        conveyor.conveyorLogic.Stop();
        conveyor.OnStop.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DeployObject(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DeployObject(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RetractAndDeployObject();
        }
    }
}
