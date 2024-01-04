using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StepEnablerObjectiveBind : MonoBehaviour
{
    [SerializeField] private List<StepEnablerIndexer> indexedSE;

    public void Initialize()
    {
        var objectiveUIController = ControllersManager.Instance.GetController<InstructionController>().uiObjectiveController;

        var targetIndexes = indexedSE.GroupBy(x => x.index).Select(group => group.Key).ToList();
        objectiveUIController.SetColorForBound(targetIndexes);

        foreach (var se in indexedSE)
        {
            se.SendIndex += objectiveUIController.setObjectiveState;
            se.Initialize();
        }
    }

    public void CleanUp()
    {
        var objectiveUIController = ControllersManager.Instance.GetController<InstructionController>().uiObjectiveController;

        foreach (var se in indexedSE)
        {
            se.SendIndex -= objectiveUIController.setObjectiveState;
            se.CleanUp();
        }
    }
}