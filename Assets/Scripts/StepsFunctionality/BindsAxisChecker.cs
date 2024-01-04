using System;
using UnityEngine;

/// <summary>
/// Allows checking for movement of speciifed axis using buttons
/// </summary>
public class BindsAxisChecker : MonoBehaviour
{
    /// <summary>
    /// Reference to pressed buttons object
    /// </summary>
    [SerializeField]
    private ButtonsPressedTarget pressedButtonTarget = default;
    /// <summary>
    /// Reference to axis visualizer
    /// </summary>
    [SerializeField]
    private RobotAxisVisualizer axisVisualizer = default;

    /// <summary>
    /// Toggles axis helepr when moved
    /// </summary>
    /// <param name="axisLabel">Label for key presses</param>
    public void ToggleAxis(string axisLabel)
    {
        string minusAxis = "-" + axisLabel;
        string plusAxis = "+" + axisLabel;

        if (pressedButtonTarget.KeyPresent(minusAxis) && pressedButtonTarget.KeyPresent(plusAxis))
        {
            bool axisMoved = pressedButtonTarget.GetKeyState(minusAxis) && pressedButtonTarget.GetKeyState(plusAxis);
            if (axisMoved)
            {
                axisVisualizer.HideAxis(new Tuple<string, string>(plusAxis, minusAxis));
            }
        }
    }
}
