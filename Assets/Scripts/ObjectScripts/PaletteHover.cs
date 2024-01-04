using UnityEngine;

/// <summary>
/// Helper class for hovering obvver a palette
/// </summary>
public class PaletteHover : Hover
{
    /// <summary>
    /// After specified time light up tint
    /// </summary>
    /// <param name="other">Collider thet entered the trigger</param>
    protected override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
        if (other.GetComponentInParent<RobotInteractiveObject>())
        {
            timer += Time.fixedDeltaTime;
        }
        if (timer >= timeMax)
        {
            onHoveredForTime.Invoke();
            meshRenderer.materials[0].SetColor("_TintColor", Color.green);
        }
    }

    /// <summary>
    /// Clear up the tint
    /// </summary>
    /// <param name="other">Collider thet exited the trigger</param>
    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        if (timer >= timeMax)
        {
            timer = 0f;
        }
        else
        {
            if (other.GetComponentInParent<RobotInteractiveObject>())
            {
                onExit.Invoke();
                meshRenderer.materials[0].SetColor("_TintColor", Color.red);
                timer = 0f;
            }
        }
    }
}
