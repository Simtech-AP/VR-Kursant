using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// Controller allowing to process inputs from pendant/keyboard/gamepad
/// </summary>
public class InputController : Controller
{
    /// <summary>
    /// List containing all debug bindings for emulation of input with keyboard
    /// </summary>
    [SerializeField]
    private List<InputBind> bindings = new List<InputBind>();
    /// <summary>
    /// List containing all curently pushed keys
    /// </summary>
    public List<string> pushedKeys = new List<string>();
    /// <summary>
    /// List of every active container of binds in scene
    /// </summary>
    private List<InputContainer> binds = new List<InputContainer>();
    /// <summary>
    /// Is any key down?
    /// </summary>
    public bool isAnyKeyDown = false;

    /// <summary>
    /// Adds container to active inputs
    /// </summary>
    /// <param name="container">Container to add to active inputs</param>
    public void AddBinds(InputContainer container)
    {
        binds.Add(container);
    }

    /// <summary>
    /// Removes container from active inputs
    /// </summary>
    /// <param name="container">Container to remove from active inputs</param>
    public void RemoveBinds(InputContainer container)
    {
        binds.Remove(container);
    }

    /// <summary>
    /// Allows to get key up and down events from keyboard and process them
    /// </summary>
    private void OnGUI()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown)
        {
            foreach (InputBind ib in bindings)
            {
                if (ib.inputName == e.keyCode.ToString())
                {
                    if (!pushedKeys.Contains(e.keyCode.ToString()))
                    {
                        ib.OnPress.Invoke();
                        pushedKeys.Add(e.keyCode.ToString());
                    }
                }
            }
        }
        else if (e.type == EventType.KeyUp)
        {
            foreach (InputBind ib in bindings)
            {
                if (ib.inputName == e.keyCode.ToString())
                {
                    if (pushedKeys.Contains(e.keyCode.ToString()))
                        pushedKeys.Remove(e.keyCode.ToString());
                    ib.OnRelease.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// Uses bind of a specified name
    /// Allows also to pass state if the bind was pressed down or up
    /// </summary>
    /// <param name="name">Name of a input to send the pressed event</param>
    /// <param name="press">Was the input pressed or released?</param>
    public void UseButton(string name, bool press)
    {
        List<InputContainer> bindsCopy = new List<InputContainer>();
        for (int i = 0; i < binds.Count; i++)
        {
            bindsCopy.Add(binds[i]);
        }
        for (int i = 0; i < bindsCopy.Count; ++i)
        {
            foreach (InputBind ib in bindsCopy[i].binds)
            {
                if (ib.inputName == name)
                {
                    if (press)
                    {
                        isAnyKeyDown = true;

                        if (!pushedKeys.Contains(name))
                        {
                            pushedKeys.Add(name);
                            Debug.Log(name);
                        }

                        ib.OnPress.Invoke();
                    }
                    else
                    {
                        if (pushedKeys.Contains(name))
                        {
                            pushedKeys.Remove(name);
                        }

                        ib.OnRelease.Invoke();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Resets any key pressed flag
    /// </summary>
    private void LateUpdate()
    {
        isAnyKeyDown = false;
    }
}
