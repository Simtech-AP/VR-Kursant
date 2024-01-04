using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasTextScoller : MonoBehaviour
{
    [SerializeField] private int maxScrenLength = 7;
    [SerializeField] private float offsetPerOption = 0f;
    [SerializeField] private float initialOffset = 0;

    [SerializeField] private RectTransform scrollableRect;

    private int currentTopmostVisibleOptionIndex = 0;

    public int CurrentTopmostVisibleOptionIndex
    {
        get => currentTopmostVisibleOptionIndex;
    }
    private int currentVievLineIndex = 0;
    public int CurrentVievLineIndex { get { return currentVievLineIndex; } set { currentVievLineIndex = value; } }

    public void SetOffset(int indexOption)
    {
        currentTopmostVisibleOptionIndex = indexOption;
        CurrentVievLineIndex = indexOption;
        SetOffset();

    }

    public void AdjustScrollOffsetToOption(int currentOptionIndex)
    {
        if (currentOptionIndex < currentTopmostVisibleOptionIndex)
        {
            currentTopmostVisibleOptionIndex--;
        }
        else if (currentOptionIndex >= currentTopmostVisibleOptionIndex + maxScrenLength)
        {
            currentTopmostVisibleOptionIndex++;
        }

        CurrentVievLineIndex = currentOptionIndex - currentTopmostVisibleOptionIndex;

        SetOffset();
    }

    private void SetOffset()
    {
        var yOffset = currentTopmostVisibleOptionIndex * offsetPerOption + initialOffset;
        scrollableRect.anchoredPosition = new Vector3(scrollableRect.localPosition.x,
                                                   yOffset,
                                                   scrollableRect.localPosition.z);
    }

    public void ResetOffset()
    {
        currentTopmostVisibleOptionIndex = 0;
        CurrentVievLineIndex = 0;
        SetOffset();
    }

    private void OnDisable()
    {
        ResetOffset();
    }

}
