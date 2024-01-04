using UnityEngine;

/// <summary>
/// Checks if we have stopped person trying to enter cell
/// </summary>
public class PersonEntranceChecker : StepEnabler
{
    [Header("Person Entrance Checker")]
    /// <summary>
    /// Reference to person on scene
    /// </summary>
    [SerializeField]
    private GameObject character = default;
    /// <summary>
    /// Reference to hover of a hand
    /// </summary>
    [SerializeField]
    private GameObject handHover = default;
    /// <summary>
    /// Timer for enabling person walking animation
    /// </summary>
    private float elapsedTime = 0f;
    /// <summary>
    /// Have we hovered hand in specified place?
    /// </summary>
    private bool handHovered = false;

    [SerializeField] private float delayTime;

    /// <summary>
    /// Sets up listeners and enables hover and person
    /// </summary>

    protected override void initialize()
    {
        SetUpPersonEntrance();
    }

    public void SetUpPersonEntrance()
    {
        character.SetActive(true);
        handHover.SetActive(true);
        handHover.GetComponent<HandHover>().onHoveredForTime.AddListener(() => handHovered = true);
        handHover.GetComponent<HandHover>().onExit.AddListener(() => handHovered = false);
    }

    /// <summary>
    /// Finishes step
    /// </summary>
    public void EndStep()
    {
        Enabled = true;
    }

    /// <summary>
    /// Cleans up listeners and objects
    /// </summary>
    public void CleanUpPersonEntrance()
    {
        handHover.GetComponent<HandHover>().onHoveredForTime.RemoveListener(() => handHovered = true);
        handHover.GetComponent<HandHover>().onExit.RemoveListener(() => handHovered = false);
        character.SetActive(false);
        handHover.SetActive(false);
    }

    /// <summary>
    /// Checks if we hovered hand in specified place
    /// </summary>
    protected override void onUpdate()
    {
        PerformConditionTick();
    }

    private void PerformConditionTick()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > delayTime - 1 && elapsedTime < delayTime)
        {
            elapsedTime = delayTime;
            character.GetComponent<Animator>().SetTrigger("SwitchToWalk");
        }

        if (elapsedTime > delayTime && handHovered)
        {
            EndStep();
        }
    }

    protected override void cleanUp()
    {
        CleanUpPersonEntrance();
    }
}
