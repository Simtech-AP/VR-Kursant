using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReparentToPendant : MonoBehaviour
{
    [SerializeField]
    private Transform objectToReparent = default;

    private Transform pendantObject = default;

    private void Awake()
    {
        pendantObject = ControllersManager.Instance.GetController<PendantController>().transform.parent;
    }

    public void Reparent()
    {
        objectToReparent.parent = pendantObject;
        objectToReparent.localPosition = Vector3.zero;
        objectToReparent.localRotation = Quaternion.identity;
    }

    public void CleanUp()
    {
        Destroy(objectToReparent.gameObject);
    }
}
