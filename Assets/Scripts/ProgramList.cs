using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum ProgramListAction
{
    SELECT = 0,
    REMOVE = 1,
    DUPLICATE = 2
}

public class ProgramList : MonoBehaviour
{
    public PendantController pendantController;
    public MenuList programListMenu;
    public InputContainer programCanvas;

    private Program selectedProgram;

    public UnityEvent onProgramDuplication;
    public UnityEvent onProgramRemoval;

    private void OnEnable()
    {
        SetProgramListAction();
        selectedProgram = PendantData.Instance.SavedPrograms[programListMenu.OptionIndex];
        programListMenu.SelectOption();
    }

    public void SetProgramListAction(int actionType = (int)ProgramListAction.SELECT)
    {
        for (int i = 0; i < PendantData.Instance.SavedPrograms.Count; ++i)
        {

            if (programListMenu.EventList[i] != null)
            {
                programListMenu.EventList[i].RemoveAllListeners();
            }

            int index = programListMenu.EventList.IndexOf(programListMenu.EventList[i]);

            Action action = delegate { };

            switch (actionType)
            {
                case (int)ProgramListAction.SELECT:
                    action = () => SetProgram(index);
                    break;
                case (int)ProgramListAction.DUPLICATE:
                    action = () => DuplicateProgram();
                    break;
                case (int)ProgramListAction.REMOVE:
                    action = () => DeleteProgram();
                    break;
            }

            programListMenu.EventList[index].AddListener(() => action());
        }

        Debug.Log(actionType);
    }


    private void Update()
    {
        try
        {
            selectedProgram = PendantData.Instance.SavedPrograms[programListMenu.OptionIndex];
        }
        catch
        {
            selectedProgram = PendantData.Instance.SavedPrograms[0];
        }
    }

    private void SetProgram(int i)
    {
        gameObject.SetActive(false);
        programCanvas.enabled = true;
        pendantController.SetCurrentProgram(PendantData.Instance.SavedPrograms[i]);
    }

    private void DuplicateProgram()
    {
        onProgramDuplication.Invoke();
    }


    public void DeleteProgram()
    {
        onProgramRemoval.Invoke();

        var selectedIndex = PendantData.Instance.SavedPrograms.IndexOf(selectedProgram);

        if (selectedProgram != null)
        {
            pendantController.DeleteProgram(selectedProgram);
        }

        if (selectedIndex == PendantData.Instance.SavedPrograms.Count)
        {
            programListMenu.ChangeOption(false);
        }
        else
        {
            programListMenu.SelectOption();
        }
    }

    public Program GetSelectedProgram()
    {
        if (selectedProgram == null)
        {
            selectedProgram = PendantData.Instance.SavedPrograms[programListMenu.OptionIndex];
        }

        return selectedProgram;
    }

    public void ClearSelectedProgram()
    {
        selectedProgram = null;
    }

    private void OnDisable()
    {
        // ClearSelectedProgram();
    }
}
