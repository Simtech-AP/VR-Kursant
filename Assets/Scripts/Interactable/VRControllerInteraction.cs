using System;
using UnityEngine;

public class VRControllerInteraction
{
    private long interactionTime;
    private bool down = default;
    private bool up = default;
    private bool stay = default;

    public virtual bool Down { get { return down; } }
    public virtual bool Up { get { return up; } }

    public virtual bool Stay
    {
        get
        {
            return stay;
        }

        set
        {
            if (value != stay && value == false)
            {
                up = true;
                interactionTime = 0;
            }
            else if (value != stay && value == true)
            {
                down = true;
                interactionTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
            }
            else
            {
                up = false;
                down = false;
            }

            stay = value;
        }
    }

    public virtual long InteractionTime
    {
        get
        {
            if (interactionTime != 0)
            {
                return ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds() - interactionTime;
            }
            else return 0;
        }
    }
}