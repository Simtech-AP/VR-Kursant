/// <summary>
/// Allows checking if there are security errors
/// </summary>
public class SecurityErrorChecker : ErrorChecker
{
    /// <summary>
    /// Checks if we have any securty errors
    /// </summary>
    /// <returns>Is any error active?</returns>
    public override bool CheckState()
    {
        if (ErrorRequester.HasResetSecurityErrors())
        {
            return true;
        }

        return false;
    }
}
