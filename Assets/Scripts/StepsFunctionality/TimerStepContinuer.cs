using UnityEngine;

/// <summary>
/// Allows enabling next step after specifed time delay
/// </summary>
public class TimerStepContinuer : MonoBehaviour
{
    /// <summary>
    /// Reference to currently running scenario
    /// </summary>
    private Scenario currentScenario;
    /// <summary>
    /// Temporary timer variable
    /// </summary>
    private float timer = 0f;
    /// <summary>
    /// Delay to reach
    /// </summary>
    public float ReachedTime = 5f;
    /// <summary>
    /// Index of step to change to
    /// </summary>
    public int NextStepIndex;

    /// <summary>
    /// Sets up references
    /// </summary>
    private void Awake()
    {
        currentScenario = GetComponentInParent<Scenario>();
    }

    /// <summary>
    /// Resets timer
    /// </summary>
    private void OnEnable()
    {
        timer = 0f;
    }

    /// <summary>
    /// Checks if we reached time to change step
    /// </summary>
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > ReachedTime)
        {
            currentScenario.RunStep(NextStepIndex);
            timer = 0;
        }
    }
}
