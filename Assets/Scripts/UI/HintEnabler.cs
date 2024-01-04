using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

/// <summary>
/// Class in charge of enabing, showing and hiding hints and descriptions
/// </summary>
public class HintEnabler : MonoBehaviour
{
    [Serializable]
    private class HintAnimation
    {
        public Hint Hint = default;
        public Tween TitleAnimation = default;
        public Tween UnderlineAnimation = default;
        public Tween DescriptionAnimation = default;
    }

    /// <summary>
    /// Title texts for all hints
    /// </summary>
    [SerializeField]
    private List<HintAnimation> hints = default;

    [SerializeField]
    private Material lineMaterialTemplate = default;

    private Material lineMaterialCopy = default;

    [SerializeField]
    private float visibilityDistanceForDescription = default;

    [SerializeField]

    private float visibilityDistanceForMainHint = default;

    [SerializeField]

    private float fadeTime = default;

    [SerializeField]
    private float maxTitleAlpha = default;

    [SerializeField]
    private float maxDescriptionAlpha = default;

    [SerializeField]
    private float maxLineAlpha = default;

    private Transform playerCamera;

    private Tween lineAnimation = default;


    /// <summary>
    /// Get reference to current player camera
    /// Set all colors of titles, descriptions and lines to clear
    /// </summary>
    private void Start()
    {
        playerCamera = Camera.main.transform;
        lineMaterialCopy = new Material(lineMaterialTemplate);

        foreach (var hint in hints)
        {
            hint.Hint.Title.color = Color.clear;
            hint.Hint.Text.color = Color.clear;
            hint.Hint.Underline.color = Color.clear;
            hint.Hint.HintLine.material = lineMaterialCopy;
        }

        lineMaterialCopy.color = Color.clear;
    }

    /// <summary>
    /// Checks continously if we are in defined distance near hints
    /// </summary>
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, playerCamera.position);

        Color mainColor = default;
        Color descriptionColor = default;
        Color lineColor = default;

        if (distance < visibilityDistanceForDescription)
        {
            mainColor = new Color(1, 1, 1, maxTitleAlpha);
            descriptionColor = new Color(1, 1, 1, maxDescriptionAlpha);
            lineColor = new Color(1, 1, 1, maxLineAlpha);
        }
        else if (distance > visibilityDistanceForDescription && distance < visibilityDistanceForMainHint)
        {
            mainColor = new Color(1, 1, 1, maxTitleAlpha);
            descriptionColor = Color.clear;
            lineColor = new Color(1, 1, 1, maxLineAlpha);

        }
        else
        {
            mainColor = Color.clear;
            descriptionColor = Color.clear;
            lineColor = Color.clear;

        }

        foreach (var hint in hints)
        {
            if (hint.Hint.Title.color != mainColor && hint.TitleAnimation == null)
            {
                hint.TitleAnimation = hint.Hint.Title.DOColor(mainColor, fadeTime).OnKill(() => hint.TitleAnimation = null);
            }

            if (hint.Hint.Underline.color != mainColor && hint.UnderlineAnimation == null)
            {
                hint.UnderlineAnimation = hint.Hint.Underline.DOColor(mainColor, fadeTime).OnKill(() => hint.UnderlineAnimation = null);
            }

            if (hint.Hint.Text.color != descriptionColor && hint.DescriptionAnimation == null)
            {
                hint.DescriptionAnimation = hint.Hint.Text.DOColor(descriptionColor, fadeTime).OnKill(() => hint.DescriptionAnimation = null);
            }
        }

        if (lineMaterialCopy.color != lineColor && lineAnimation == null)
        {
            lineAnimation = lineMaterialCopy.DOColor(lineColor, fadeTime).OnKill(() => lineAnimation = null);
        }
    }
}
