using UnityEngine;
using UnityEngine.Events;

public class PlayButtonChecker : StepEnabler
{
    private bool rightDeadman = false;
    private bool leftDeadman = false;

    public void RightDeadmanToggle(bool flag)
    {
        rightDeadman = flag;
    }

    public void LeftDeadmanToggle(bool flag)
    {
        leftDeadman = flag;
    }

    public void PlayPressedOnOneDeadman()
    {
        if (rightDeadman || leftDeadman)
        {
            Enabled = true;
        }
    }
}
