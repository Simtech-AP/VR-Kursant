/// <summary>
/// Allows setup of gazable target for security reset button
/// </summary>
public class SecurityResetButtonGazableTarget : GazableTarget
{
    /// <summary>
    /// Setup references
    /// </summary>
    private void OnEnable()
    {
        var buttons = InteractablesManager.Instance.GetAllInteractableBehaviour<SecurityResetButton>();
        for (int i = 0; i < buttons.Length; ++i)
        {
            objectsToGaze.Add(buttons[i].gameObject);
        }
    }

    /// <summary>
    /// Setup gazable targets
    /// </summary>
    public void EnableGazableTarget()
    {
        SetUpGazables();
    }
}
