using UnityEngine;

/// <summary>
/// Contains logic for gazable target of reset button
/// </summary>
public class ResetButtonGazableTarget : CustomizedGazableTarget
{
    [Header("Reset Button Gazable Target")]
    public bool forSingleResetButton = false;
    public int customButtonIndex = 0;
    /// <summary>
    /// Sets up references
    /// </summary>
    protected override void initialize()
    {
        var buttons = InteractablesManager.Instance.GetAllInteractableBehaviour<AlarmResetButton>();

        if (!forSingleResetButton)
        {
            for (int i = 0; i < buttons.Length; ++i)
            {
                objectsToGaze.Add(buttons[i].transform.Find("Visual").gameObject);
            }
            SetUpGazables();
            SetCustomColliders();
        }
        else
        {
            objectToGaze = buttons[customButtonIndex].transform.Find("Visual").gameObject;
            SetUpGazable();
            SetCustomColliders();
        }
    }
}
