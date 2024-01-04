using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotReposition : MonoBehaviour
{
    [SerializeField]
    private Transform pivot = default;

    [SerializeField]
    private Transform container = default;

    [SerializeField]
    private Transform antiContainer = default;


    private void Awake()
    {
        var pivotPos = pivot.localPosition;
        container.localPosition += pivotPos;
        antiContainer.localPosition -= pivotPos;
    }
}
