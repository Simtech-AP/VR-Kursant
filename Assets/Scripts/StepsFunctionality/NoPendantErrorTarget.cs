using UnityEngine;

public class NoPendantErrorTarget : StepEnabler
{
    [Header("No Pendant Error Target")]
    [SerializeField] private bool checkErrors;
    [SerializeField] private bool checkAlarms;

    protected override void onUpdate()
    {
        CheckForPendantErrors();
    }

    public void CheckForPendantErrors()
    {
        var areErrorsReset = ErrorRequester.HasAllErrorsReset();
        var areAlarmsReset = ErrorRequester.HasResetAlarmErrors();

        Enabled = (areErrorsReset || !checkErrors) && (areAlarmsReset || !checkAlarms);
    }
}
