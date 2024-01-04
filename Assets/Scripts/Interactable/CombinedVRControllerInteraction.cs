using System.Collections.Generic;
using System.Linq;

public class CombinedVRControllerInteraction : VRControllerInteraction
{
    private List<VRControllerInteraction> combinedInteractions = new List<VRControllerInteraction>();

    public override bool Down
    {
        get
        {
            var recentDowns = combinedInteractions.Where(x => x.Stay && x.InteractionTime < 200).OrderBy(x => x.InteractionTime).ToList();
            var identicalRecentDowns = recentDowns.Where(x => !(x.InteractionTime == recentDowns[0].InteractionTime)).ToList();
            return combinedInteractions.Any(x => x.Down == true) && identicalRecentDowns.Count == 0;
        }
    }
    public override bool Up { get { return combinedInteractions.Any(x => x.Up == true); } }

    public override bool Stay
    {
        get
        {
            return combinedInteractions.Any(x => x.Stay == true); ;
        }
    }

    public CombinedVRControllerInteraction(VRControllerInteraction[] interactions)
    {
        combinedInteractions = interactions.ToList();
    }

    public CombinedVRControllerInteraction()
    {
    }
}