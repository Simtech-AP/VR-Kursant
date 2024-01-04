using System;
using UnityEngine;

/// <summary>
/// Base abstract class for all controllers
/// </summary>
public abstract class Controller : MonoBehaviour
{
    /// <summary>
    /// Initalizes controller
    /// </summary>
    protected virtual void Awake()
    {
        InitalizeControllers();
    }

    /// <summary>
    /// Initalize only required components for independent controller
    /// </summary>
    protected virtual void InitalizeControllers() { }
}
