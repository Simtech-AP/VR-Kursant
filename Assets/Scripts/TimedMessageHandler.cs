using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimedMessageHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private int defaultTimeout;
    private Coroutine currentTimeoutProcedure = default;

    public void SetTextField(string text, bool persistent = true)
    {
        textField.text = text;

        if (!persistent)
        {
            if (currentTimeoutProcedure != null)
            {
                StopCoroutine(currentTimeoutProcedure);
            }
            currentTimeoutProcedure = StartCoroutine(timeoutProcedure(defaultTimeout));
        }
    }

    public void SetTextField(string text, int customTimeout)
    {
        textField.text = text;

        if (currentTimeoutProcedure != null)
        {
            StopCoroutine(currentTimeoutProcedure);
        }
        currentTimeoutProcedure = StartCoroutine(timeoutProcedure(customTimeout));
    }

    private IEnumerator timeoutProcedure(int timeOut)
    {
        yield return new WaitForSecondsRealtime(timeOut);
        textField.text = "";
    }

}
