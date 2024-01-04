using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressedButtonHighlighter : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    [SerializeField] private ButtonBindsHolder buttons;

    private float yPosition = -0.004f;

    public void SetHighlight(string ButtonID)
    {
        var buttonPosition = buttons.GetButtonObjectById(ButtonID).transform.localPosition;
        highlight.transform.localPosition = new Vector3(buttonPosition.x, yPosition, buttonPosition.z);
        highlight.SetActive(true);
    }

    public void UnsetHighlight()
    {
        highlight.SetActive(false);
    }
}
