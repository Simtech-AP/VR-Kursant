using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadmanAnimation : MonoBehaviour
{
    public GameObject Obj;
    public Vector3 DestRotation;

    public void Rotate(bool flag) => Obj.transform.DOLocalRotate(flag == true ? DestRotation : Vector3.zero, 0.25f);
    
}
