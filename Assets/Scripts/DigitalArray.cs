using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DigitalArray
{
    [Serializable]
    public class DigitalArrayBit
    {
        private bool bitState;

        [SerializeField]
        private UnityEvent onSet;

        [SerializeField]
        private UnityEvent onReset;

        public bool BitState
        {
            get
            {
                return bitState;
            }

            set
            {
                bitState = value;
                if (bitState)
                {
                    onSet.Invoke();
                }
                else
                {
                    onReset.Invoke();
                }
            }
        }

        public void Set()
        {
            if (BitState != true)
                BitState = true;
        }

        public void Reset()
        {
            if (BitState != false)
                BitState = false;
        }

        public void Toggle()
        {
            BitState = !BitState;
        }
    }

    [SerializeField]
    private List<DigitalArrayBit> bits = default;

    public int Count { get { return bits.Count; } }

    public void SetBit(uint index)
    {
        if (index >= 0 && index < bits.Count)
        {
            bits[(int)index].Set();
        }
    }

    public void ResetBit(uint index)
    {
        if (index >= 0 && index < bits.Count)
        {
            bits[(int)index].Reset();
        }
    }

    public void ToggleBit(uint index)
    {
        if (index >= 0 && index < bits.Count)
        {
            bits[(int)index].Toggle();
        }
    }
}