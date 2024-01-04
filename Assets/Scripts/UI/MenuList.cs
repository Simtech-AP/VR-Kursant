using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System;

/// <summary>
/// Menu class for pendant UI
/// </summary>
public class MenuList : MonoBehaviour
{
    /// <summary>
    /// Currently selected option index
    /// </summary>
    private int optionIndex;

    [SerializeField] private bool rememberSelectedOption = false;

    public int OptionIndex
    {
        get { return optionIndex; }
        set
        {
            optionIndex = value;
            OnOptionChanged.Invoke(optionIndex);
        }
    }
    /// <summary>
    /// Full text of menu list
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI text = default;

    /// <summary>
    /// Event list with events connected to every option 
    /// </summary>
    [SerializeField]
    private List<UnityEvent> eventList = new List<UnityEvent>();

    /// <summary>
    /// Default text shown on options
    /// </summary>
    private string defaultText = "";

    [SerializeField] private List<IndexedTMPContainer> colorBoundTextContainers = new List<IndexedTMPContainer>();

    /// <summary>
    /// Actions run when menu is opened
    /// </summary>
    public static Action<string> MenuOpened;
    public static Action<string, List<UnityEvent>> MenuInteracted;


    public UnityEventInt OnOptionChanged;

    public List<UnityEvent> EventList { get => eventList; set => eventList = value; }

    /// <summary>
    /// Set start option at the top
    /// </summary>
    private void Start()
    {
        ChangeOption(false);
    }

    /// <summary>
    /// Changes option 
    /// </summary>
    /// <param name="down">Do we change option up?</param>
    public void ChangeOption(bool down)
    {
        if (down && OptionIndex < text.textInfo.lineCount - 1)
        {
            OptionIndex++;
        }
        else if (!down && OptionIndex > 0)
        {
            OptionIndex--;
        }
        SelectOption();
    }

    /// <summary>
    /// Marks text for selecting an instruction
    /// </summary>
    public void SelectOption()
    {
        defaultText = text.text;
        defaultText = defaultText.Replace("<color=#fff>", "");
        defaultText = defaultText.Replace("</color>", "");
        text.text = defaultText;
        int firstLine = 0;
        int nextLine = 0;
        if (text.text.Length > firstLine + 3)
            nextLine = text.text.IndexOf("\n", firstLine + 3);
        for (int i = 0; i < OptionIndex; i++)
        {
            firstLine = text.text.IndexOf("\n", firstLine + 3);
            nextLine = text.text.IndexOf("\n", firstLine + 3);
        }
        text.text = text.text.Insert(nextLine, "</color>");
        text.text = text.text.Insert(firstLine, "<color=#fff>");

        SetColorsForColorBoundTexts();
    }

    private void SetColorsForColorBoundTexts()
    {

        foreach (var i in colorBoundTextContainers)
        {
            if (i.index == OptionIndex)
            {
                foreach (var TMPitem in i.Container)
                {
                    TMPitem.color = new Color(0.86f, 0.89f, 0.86f, 1f);
                }
            }
            else
            {
                foreach (var TMPitem in i.Container)
                {
                    TMPitem.color = new Color(0f, 0.19f, 0.95f, 1f);
                }
            }
        }
    }

    /// <summary>
    /// Invokes event when list is opened
    /// </summary>
    private void OnEnable()
    {
        OnOptionChanged.Invoke(OptionIndex);
        MenuOpened?.Invoke(this.name);
    }

    private void OnDisable()
    {
        if (!rememberSelectedOption)
        {
            OptionIndex = 0;
            SelectOption();
        }
    }

    /// <summary>
    /// Invokes event when option is used
    /// </summary>
    public void UseOption()
    {
        if (OptionIndex < eventList.Count)
        {
            MenuInteracted?.Invoke(this.name, eventList);
            eventList[OptionIndex].Invoke();
        }
    }
}
