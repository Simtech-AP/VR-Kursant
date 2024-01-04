using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum LogicOperationType
{
    ALL,
    ANY,
    NOT_ANY
}

public class ButtonsPressedTarget : StepEnabler
{

    [Header("Buttons Pressed Target")]
    [SerializeField] private bool shouldCheckPushedOnInit = false;
    [SerializeField] private LogicOperationType conditionType;
    [SerializeField] private List<string> requiredButtonNames;
    private List<bool> buttonStateFlags;


    protected override void initialize()
    {
        buttonStateFlags = new List<bool>();
        buttonStateFlags.AddRange(Enumerable.Repeat(false, requiredButtonNames.Count));

        if (shouldCheckPushedOnInit)
        {
            checkForPushedButtons();
        }
    }

    private void checkForPushedButtons()
    {
        var pushedKeys = ControllersManager.Instance.GetController<InputController>().pushedKeys;

        for (int i = 0; i < requiredButtonNames.Count; i++)
        {
            if (pushedKeys.Contains(requiredButtonNames[i]))
            {
                OnButtonPressed(requiredButtonNames[i]);
            }
        }
    }

    public void OnButtonPressed(string name)
    {
        int index = requiredButtonNames.IndexOf(name);
        buttonStateFlags[index] = true;

        OnAnyButtonStateChanged();
    }

    public void OnButtonReleased(string name)
    {
        int index = requiredButtonNames.IndexOf(name);
        buttonStateFlags[index] = false;

        OnAnyButtonStateChanged();
    }

    private void OnAnyButtonStateChanged()
    {
        if (conditionType == LogicOperationType.ALL && buttonStateFlags.All((x) => x == true))
        {
            Enabled = true;
        }
        else if (conditionType == LogicOperationType.ANY && buttonStateFlags.Any((x) => x == true))
        {
            Enabled = true;
        }
        else if (conditionType == LogicOperationType.NOT_ANY && buttonStateFlags.All((x) => x == false))
        {
            Enabled = true;
        }
        else
        {
            Enabled = false;
        }
    }

    protected override void onUpdate()
    {
        OnAnyButtonStateChanged();
    }

    protected override void cleanUp()
    {
        buttonStateFlags.Clear();
    }

    public bool KeyPresent(string name)
    {
        return requiredButtonNames.Contains(name);
    }
    public bool GetKeyState(string name)
    {
        var index = requiredButtonNames.IndexOf(name);
        return buttonStateFlags[0];
    }

}
