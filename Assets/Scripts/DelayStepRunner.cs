using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayStepRunner : StepEnabler
{
    [Header("Delay Step Runner")]
    public int DelayTime;

    protected override void initialize()
    {
        DelayStep();
    }

    public async void DelayStep()
    {
        await System.Threading.Tasks.Task.Delay(DelayTime * 1000);
        Enabled = true;
    }
}
