using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing logic for every bing used in current time
/// </summary>
public class InputContainer : MonoBehaviour
{
    /// <summary>
    /// List of all binds in current input container
    /// </summary>
    public List<InputBind> binds = new List<InputBind>();
    /// <summary>
    /// Reference to input controller
    /// </summary>
    private InputController inputController = default;

    /// <summary>
    /// Gets reference to input controller from scene
    /// </summary>
    private void Awake()
    {
        inputController = ControllersManager.Instance.GetController<InputController>();
    }

    /// <summary>
    /// If this InputContainer component has been enabled add it to InputController list of active InputContainers
    /// </summary>
    private void OnEnable()
    {
        inputController.AddBinds(this);
    }

    /// <summary>
    /// If this InputContainer component has been disabled remove it from InputController list of active InputContainers
    /// </summary>
    private void OnDisable()
    {
        inputController.RemoveBinds(this);
    }
}
