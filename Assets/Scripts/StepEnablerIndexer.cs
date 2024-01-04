using System;


[Serializable]
public class StepEnablerIndexer
{
    public int index;
    public StepEnabler stepEnabler;
    public Action<int, bool> SendIndex = delegate { };


    public void Initialize()
    {
        stepEnabler.OnEnabledStateReached.AddListener(GetIndex);
        stepEnabler.OnEnabledStateLost.AddListener(GetIndex);
    }

    public void GetIndex()
    {
        SendIndex.Invoke(index, stepEnabler.Enabled);
    }

    internal void CleanUp()
    {
        stepEnabler.OnEnabledStateReached.RemoveListener(GetIndex);
        stepEnabler.OnEnabledStateLost.RemoveListener(GetIndex);
    }
}