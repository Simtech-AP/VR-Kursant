using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains definitions for light switching
/// </summary>
public class LightType
{
    /// <summary>
    /// Values of lights
    /// </summary>
    public static Dictionary<LightColor, Color> LightValue = new Dictionary<LightColor, Color>()
    {
        {LightColor.GREEN, Color.green },
        {LightColor.RED, Color.red },
        {LightColor.YELLOW, Color.yellow }
    };
}
