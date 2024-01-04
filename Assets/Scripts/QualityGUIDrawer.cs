using UnityEngine;

/// <summary>
/// Temporary drawer of quaity settings on screen
/// </summary>
public class QualityGUIDrawer : MonoBehaviour
{
    /// <summary>
    /// Should the drawer be shown?
    /// </summary>
    public bool DrawUI;
    /// <summary>
    /// Temporary antialiasing variable
    /// </summary>
    private int antiAliasing = 8;
    /// <summary>
    /// Current pixel light count
    /// </summary>
    private int pixelLightCount = 10;
    /// <summary>
    /// Is the toggle fo anisotropic filtering on?
    /// </summary>
    private bool toggle1 = true;
    /// <summary>
    /// Is the toggle for soft particles on?
    /// </summary>
    private bool toggle2 = true;
    /// <summary>
    /// Is the toggle for realtime reflection probes on?
    /// </summary>
    private bool toggle3 = true;

    /// <summary>
    /// Main drawing method
    /// </summary>
    private void OnGUI()
    {
        if (DrawUI)
        {
            int width = 250;
            int height = 180;
            int left = Screen.width - width - 10;
            int top = 130;

            GUI.Box(new Rect(left, top - 30, width, height), "Quality Settings");
            QualitySettings.antiAliasing = antiAliasing = (int)GUI.HorizontalSlider(new Rect(left, top, 200, 20), antiAliasing, 0, 8);
            QualitySettings.pixelLightCount = pixelLightCount = (int)GUI.HorizontalSlider(new Rect(left, top + 30, 200, 20), pixelLightCount, 0, 15);
            QualitySettings.anisotropicFiltering = (toggle1 = GUI.Toggle(new Rect(left, top + 60, 200, 20), toggle1, "Anisotropic Filtering"))? AnisotropicFiltering.Enable : AnisotropicFiltering.Disable;
            QualitySettings.softParticles = toggle2 = GUI.Toggle(new Rect(left, top + 90, 200, 20), toggle2, "Soft Particles");
            QualitySettings.realtimeReflectionProbes = toggle3 = GUI.Toggle(new Rect(left, top + 120, 200, 20), toggle3, "Reflection probes");
        }
    }
}
