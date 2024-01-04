using QuickOutline;
using UnityEngine;

/// <summary>
/// Allows switching of material and outline on objects
/// </summary>
public class MaterialSwitcher : MonoBehaviour
{
    /// <summary>
    /// Reference to outline 
    /// </summary>
    private Outline outline;

    /// <summary>
    /// Switches color of outline
    /// </summary>
    /// <param name="source">Transform of object containing outline</param>
    /// <param name="color">Color of outline to set</param>
    public void SwitchMaterial(Transform source, Color color)
    {
        outline = source.gameObject.GetComponent<QuickOutline.Outline>();
        if (outline == null)
        {
            outline = source.gameObject.AddComponent<QuickOutline.Outline>();
        }

        outline.GetComponent<MeshRenderer>().materials[0].color = color;
    }

    /// <summary>
    /// Switches material of object
    /// </summary>
    /// <param name="source">Transform of object to switch material</param>
    /// <param name="mat">Material to switch to</param>
    public void SwitchMaterial(Transform source, Material mat)
    {
        var mats = new Material[1];
        mats[0] = mat;
        source.GetComponent<MeshRenderer>().materials = mats;
    }

    public void SwitchMaterial(MeshRenderer target, Material mat)
    {
        var mats = new Material[1];
        mats[0] = mat;
        target.materials = mats;
    }

    public void SwitchColor(Transform source, Color color)
    {
        source.GetComponent<MeshRenderer>().materials[0].color = color;
    }
}
