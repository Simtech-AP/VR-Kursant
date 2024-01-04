using UnityEngine;

/// <summary>
/// Contains logic for functional buttons on scene
/// </summary>
public class FunctionalButtonTarget : PressedButtonTarget
{
    /// <summary>
    /// Reference to error checking object on scene
    /// </summary>
    [SerializeField]
    private ErrorChecker errorChecker = default;

    /// <summary>
    /// Sets up checking
    /// </summary>
    public override void SetUpButtons()
    {
        base.SetUpButtons();
        if (errorChecker && errorChecker.CheckState())
        {
            Enabled = true;
        }
    }
}
