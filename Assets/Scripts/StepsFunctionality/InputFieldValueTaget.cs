using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputFieldValueTaget : StepEnabler
{
    [Header("Input Field Value Taget")]
    [SerializeField] private bool isAnyInputAcceptable;
    [SerializeField] private string targetInput = "";
    [SerializeField] private string inputFieldName;

    private TextMeshProUGUI inputField;

    protected override void initialize()
    {
        var pendantUI = FindObjectOfType<PendantUI>();

        try
        {
            inputField = pendantUI.TMPInputFields.Find((x) => { return x.name == inputFieldName; }).item;
        }
        catch
        {
            Debug.LogError("No Pendant UI found");
        }

    }

    public void AssertCorrectInputFieldValue()
    {
        var trimmedInput = inputField.text.Remove(inputField.text.Length - 1);

        if ((isAnyInputAcceptable && trimmedInput.Length > 0) || (!isAnyInputAcceptable && targetInput == trimmedInput))
        {
            Enabled = true;
        }
    }
}
