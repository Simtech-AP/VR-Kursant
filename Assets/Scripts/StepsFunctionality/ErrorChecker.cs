using UnityEngine;

/// <summary>
/// Template class for checking errors
/// </summary>
public abstract class ErrorChecker : MonoBehaviour
{
    /// <summary>
    /// Checks current state of error
    /// </summary>
    /// <returns>Is this error currently on?</returns>
    public abstract bool CheckState();
}
