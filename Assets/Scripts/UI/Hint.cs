using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Helper class for detecting hints
/// </summary>
public class Hint : MonoBehaviour
{
    [HideInInspector]
    public HintType hintType = default;

    /// <summary>
    /// Hint title
    /// </summary>
    [SerializeField]
    private TextMeshPro title = default;

    /// <summary>
    /// Additional hint text
    /// </summary>
    [SerializeField]
    private TextMeshPro text = default;

    /// <summary>
    /// Title underline
    /// </summary>
    [SerializeField]
    private TextMeshPro underline = default;

    /// <summary>
    /// Line between hint and it's target
    /// </summary>
    [SerializeField]
    private LineRenderer hintLine = default;

    public TextMeshPro Title { get { return title; } }
    public TextMeshPro Text { get { return text; } }
    public TextMeshPro Underline { get { return underline; } }
    public LineRenderer HintLine { get { return hintLine; } }


    /// <summary>
    /// Id of text data
    /// </summary>
    [SerializeField]
    private string id = "";

    public string Id
    {
        get
        {
            return hintType.ToString() + id;
        }
    }

    /// <summary>
    /// Sets text data to a visible text
    /// </summary>
    /// <param name="text">Text to set</param>
    public void SetTexts(string title, string text)
    {
        this.title.text = title;
        this.text.text = text;
    }
}
