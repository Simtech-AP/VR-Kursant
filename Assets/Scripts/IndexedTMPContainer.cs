using System;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using TMPro;

[System.Serializable]
public class IndexedTMPContainer
{
    public int index;

    [SerializeField] private List<TextMeshProUGUI> container = new List<TextMeshProUGUI>();

    public List<TextMeshProUGUI> Container { get => container; }
}