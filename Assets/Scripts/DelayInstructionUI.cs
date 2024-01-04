using System;
using System.Globalization;
using TMPro;
using UnityEngine;


public class DelayInstructionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI delayTimeInputField;

    [SerializeField] private PendantController pendantController;

    public void Interact()
    {
        try
        {
            float dlyTime = 0.0f;
            var text = delayTimeInputField.text.Remove(delayTimeInputField.text.Length - 1);

            if (float.TryParse(text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out dlyTime))
            {
                var instruction = InstructionFactory.CreateBaseInstruction<DelayInstruction>();
                ((DelayInstruction)instruction).delayTime = dlyTime;
                pendantController.InsertInstructionAbove(instruction);
            }
            else
            {
                Debug.LogError("Cannot parse " + text + " into number");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}
