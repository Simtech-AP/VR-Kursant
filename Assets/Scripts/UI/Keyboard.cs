using TMPro;
using UnityEngine;

/// <summary>
/// Class allowing for keyboard-linke inpout for program name and description, comments in program etc.
/// </summary>
public class Keyboard : MonoBehaviour
{
    /// <summary>
    /// Direction of change of key in keyboard
    /// </summary>
    public enum KeyboardKeyDirection
    {
        Up,
        Down,
        Left,
        Right
    }
    /// <summary>
    /// Currently selected key
    /// </summary>
    [SerializeField]
    private KeyboardKey currentKey = default;
    /// <summary>
    /// Graphic representation of selected key, uses TextMeshProUGUI to show key code below the selector
    /// </summary>
    [SerializeField]
    private RectTransform selector = default;
    /// <summary>
    /// Text field to show inputted string
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI templateTextField = default;
    /// <summary>
    /// Currently selected text input field
    /// </summary>
    [SerializeField]
    private InputFieldData inputFieldData = default;
    /// <summary>
    /// InputContaner usend when editing program.
    /// Needs reference here to enable/disable it to account for any inputs
    /// </summary>
    [SerializeField]
    public InputContainer programNameScreenInput;

    public GameObject KeyboardUIObject;

    /// <summary>
    /// When enabled keyboard disable inputs according to active screen
    /// </summary>
    public void Initialize(InputFieldData activeInput)
    {
        SetCurrentTextField(activeInput);

        KeyboardUIObject.SetActive(true);

        if (programNameScreenInput.gameObject.activeInHierarchy)
        {
            programNameScreenInput.enabled = false;
        }
    }

    /// <summary>
    /// Re-enable inputs when hiding keyboard
    /// </summary>
    public void DisableKeayboard()
    {
        KeyboardUIObject.SetActive(false);

        if (programNameScreenInput.gameObject.activeInHierarchy)
        {
            programNameScreenInput.enabled = true;
        }
    }

    /// <summary>
    /// Changes active key using direction from enumeration, selects it using selector
    /// </summary>
    /// <param name="direction">Direction in which change key to</param>
    public void ChangeKey(int direction)
    {
        switch ((KeyboardKeyDirection)direction)
        {
            case KeyboardKeyDirection.Up:
                if (currentKey.upKey)
                    currentKey = currentKey.upKey;
                break;
            case KeyboardKeyDirection.Down:
                if (currentKey.downKey)
                    currentKey = currentKey.downKey;
                break;
            case KeyboardKeyDirection.Left:
                if (currentKey.leftKey)
                    currentKey = currentKey.leftKey;
                break;
            case KeyboardKeyDirection.Right:
                if (currentKey.rightKey)
                    currentKey = currentKey.rightKey;
                break;
        }
        selector.anchoredPosition = (Vector3)currentKey.GetComponent<RectTransform>().anchoredPosition + new Vector3(0, 0, -0.003f);
        selector.GetComponentInChildren<TextMeshProUGUI>().text = currentKey.GetKeyString();
    }

    /// <summary>
    /// Uses currently selected key to input text
    /// </summary>
    public void UseKey()
    {
        if (inputFieldData != null)
        {
            if (inputFieldData.characterLimit != 0 && inputFieldData.textField.text.Length == inputFieldData.characterLimit) return;

            inputFieldData.textField.text += currentKey.GetKeyString();
            templateTextField.text = inputFieldData.textField.text;
        }

    }

    public void UseKey(string name)
    {
        if (inputFieldData != null)
        {
            if (inputFieldData.characterLimit != 0 && inputFieldData.textField.text.Length == inputFieldData.characterLimit) return;

            inputFieldData.textField.text += name;
            templateTextField.text = inputFieldData.textField.text;
        }
    }

    /// <summary>
    /// Uses currently selected key as a backspace
    /// </summary>
    public void Backspace()
    {
        if (inputFieldData != null && inputFieldData.textField.text.Length >= 1)
        {
            inputFieldData.textField.text = inputFieldData.textField.text.Substring(0, templateTextField.text.Length - 1);
            templateTextField.text = inputFieldData.textField.text;
        }

    }

    /// <summary>
    /// Sets currently edited text input field
    /// </summary>
    /// <param name="textField">Input field to parse text to</param>
    public void SetCurrentTextField(InputFieldData _data)
    {
        inputFieldData = _data;
    }

    public void ClearInputField()
    {
        inputFieldData = null;
    }
}
