/// <summary>
/// Helper class to check if there is alarm error
/// </summary>
public class AlarmErrorChecker : ErrorChecker
{
    /// <summary>
    /// Check current state of error
    /// </summary>
    /// <returns></returns>
    public override bool CheckState()
    {
        if (ErrorRequester.HasAllErrorsReset())
        {
            return true;
        }

        return false;
    }
}
