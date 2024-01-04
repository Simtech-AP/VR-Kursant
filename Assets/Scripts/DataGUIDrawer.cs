using UnityEngine;

/// <summary>
/// Helper utility for visualizing robot status
/// </summary>
public class DataGUIDrawer : MonoBehaviour
{
    /// <summary>
    /// Are we drawing the gui?
    /// </summary>
    public bool DrawUI;

    /// <summary>
    /// Draw helper gui
    /// </summary>
    private void OnGUI()
    {
        if (DrawUI)
        {
            int width = 200;
            int height = 100;
            int left = Screen.width - width - 10;
            int top = 0;

            string text =
                "Movement Mode: " + RobotData.Instance.MovementMode.ToString() + " \n" +
                "Movement Type: " + RobotData.Instance.MovementType.ToString() + " \n" +
                "Robot Speed: " + RobotData.Instance.RobotSpeed.ToString() + " \n" +
                "IsRunning: " + RobotData.Instance.IsRunning.ToString() + " \n";

            GUI.Box(new Rect(left, top, width, height), "Robot Data");
            GUI.Label(new Rect(left + 10, top + 30, width - 20, height - 20), text);
        }
    }
}
