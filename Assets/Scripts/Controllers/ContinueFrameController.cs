using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Types of canvas positions
/// </summary>
public enum CanvasType
{
    INSIDE_FRONT = 0,
    INSIDE_LEFT = 1,
    OUTSIDE_FRONT = 2,
}

/// <summary>
/// Controller for contnuiing step by step with confirmation 
/// </summary>
public class ContinueFrameController : Controller
{
    /// <summary>
    /// Reference to current module
    /// </summary>
    private Scenario scenario = default;
    /// <summary>
    /// Reference to fading indicator
    /// </summary>
    [SerializeField]
    private Image stepIndicator = default;

    /// <summary>
    /// Continously fades indicator in and out
    /// </summary>
    private void Start()
    {
        stepIndicator.DOFade(0, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// Disable steping into next step
    /// </summary>
    public void DisableNextStep()
    {
        StopAllCoroutines();
        stepIndicator.gameObject.SetActive(false);
    }

    /// <summary>
    /// Continues to the next step
    /// </summary>
    public void GoToNextStep()
    {
        scenario = StateModel.currentScenario;
        if (scenario)
            scenario.NextStep();
    }

    /// <summary>
    /// Enable continuing to next step
    /// </summary>
    public void EnableNextStep()
    {
        stepIndicator.gameObject.SetActive(true);
    }
}
