using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

/// <summary>
/// Manager for controlling and storing all controllers
/// </summary>
public class ControllersManager : Singleton<ControllersManager>
{
    /// <summary>
    /// List of all controllers
    /// </summary>
    [SerializeField]
    private List<Controller> controllers = default;

    /// <summary>
    /// Context menu method to get all controllers on scene
    /// </summary>
    [ContextMenu("Get all controllers")]
    public void GetAllControllers()
    {
        controllers.Clear();
        controllers = FindObjectsOfType<Controller>().ToList();
#if UNITY_EDITOR
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#endif
    }

    /// <summary>
    /// Gets controller from collection
    /// </summary>
    /// <typeparam name="T"> Type of returned controller </typeparam>
    /// <returns> Controller of specified type</returns>
    public T GetController<T>() where T : Controller
    {
        var findController = controllers.Find(x => x as T);
        return findController as T;
    }

    /// <summary>
    /// Gets all controllers of type from collection
    /// </summary>
    /// <typeparam name="T"> Type of returned controllers </typeparam>
    /// <returns> List of controllers of specified type</returns>
    public T[] GetControllers<T>() where T : Controller
    {
        var findControllers = controllers.FindAll(x => x as T);
        List<T> ret = new List<T>();
        for (int i = 0; i < findControllers.Count; ++i)
        {
            ret.Add(findControllers[i] as T);
        }
        return ret.ToArray();
    }
}
