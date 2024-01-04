using UnityEngine;

public class MaterialCleaner : MonoBehaviour
{

    [SerializeField] private RobotMaterialContainer materialContainer = default;
    [SerializeField] private Transform cleanUpTarget = default;
    [SerializeField] private MaterialSwitcher materialSwitcher = default;

    // TODO do some dependency injection instead
    private void Awake()
    {
        cleanUpTarget = ControllersManager.Instance.GetController<RobotController>().CurrentRobot.transform;
    }

    public void CleanTarget()
    {
        var allTargets = cleanUpTarget.GetComponentsInChildren<MeshRenderer>();

        var material = materialContainer.Materials[FindObjectOfType<Robot>().GetType()];

        foreach (var t in allTargets)
        {
            materialSwitcher.SwitchMaterial(t, material);
        }
    }

}