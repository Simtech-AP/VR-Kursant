using TMPro;
using UnityEngine;
using System;

[System.Serializable]
public class ProgramUIData
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;

}

public class ProgramUI : MonoBehaviour
{
    public ProgramUIData Data;

    public ProgramList ProgramList;
    public PendantController pendantController;

    [SerializeField]
    private TextMeshProUGUI changeableTitle;

    private Action onOkButtonPressed = delegate { };

    private void OnEnable()
    {
        SetActionToAddProgram();
    }

    public void ExecuteOnButtonPressed()
    {
        onOkButtonPressed?.Invoke();
    }

    public void SetActionToAddProgram()
    {
        changeableTitle.text = "Set name for new program";
        onOkButtonPressed = addProgram;
    }

    private void addProgram()
    {
        pendantController.AddProgram(Data);
    }

    public void SetActionToRenameProgram()
    {
        changeableTitle.text = "Set new name for program";
        onOkButtonPressed = renameProgram;
    }

    private void renameProgram()
    {
        Program pr = PendantData.Instance.EditedProgram;
        if (pr != null)
        {
            pr.Name = Data.Name.text;
            pr.Description = Data.Description.text;
        }
        else
        {
            pendantController.RenameCurrentProgram(Data);
        }
        PendantData.OnUpdatedData?.Invoke(PendantData.Instance);
    }

    public void SetActionToDuplicateProgram()
    {
        changeableTitle.text = "Set name for duplicated program";
        onOkButtonPressed = duplicateProgram;
    }

    private void duplicateProgram()
    {
        Program toDuplicate = ProgramList.GetSelectedProgram();
        if (toDuplicate != null)
        {
            pendantController.DuplicateProgram(toDuplicate, Data);
        }
    }
}