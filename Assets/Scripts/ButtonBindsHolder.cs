using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

class ButtonBindsHolder : MonoBehaviour
{
    [SerializeField] private List<ButtonBind> buttonNameBinds;

    public GameObject GetButtonObjectById(string buttonId)
    {
        return buttonNameBinds.Find((x) => { return x.ButtonID == buttonId; }).Button;
    }
}

