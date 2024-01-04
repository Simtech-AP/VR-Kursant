using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contains logic for pendant menu targets
/// </summary>
public class PendantMenuTarget : StepEnabler
{
    [Header("Pendant Menu Target")]
    /// <summary>
    /// Menu to open
    /// </summary>
    public string ButtonMenu;
    /// <summary>
    /// Reference to continue enabler
    /// </summary>

    protected override void initialize()
    {
        SetUpButtonListener();
    }

    /// <summary>
    /// Sets up listeners
    /// </summary>
    public virtual void SetUpButtonListener()
    {
        MenuList.MenuOpened += FinishStep;
    }

    /// <summary>
    /// Finishes step
    /// </summary>
    /// <param name="obj">Name of menu</param>
    private void FinishStep(string obj)
    {
        if (obj.CompareTo(ButtonMenu) == 0)
        {
            EndStep();
        }
    }

    /// <summary>
    /// Removes listeners
    /// </summary>
    public virtual void CleanUpButtonListener()
    {
        MenuList.MenuOpened -= FinishStep;
    }

    protected override void cleanUp()
    {
        CleanUpButtonListener();
    }

    /// <summary>
    /// Enables continuation
    /// </summary>
    protected virtual void EndStep()
    {
        Enabled = true;
    }
}
