using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayModeRedirect : MonoBehaviour
{

    private int sceneToLoadIndex = default;

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Main")
        {
            redirectToMainScene();
        }
    }

    private void redirectToMainScene()
    {
        sceneToLoadIndex = int.Parse(SceneManager.GetActiveScene().name.Replace("Module", ""));

        SceneManager.sceneLoaded += onSceneLoaded;
        SceneManager.LoadScene("Main");
    }

    private void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main")
        {
            ControllersManager.Instance.GetController<FlowController>().moduleIndexToRun = sceneToLoadIndex;
        }
        SceneManager.sceneLoaded -= onSceneLoaded;
    }
}
