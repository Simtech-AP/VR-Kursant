using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

/// <summary>
/// Universal loomer (asynchornous execution on threads)
/// </summary>
public class Loom : MonoBehaviour
{
    /// <summary>
    /// Maximum number of concurrent threads
    /// </summary>
    public static int maxThreads = 8;
    /// <summary>
    /// Current number of threads
    /// </summary>
    static int numThreads;
    /// <summary>
    /// Current instance of Loomer
    /// </summary>
    private static Loom _current;
    /// <summary>
    /// Public accessor for current instance of Loomer
    /// </summary>
    public static Loom Current
    {
        get
        {
            Initialize();
            return _current;
        }
    }

    /// <summary>
    /// Sets up references
    /// </summary>
    void Awake()
    {
        _current = this;
        initialized = true;
    }

    /// <summary>
    /// Is the loomer initialized?
    /// </summary>
    static bool initialized;

    /// <summary>
    /// Initializes Loomer
    /// </summary>
    static void Initialize()
    {
        if (!initialized)
        {

            if (!Application.isPlaying)
                return;
            initialized = true;
            var g = new GameObject("Loom");
            _current = g.AddComponent<Loom>();
        }

    }

    /// <summary>
    /// Actions to run in a thread
    /// </summary>
    private List<Action> _actions = new List<Action>();

    /// <summary>
    /// Structure for delayed activation of main thread action
    /// </summary>
    public struct DelayedQueueItem
    {
        public float time;
        public Action action;
    }
    /// <summary>
    /// List of delayd actions
    /// </summary>
    private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();
    /// <summary>
    /// List of currentl items being delayed
    /// </summary>
    List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

    /// <summary>
    /// Allows queueing items on main thread
    /// </summary>
    /// <param name="action">Action to perform on main thread</param>
    public static void QueueOnMainThread(Action action)
    {
        QueueOnMainThread(action, 0f);
    }

    /// <summary>
    /// Allows queueing items on main thread with a delay
    /// </summary>
    /// <param name="action">Action to perform on main thread</param>
    /// <param name="time">Delay of action</param>
    public static void QueueOnMainThread(Action action, float time)
    {
        if (time != 0)
        {
            lock (Current._delayed)
            {
                Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
            }
        }
        else
        {
            lock (Current._actions)
            {
                Current._actions.Add(action);
            }
        }
    }

    /// <summary>
    /// Runs an asynchronous thread
    /// </summary>
    /// <param name="a">Action to do while thread is running</param>
    /// <returns></returns>
    public static Thread RunAsync(Action a)
    {
        Initialize();
        Thread.CurrentThread.IsBackground = true;
        while (numThreads >= maxThreads)
        {
            Thread.Sleep(1);
        }
        Interlocked.Increment(ref numThreads);
        ThreadPool.QueueUserWorkItem(RunAction, a);
        return null;
    }

    /// <summary>
    /// Performs an action
    /// </summary>
    /// <param name="action">Action to perform</param>
    private static void RunAction(object action)
    {
        try
        {
            ((Action)action)();
        }
        catch
        {
        }
        finally
        {
            Interlocked.Decrement(ref numThreads);
        }

    }

    /// <summary>
    /// Cleans up instance
    /// </summary>
    void OnDisable()
    {
        if (_current == this)
        {
            _current = null;
        }
    }

    /// <summary>
    /// Template start method
    /// </summary>
    void Start()
    {

    }

    /// <summary>
    /// Current list of actions
    /// </summary>
    List<Action> _currentActions = new List<Action>();

    /// <summary>
    /// Performs queued actions
    /// </summary>
    void Update()
    {
        lock (_actions)
        {
            _currentActions.Clear();
            _currentActions.AddRange(_actions);
            _actions.Clear();
        }
        foreach (var a in _currentActions)
        {
            a();
        }
        lock (_delayed)
        {
            _currentDelayed.Clear();
            _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
            foreach (var item in _currentDelayed)
                _delayed.Remove(item);
        }
        foreach (var delayed in _currentDelayed)
        {
            delayed.action();
        }
    }
}
