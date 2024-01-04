using UnityEngine;

/// <summary>
/// Template class for examination task
/// </summary>
public class ExamTask : MonoBehaviour
{
    /// <summary>
    /// Reference to testing module
    /// </summary>
    [SerializeField]
    protected TestModule testModule;

    /// <summary>
    /// Ends current task
    /// </summary>
    public virtual void EndTask() { }
}
