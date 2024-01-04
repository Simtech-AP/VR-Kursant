using System;
using UnityEngine;



[CreateAssetMenu(menuName = "Instructions/DigitalInInstruction")]

public class DigitalInInstruction : Instruction
{
    public bool targetState = default;
    public uint bitIndex = default;

    public DigitalInInstruction()
    {
        type = InstructionType.DIGITAL_IN;
        maxSelectablePartIndex = 2;
    }

    public DigitalInInstruction(DigitalInInstruction other) : base(other)
    {
        this.targetState = other.targetState;
        this.bitIndex = other.bitIndex;
    }

    protected override string generateInstructionSpecificText()
    {
        return "SET DIN[" + bitIndex + "] TO " + (targetState ? "1" : "0");
    }

    public override void ProcessInput(string input)
    {
        if (selectedPart == 1)
        {
            if (char.IsDigit(input[0]))
            {
                if (isPartEditedFirstTime)
                {
                    bitIndex = uint.Parse(input);
                    isPartEditedFirstTime = false;
                }
                else
                {
                    bitIndex = bitIndex * 10 + uint.Parse(input);
                }

                bitIndex = (uint)Mathf.Clamp(bitIndex, 0, DigitalInput.Instance.array.Count - 1);
            }
        }
        else if (selectedPart == 2)
        {
            if (input.Equals("Enter"))
            {
                targetState = !targetState;
            }
            else if (input.Equals("0"))
            {
                targetState = false;
            }
            else if (char.IsDigit(input[0]))
            {
                targetState = true;
            }
        }


    }

    protected override Tuple<int, int> selectPart(int part)
    {
        selectedPart = part;

        var _selectionRange = new Tuple<int, int>(0, 5);

        var indexDigitCount = 1;
        if (bitIndex != 0)
        {
            indexDigitCount = Mathf.FloorToInt(Mathf.Log10(bitIndex) + 1);
        }

        if (part == 1)
        {
            _selectionRange = new Tuple<int, int>(8, 8 + indexDigitCount);
        }
        else if (part == 2)
        {
            _selectionRange = new Tuple<int, int>(8 + indexDigitCount + 5, 8 + indexDigitCount + 6);
        }

        return _selectionRange;
    }
}