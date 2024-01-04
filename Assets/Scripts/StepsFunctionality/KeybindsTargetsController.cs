using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows chcking of keybinds on pendant
/// </summary>
public class KeybindsTargetsController : StepEnabler
{
    /// <summary>
    /// Container holding all of the targets to gaze upon
    /// </summary>
    [SerializeField]
    public GameObject targetsParent = default;
    /// <summary>
    /// All targets to gaze upon
    /// </summary>
    [SerializeField]
    private List<GameObject> targets = new List<GameObject>();
    /// <summary>
    /// Pedant Controller object
    /// </summary>
    private PendantController pendantController;
    /// <summary>
    /// Input Controller object
    /// </summary>
    private InputController inputController;
    /// <summary>
    /// List of targets to gaze at
    /// </summary>
    public List<GameObject> Targets { get => targets; }


    /// <summary>
    /// Initialize base controllers for all steps
    /// </summary>
    protected override void initialize()
    {
        pendantController = ControllersManager.Instance.GetController<PendantController>();
        inputController = ControllersManager.Instance.GetController<InputController>();

        ParentTargets();
    }

    /// <summary>
    /// Lock targets to parent on main scene
    /// </summary>
    public void ParentTargets()
    {
        targetsParent.transform.parent = pendantController.gameObject.transform;
        targetsParent.transform.localPosition = Vector3.zero;
        targetsParent.transform.localEulerAngles = Vector3.zero;
        targetsParent.SetActive(true);
    }

    /// <summary>
    /// Check if all the targets were gazed upon
    /// </summary>
    public void CheckTargets()
    {
        foreach (GameObject go in targets)
        {
            if (go.activeInHierarchy) return;
        }
        Enabled = true;
    }

    protected override void cleanUp()
    {
        Destroy(targetsParent);
    }
}
