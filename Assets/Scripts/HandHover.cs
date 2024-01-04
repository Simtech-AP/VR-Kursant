using UnityEngine;

/// <summary>
/// Class used to check if hand was hovered in object
/// </summary>
public class HandHover : Hover
{
    /// <summary>
    /// What happens when we hover over trigger
    /// </summary>
    /// <param name="other">Other collider</param>
    protected override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
        TriggerStay<InteractGlove>(other);
    }

    /// <summary>
    /// What happens when we stop hovering over collider
    /// </summary>
    /// <param name="other">Other collider</param>
    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        TriggerExit<InteractGlove>(other);
    }
}
