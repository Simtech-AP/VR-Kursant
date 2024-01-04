using System.Collections;
using UnityEngine;

/// <summary>
/// Contains logic for blinking lights when applied
/// </summary>
public class Blinker : MonoBehaviour, IBlinkable
{
    /// <summary>
    /// Time for light to be active before switching off
    /// </summary>
    public float ActiveTime;
    /// <summary>
    /// Time for light to be disabled before switching on
    /// </summary>
    public float DisableTime;
    /// <summary>
    /// Gameobject containing light
    /// </summary>
    public GameObject Source;
    /// <summary>
    /// Are we currently blinking the light?
    /// </summary>
    public bool Blinking { get; set; }

    /// <summary>
    /// Starts blinking the light
    /// </summary>
    /// <param name="color">Index of light color</param>
    public void BlinkLight(int color)
    {
        
    }

    /// <summary>
    /// Disables the light
    /// </summary>
    /// <param name="color">Index of light color</param>
    public void DisableLight(int color)
    {
        
    }

    /// <summary>
    /// Enables the light
    /// </summary>
    /// <param name="color">Index of light color</param>
    public void EnableLight(int color)
    {
        
    }

    /// <summary>
    /// Starts blinking the light after delay
    /// </summary>
    /// <param name="color">Color object</param>
    /// <returns>Handle to coroutine</returns>
    public IEnumerator StartBlinking(object color)
    {
        while (true)
        {
            yield return new WaitForSeconds(ActiveTime);
            Source.SetActive(false);
            yield return new WaitForSeconds(DisableTime);
            Source.SetActive(true);
        }
    }

    /// <summary>
    /// Sets up defaults and starts blinking
    /// </summary>
    private void Awake()
    {
        Blinking = true;
        StartCoroutine(StartBlinking(null));
    }
}
