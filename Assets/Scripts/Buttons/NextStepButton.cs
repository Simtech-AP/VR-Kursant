using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class handling logic for next step button
/// </summary>
public class NextStepButton : PhysicalButton
{
    /// <summary>
    /// Main visual button for continuing
    /// </summary>
    [SerializeField]
    private GameObject button = default;

    /// <summary>
    /// Referenceto right hand glove/controller
    /// </summary>
    [SerializeField]
    private InteractGlove interactGlove = default;

    /// <summary>
    /// Reference to continue frame on scene
    /// </summary>
    private ContinueFrameController continueFrame = default;

    /// <summary>
    /// Sets up references
    /// </summary>
    private void Awake()
    {
        continueFrame = ControllersManager.Instance.GetController<ContinueFrameController>();
    }

    /// <summary>
    /// Press the button
    /// </summary>
    public void Press()
    {
        if (button.activeInHierarchy)
        {
            continueFrame.GoToNextStep();
            button.SetActive(false);
            interactGlove.ReleaseObject();
        }
    }

    /// <summary>
    /// Release held object from hand when this object is disabled  
    /// </summary>
    public void OnDisable()
    {
        interactGlove.ReleaseObject();
    }
}
