using System.Collections.Generic;
using UnityEngine;

public class CustomizedGazableTarget : GazableTarget
{
    [Header("Customized Gazable Target")]
    [SerializeField] protected List<Vector3> colliderCenter;
    [SerializeField] protected List<Vector3> colliderSize;

    protected void SetCustomColliders()
    {
        setCustomColliders(colliderSize, colliderCenter);
    }
}