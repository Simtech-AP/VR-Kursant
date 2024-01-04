using UnityEngine;
using UnityEngine.Events;
using System;

/// <summary>
/// Contains logic for enabling or disabling continuation to next step
/// </summary>
public abstract class StepEnabler : MonoBehaviour
{

    /// <summary>
    /// Is it possible to go to next step?
    /// </summary>
    [HideInInspector] private bool isEnabled = false;

    public bool Enabled
    {
        get { return isEnabled; }
        set
        {
            if (StatePersistent) { return; }
            isEnabled = value;
            if (isEnabled)
            {
                OnEnabledStateReached.Invoke();
            }
            else
            {
                OnEnabledStateLost.Invoke();
            }
        }
    }

    [Header("Step Enabler")]
    public UnityEvent OnInitialize = default;
    public UnityEvent OnCleanup = default;
    public UnityEvent OnEnabledStateReached = default;
    public UnityEvent OnEnabledStateLost = default;

    /// <summary>
    /// Should Enabled be disallowed to change?
    /// </summary>
    [SerializeField] private bool statePersistent = false;
    public bool StatePersistent { get => statePersistent; set => statePersistent = value; }

    /// <summary>
    /// Should the OnUpdate be called each frame?
    /// </summary>
    [SerializeField] private bool checkCountinously = false;
    public bool CheckCountinously { get => checkCountinously; set => checkCountinously = value; }

    /// <summary>
    /// Override this in derived script to check your custom condition every frame.
    /// </summary>
    protected virtual void onUpdate() { }

    private void Update()
    {
        if (CheckCountinously) { onUpdate(); }
    }

    public void Initialize()
    {
        initialize();
        OnInitialize.Invoke();
    }

    protected virtual void initialize() { }

    public void CleanUp()
    {
        cleanUp();
        OnCleanup.Invoke();
    }

    protected virtual void cleanUp() { }

    public void RawForceEnable(bool enablePersistance)
    {
        isEnabled = true;
        StatePersistent = enablePersistance;
    }

}
