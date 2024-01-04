using UnityEngine;

public class ProgramDeletedTarget : StepEnabler
{

    protected override void initialize()
    {
        PendantController.OnProgramDeleted += onSuccessfulDeletion;
    }

    private void onSuccessfulDeletion()
    {
        Enabled = true;
    }

    protected override void cleanUp()
    {
        PendantController.OnProgramDeleted -= onSuccessfulDeletion;
    }
}