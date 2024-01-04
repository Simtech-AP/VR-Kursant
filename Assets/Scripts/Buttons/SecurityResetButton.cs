using System.Collections;
using UnityEngine;

/// <summary>
/// Security reset button behaviour
/// </summary>
public class SecurityResetButton : PhysicalButton, IBlinkable, IResetable
{
    /// <summary>
    /// Light
    /// </summary>
    [SerializeField]
    private GameObject errorLight = default;
    /// <summary>
    /// Are we currently blinking button light?
    /// </summary>
    public bool Blinking { get; set; }

    /// <summary>
    /// Start blinking light
    /// </summary>
    [EnumAction(typeof(LightColor))]
    public void BlinkLight(int color)
    {
        if (!Blinking && !ErrorRequester.HasAnyErrors())
        {
            EndBlinking();
            Blinking = true;
            StartCoroutine(StartBlinking(color));
        }
    }

    /// <summary>
    /// Enable light
    /// </summary>
    [EnumAction(typeof(LightColor))]
    public void EnableLight(int color)
    {
        EndBlinking();
        Blinking = false;
        errorLight.SetActive(true);
    }

    /// <summary>
    /// Disable light
    /// </summary>
    [EnumAction(typeof(LightColor))]
    public void DisableLight(int color)
    {
        if (!ErrorRequester.HasAnyErrors())
        {
            EndBlinking();
            Blinking = false;
            errorLight.SetActive(false);
        }
    }

    /// <summary>
    /// Ends blinking light
    /// </summary>
    public void EndBlinking()
    {
        StopAllCoroutines();
        Blinking = false;
        errorLight.SetActive(false);
    }

    /// <summary>
    /// Blink light object
    /// </summary>
    /// <param name="obj">Color to change light to</param>
    /// <returns>Yield object</returns>
    public IEnumerator StartBlinking(object obj)
    {
        while (true)
        {
            errorLight.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            errorLight.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Security button press callback on scene
    /// </summary>
    [ContextMenu("PRESS")]
    public void SecurityButtonPress()
    {

        OnPressed.Invoke();
        DisableLight((int)LightColor.ALL);
    }

    /// <summary>
    /// Resets this object state to default
    /// </summary>
    void IResetable.Reset()
    {
        OnPressed.Invoke();
    }
}
