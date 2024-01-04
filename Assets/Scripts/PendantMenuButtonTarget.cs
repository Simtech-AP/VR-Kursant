using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PendantMenuButtonTarget : PendantMenuTarget
{
    public int ButtonIndex;
    private List<UnityEvent> events;

    public override void SetUpButtonListener()
    {
        MenuList.MenuInteracted += FinishStep;
    }

    private void FinishStep(string name, List<UnityEvent> events)
    {
        if (ButtonMenu.Equals(name))
        {
            this.events = events;
            this.events[ButtonIndex].AddListener(EndStep);
        }
    }

    protected override void EndStep()
    {
        this.events[ButtonIndex].RemoveListener(EndStep);
        Enabled = true;
    }

    public override void CleanUpButtonListener()
    {
        MenuList.MenuInteracted -= FinishStep;
    }
}
