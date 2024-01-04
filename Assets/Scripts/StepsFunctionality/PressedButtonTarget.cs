using QuickOutline;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains logic for buttons to press by user
/// </summary>
public class PressedButtonTarget : StepEnabler
{
    [Header("Pressed Button Target")]
    /// <summary>
    /// Type of button to press
    /// </summary>
    [SerializeField]
    protected ButtonType button;
    /// <summary>
    /// Are we checking multiple buttons?
    /// </summary>
    [SerializeField]
    protected bool multipleButtons = false;
    /// <summary>
    /// Is the button we are checking a button on pendant?
    /// </summary>
    [SerializeField]
    protected bool isPendantButton = false;
    /// <summary>
    /// Reference to input container
    /// </summary>
    private InputContainer inputContainer = default;
    /// <summary>
    /// List of button gameobjects on scene
    /// </summary>
    private List<GameObject> buttons = new List<GameObject>();
    /// <summary>
    /// List of outlines for buttons
    /// </summary>
    private List<Outline> objectToHighlightOutlineList = new List<QuickOutline.Outline>();
    /// <summary>
    /// List of flags for buttons
    /// </summary>
    private Dictionary<string, bool> buttonsFlags = new Dictionary<string, bool>();
    /// <summary>
    /// Public accessor for buttons flags
    /// </summary>
    public Dictionary<string, bool> ButtonsFlags { get => buttonsFlags; }

    /// <summary>
    /// Sets references according to if button is on pendant
    /// </summary>
    protected override void initialize()
    {
        if (isPendantButton)
        {
            inputContainer = GetComponent<InputContainer>();
        }
        else
        {
            buttons = InteractablesManager.Instance.GetAllInteractableButton(button);
        }

        SetUpButtons();
    }

    /// <summary>
    /// Sets up outlines for buttons to press
    /// </summary>
    public virtual void SetUpButtons()
    {
        objectToHighlightOutlineList.Clear();
        Enabled = false;
        for (int i = 0; i < buttons.Count; ++i)
        {
            var outline = buttons[i].gameObject.AddComponent<Outline>();
            objectToHighlightOutlineList.Add(outline);
            PhysicalButton physBut = buttons[i].GetComponent<PhysicalButton>();
            physBut.OnPressed.AddListener(() => { PressedButton(physBut.gameObject, outline); });
            physBut.OnReleased.AddListener(() => { ReleasedButton(physBut.gameObject); });
        }
    }

    /// <summary>
    /// If we released button set flag
    /// </summary>
    /// <param name="gameObject">Reference to gameobject in dictionary</param>
    private void ReleasedButton(GameObject gameObject)
    {
        buttonsFlags.Remove(gameObject.name);
    }

    /// <summary>
    /// If we pressed button set flags and remove outline
    /// </summary>
    /// <param name="button">Button for dictionary</param>
    /// <param name="outline">Outline object</param>
    private void PressedButton(GameObject button, QuickOutline.Outline outline)
    {
        if (!isPendantButton)
        {
            RemoveComponents(outline);
        }
        if (!buttonsFlags.ContainsKey(button.name))
            buttonsFlags.Add(button.name, true);
    }

    /// <summary>
    /// Sets flags if we pressed a button
    /// </summary>
    /// <param name="name">Code of button pressed</param>
    private void PressedButton(string name)
    {
        if (!buttonsFlags.ContainsKey(name))
            buttonsFlags.Add(name, true);
    }

    /// <summary>
    /// Removes specified outline
    /// </summary>
    /// <param name="outline">Outline to remove</param>
    private void RemoveComponents(Outline outline)
    {
        var highlight = objectToHighlightOutlineList.Find(x => x == outline);
        Destroy(highlight);
    }

    /// <summary>
    /// Cleans up all buttons outlines and listeners
    /// </summary>
    public virtual void CleanUpButtons()
    {
        objectToHighlightOutlineList.ForEach(x => Destroy(x));
        objectToHighlightOutlineList.Clear();

        for (int i = 0; i < buttons.Count; ++i)
        {
            PhysicalButton physBut = buttons[i].GetComponent<PhysicalButton>();
            physBut.OnPressed.RemoveListener(() => { PressedButton(physBut.gameObject, objectToHighlightOutlineList[i]); });
            physBut.OnReleased.RemoveListener(() => { ReleasedButton(physBut.gameObject); });
        }
    }

    /// <summary>
    /// Detects button presses
    /// </summary>
    protected override void onUpdate()
    {
        CheckButtonCondition();
    }

    private void CheckButtonCondition()
    {
        if (Enabled)
            return;
        if (multipleButtons)
        {
            if (isPendantButton)
            {
                if (buttonsFlags.Count == inputContainer.binds.Count)
                {
                    EnabledSwitch();
                }
                else
                {
                    Enabled = false;
                }
            }
            else
            {
                if (buttonsFlags.Count == buttons.Count)
                {
                    EnabledSwitch();
                }
                else
                {
                    Enabled = false;
                }
            }
        }
        else
        {
            if (buttonsFlags.Count >= 1)
            {
                EnabledSwitch();
            }
            else
            {
                Enabled = false;
            }
        }
    }

    /// <summary>
    /// Enables continuation and cleans up buttons
    /// </summary>
    private void EnabledSwitch()
    {
        Enabled = true;
        CleanUpButtons();
    }

    /// <summary>
    /// Detects press on pendant
    /// </summary>
    /// <param name="inputName">Pressed button code</param>
    public void PressButtonPendant(string inputName)
    {
        PressedButton(inputName);
    }

    protected override void cleanUp()
    {
        CleanUpButtons();
    }

}
