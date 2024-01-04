using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum EditInstructionType
{
    INSERT_UNDER,
    INSERT_ABOVE
}

public class EditInstructionUI : MonoBehaviour
{
    public TextMeshProUGUI amount;
    public EditInstructionType funcionalityType;
    public PendantController pendantController;

    public void Interact()
    {
        try
        {
            int arrSize;
            var text = amount.text.Remove(amount.text.Length - 1);
            int.TryParse(text, out arrSize);
            Debug.Log("arrsize: " + arrSize);
            switch (funcionalityType)
            {
                case EditInstructionType.INSERT_UNDER:
                    for (int i = 0; i < arrSize; ++i)
                    {
                        pendantController.InsertInstructionUnder(new EmptyInstruction());
                    }
                    break;
                case EditInstructionType.INSERT_ABOVE:
                    for (int i = 0; i < arrSize; ++i)
                    {
                        pendantController.InsertInstructionAbove(new EmptyInstruction());
                    }
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
