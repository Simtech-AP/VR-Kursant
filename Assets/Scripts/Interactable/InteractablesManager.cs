using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Types of buttons enumeration
/// </summary>
public enum ButtonType
{
    PENDANT,
    ESTOP,
    ALARM_RESET,
    SECUIRTY_RESET,
    TELEPORT,
    NEXT_STEP,
    START,
    STOP,
    MODE_KEY
}

/// <summary>
/// Base class for button type and object
/// </summary>
//TDO: Cans this be a Struct?
[System.Serializable]
public class Button
{
    public ButtonType Type;
    public GameObject Object;
}

/// <summary>
/// Manager class for storing interactables components in application
/// </summary>
public class InteractablesManager : Singleton<InteractablesManager>
{
    /// <summary>
    /// List of Interactables
    /// </summary>
    [SerializeField]
    private List<Interactable> interactables = default;
    /// <summary>
    /// Public accessor for Intractables list
    /// </summary>
    public List<Interactable> Interactables { get => interactables; }
    /// <summary>
    /// List of Button objects
    /// </summary>
    [SerializeField]
    private List<Button> buttons = default;
    /// <summary>
    /// Public acessor for Buttons list
    /// </summary>
    public List<Button> Buttons { get => buttons; }

    /// <summary>
    /// Initalize class
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
    }

#if UNITY_EDITOR
    /// <summary>
    /// Context menu method for loading interactables objects
    /// </summary>
    [ContextMenu("Get all interactables")]
    public void GetAllInteratables()
    {
        Interactables.Clear();
        interactables = FindObjectsOfType<Interactable>().ToList();
    }
#endif

    /// <summary>
    /// Gets interactable component
    /// </summary>
    /// <typeparam name="T"> Type of returned component </typeparam>
    /// <returns> Desired Interactable </returns>
    public T GetInteractableBehaviour<T>() where T : MonoBehaviour
    {
        for (int i = 0; i < Interactables.Count; ++i)
        {
            T component = Interactables[i].GetComponent<T>();
            if (component)
                return component;
        }
        return null;
    }

    /// <summary>
    /// Gets array of Interactable components
    /// </summary>
    /// <typeparam name="T"> Type of returned components </typeparam>
    /// <returns> Desired Interactables </returns>
    public T[] GetAllInteractableBehaviour<T>() where T : MonoBehaviour
    {
        List<T> ret = new List<T>();
        for (int i = 0; i < Interactables.Count; ++i)
        {
            T component = Interactables[i].GetComponent<T>();
            if (component)
                ret.Add(component);
        }
        return ret.ToArray();
    }

    public GameObject GetInteractableButton(ButtonType type)
    {
        foreach (var button in buttons)
        {
            if (button.Type == type)
            {
                return button.Object;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets List of Button components of type
    /// </summary>
    /// <param name="type">Type of buttons to get</param>
    /// <returns>List of buttons of specified type</returns>
    public List<GameObject> GetAllInteractableButton(ButtonType type)
    {
        List<GameObject> go = new List<GameObject>();
        var buttons = Buttons.FindAll(x => x.Type == type);
        for (int i = 0; i < buttons.Count; ++i)
        {
            go.Add(buttons[i].Object);
        }
        return go;
    }
}
