using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class DinInstructionUI : MonoBehaviour
{
    [SerializeField] private PendantController pendantController;

    [SerializeField] private TextMeshProUGUI indexInputField;
    [SerializeField] private TextMeshProUGUI targetStateField;


    private bool targetState = default;

    public void StateFieldInteract()
    {
        if (targetStateField.text != "OFF")
        {
            targetStateField.text = "OFF";
            targetState = false;
        }
        else
        {
            targetStateField.text = "ON";
            targetState = true;
        }
    }

    public void OkButtonInteract()
    {
        try
        {
            uint arrayIndex = 0;
            uint.TryParse(indexInputField.text.Trim(), out arrayIndex);

            var instruction = InstructionFactory.CreateInstruction<DigitalInInstruction>();
            instruction.bitIndex = arrayIndex;
            instruction.targetState = targetState;

            pendantController.InsertInstructionAbove(instruction);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}