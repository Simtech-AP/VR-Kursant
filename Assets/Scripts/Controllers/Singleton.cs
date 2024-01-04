using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic class for Singleton pattern implementation
/// </summary>
/// <typeparam name="T"> Type of singleton class </typeparam>
public class Singleton<T> : MonoBehaviour
    where T : class
{
    /// <summary>
    /// Instance of singleton
    /// </summary>
    private static T instance;
    /// <summary>
    /// Property for getting Instance
    /// </summary>
    public static T Instance { get { return instance; } }

    /// <summary>
    /// Initalize singleton class
    /// </summary>
    protected virtual void Awake()
    {
        if (instance != null && instance != this as T)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this as T;
        }

        if (transform.parent == null)
            DontDestroyOnLoad(this);
    }
}