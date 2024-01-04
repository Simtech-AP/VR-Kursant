using System.Collections;

/// <summary>
/// Interface with lighting functionality
/// </summary>
public interface IBlinkable
{
    /// <summary>
    /// Blink light object
    /// </summary>
    /// <param name="color">Color to change light to</param>
    /// <returns>Yield object</returns>
    IEnumerator StartBlinking(object color);
    /// <summary>
    /// Enables light with specified color index
    /// </summary>
    /// <param name="color">Index of color to set</param>
    void EnableLight(int color);
    /// <summary>
    /// Disables light with specified color index
    /// </summary>
    /// <param name="color">Index of color to disable</param>
    void DisableLight(int color);
    /// <summary>
    /// Starts blinking light with specified color index
    /// </summary>
    /// <param name="color"></param>
    void BlinkLight(int color);

    /// <summary>
    /// Are we curently blinking lingh on this object?
    /// </summary>
    bool Blinking { get; set; }
}
