using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgramDebugMarkerHandler : MonoBehaviour
{
    [SerializeField] private CanvasTextScoller scroller;

    [SerializeField] private Image markerImage;
    [SerializeField] private RectTransform marker;

    [SerializeField] private Sprite runningInstructionMarker;
    [SerializeField] private Sprite stoppedInstructionMarker;

    [SerializeField] private float initialOffset = -3;
    [SerializeField] private float offsetPerInstruction = -6.2f;

    private int currentRunningInstructionIndex = 0;

    public void UpdateMarkerPosition(int curretRunningLineIndex)
    {
        currentRunningInstructionIndex = curretRunningLineIndex;

        var trueIndex = scroller == null ? curretRunningLineIndex : curretRunningLineIndex - scroller.CurrentTopmostVisibleOptionIndex;

        var yOffsset = initialOffset + trueIndex * offsetPerInstruction;
        marker.anchoredPosition = new Vector3(marker.anchoredPosition.x, yOffsset);
    }

    public void UpdateRunningStatus(bool running)
    {
        markerImage.sprite = running ? runningInstructionMarker : stoppedInstructionMarker;
    }

    public void OnPendantUIUpdate()
    {
        UpdateMarkerPosition(currentRunningInstructionIndex);
    }

    public void OnRobotDataUpdate(RobotData data)
    {
        currentRunningInstructionIndex = data.CurrentRunningInstructionIndex;
        UpdateMarkerPosition(currentRunningInstructionIndex);
    }

}
