using System.Linq;

/// <summary>
/// Allows checking for EStop errors
/// </summary>
public class EStopErrorChecker : ErrorChecker
{
    /// <summary>
    /// Checks for EStop errors
    /// </summary>
    /// <returns>Is any EStop pressed?</returns>
    public override bool CheckState()
    {
        if (CellStateData.EStopStates.All(x => x == EStopButtonState.Released))
        {
            return true;
        }
        return false;
    }
}
