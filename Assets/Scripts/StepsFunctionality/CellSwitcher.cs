using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Switches cell state
/// </summary>
public class CellSwitcher : MonoBehaviour
{
    /// <summary>
    /// Selected button type
    /// </summary>
    [SerializeField]
    protected ButtonType button = default;
    /// <summary>
    /// List of buttons on scene of specified type
    /// </summary>
    private List<GameObject> buttons = default;
    /// <summary>
    /// Type of canvas position on scene
    /// </summary>
    [SerializeField]
    private CanvasType canvasSwitch = default;
    /// <summary>
    /// Reference to switch position frame
    /// </summary>
    private ContinueFrameSwitcher continueFrameSwitcher;

    /// <summary>
    /// Sets up references
    /// </summary>
    private void Awake()
    {
        continueFrameSwitcher = FindObjectOfType<ContinueFrameSwitcher>();
    }

    /// <summary>
    /// Gets buttons
    /// Adds listeners
    /// </summary>
    private void OnEnable()
    {
        buttons = InteractablesManager.Instance.GetAllInteractableButton(button);

        for(int i = 0; i< buttons.Count; ++i)
        {
            buttons[i].GetComponent<PhysicalButton>().OnPressed.AddListener(SwtichCellState);
        }
    }

    /// <summary>
    /// Switches canvas position to specified one
    /// </summary>
    private void SwtichCellState()
    {
        continueFrameSwitcher.SetCanvasPosition((int)canvasSwitch);
    }

    /// <summary>
    /// Removes listeners from buttons
    /// </summary>
    private void OnDisable()
    {
        for (int i = 0; i < buttons.Count; ++i)
        {
            buttons[i].GetComponent<PhysicalButton>().OnPressed.RemoveListener(SwtichCellState);
        }
    }


}
