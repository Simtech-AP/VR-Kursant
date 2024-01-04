using UnityEngine;

public class GazeController : Controller
{
    [SerializeField] private Gazer playerGazer;

    [SerializeField] private float defaultGazeTime;

    protected override void InitalizeControllers()
    {
        playerGazer = FindObjectOfType<Gazer>();
    }

    public void SetGazeTime(float time)
    {
        playerGazer.SetGazeTime(time);
    }

    public void ResetGazeTimeToDefault()
    {
        playerGazer.SetGazeTime(defaultGazeTime);
    }

    public void EnableGazerGuiCircle()
    {
        playerGazer.ToggleGuiCircle(true);
    }

    public void DisableGazerGuiCircle()
    {
        playerGazer.ToggleGuiCircle(false);
    }
}
