using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class InputFieldData
{
    public TMP_InputField textField = default;
    public int characterLimit = default;

    public static InputFieldData None { get { return new InputFieldData(); } }
}

public class KeyboardUI : MonoBehaviour
{

    [SerializeField]
    private bool clearInputFieldsOnInit = default;

    [SerializeField]
    private List<InputFieldData> inputFieldsData = default;

    public MenuList MenuList;
    public Keyboard KeyboardLogic;

    private void OnEnable()
    {
        if (clearInputFieldsOnInit)
        {
            foreach (var inputField in inputFieldsData)
            {
                inputField.textField.text = string.Empty;
            }
        }

        SetKeyboardTargetInputField();
    }

    public void SetKeyboard()
    {
        try
        {
            KeyboardLogic.Initialize(inputFieldsData[MenuList.OptionIndex]);
        }
        catch { }
    }

    public void SetKeyboardTargetInputField()
    {
        try
        {
            KeyboardLogic.SetCurrentTextField(inputFieldsData[MenuList.OptionIndex]);
        }
        catch
        {
            KeyboardLogic.ClearInputField();
        }

    }

    public void DisableKeyboard()
    {
        KeyboardLogic.DisableKeayboard();
    }

    private void OnDisable()
    {
        DisableKeyboard();
    }
}
