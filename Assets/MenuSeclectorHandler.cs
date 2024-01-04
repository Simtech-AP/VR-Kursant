using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSeclectorHandler : MonoBehaviour
{
    [SerializeField] private RectTransform selector;
    [SerializeField] private CanvasTextScoller canvasScroller;

    [SerializeField] private float offsetPerOption = 0;
    [SerializeField] private float initialOffset = 0;

    public void RepositionSelector(int targetMenuOptionIndex)
    {
        var trueTargetIndex = targetMenuOptionIndex;

        if (canvasScroller)
        {
            trueTargetIndex = canvasScroller.CurrentVievLineIndex;
        }

        var yOffset = initialOffset + trueTargetIndex * offsetPerOption;

        selector.anchoredPosition = new Vector2(selector.anchoredPosition.x, yOffset);

    }
}
