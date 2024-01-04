/// <summary>
/// Controls state of hover over objects to continue steps
/// </summary>
public class HoverController : StepEnabler
{
    /// <summary>
    /// Completes hovering logic
    /// </summary>
    public void HoverCompleted()
    {
        Enabled = true;
    }

    /// <summary>
    /// If hover ended disable continuing
    /// </summary>
    public void HoverEnded()
    {
        Enabled = false;
    }
}
