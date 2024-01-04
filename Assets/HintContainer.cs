using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintContainer : MonoBehaviour
{
    [SerializeField]
    private HintType hintType = default;
    public HintType HintType { get => hintType; private set => hintType = value; }


    [SerializeField]
    private List<Hint> hints = default;

    public List<Hint> Hints { get => hints; private set => hints = value; }

    private void Awake()
    {
        foreach (var hint in hints)
        {
            hint.hintType = hintType;
        }
    }

    public void SetHintState(bool state)
    {
        foreach (var hint in hints)
        {
            hint.gameObject.SetActive(state);
        }
    }

    public void SetHintState(string hintId, bool state)
    {
        hints.Find(x => x.Id == hintId).gameObject.SetActive(state);
    }
}
