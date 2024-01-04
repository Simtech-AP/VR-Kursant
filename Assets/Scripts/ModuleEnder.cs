using UnityEngine;

/// <summary>
/// Helper class for finishing module
/// </summary>
public class ModuleEnder : MonoBehaviour
{
    /// <summary>
    /// Ends current module
    /// </summary>
    public void EndModule()
    {
        FindObjectOfType<ModuleController>().NextModule();
    }
}
