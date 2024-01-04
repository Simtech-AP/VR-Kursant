using System;
using System.Collections.Generic;
using UnityEngine;

public class AidEnabler : MonoBehaviour
{
    [Serializable]
    private class HintsList
    {
        public HintType HintType = default;
        public bool enableAll = default;
        public List<string> Ids = default;
    }


    [SerializeField]
    private List<HintsList> targetTypes = default;

    private AidesController aidesController = default;

    private void Awake()
    {
        aidesController = ControllersManager.Instance.GetController<AidesController>();
    }

    public void EnableAids()
    {
        setHintsState(true);
    }

    public void DisableAids()
    {
        setHintsState(false);
    }

    private void setHintsState(bool targetState)
    {
        foreach (var type in targetTypes)
        {
            if (type.enableAll == true)
            {
                aidesController.SetStateForAidType(type.HintType, targetState);
            }
            else
            {
                foreach (var id in type.Ids)
                {
                    aidesController.SetStateForAidType(type.HintType, id, targetState);
                }
            }

        }
    }


}
