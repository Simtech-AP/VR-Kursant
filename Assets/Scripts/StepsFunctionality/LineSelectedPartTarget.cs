using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSelectedPartTarget : StepEnabler
{
    [Header("Line Selected Part Target")]
    [SerializeField] private int targetSelectedPartIndex;
    private PendantController pController = null;

    protected override void initialize()
    {
        pController = ControllersManager.Instance.GetController<PendantController>();
    }

    protected override void onUpdate()
    {
        AssertIndex();
    }
    public void AssertIndex()
    {
        if (pController.SelectedPart == targetSelectedPartIndex)
        {
            Enabled = true;
        }
        else
        {
            Enabled = false;
        }
    }
}
