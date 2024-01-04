using System;
using UnityEngine;

/// <summary>
/// Allows creting a waitinig/delay instruction in program
/// </summary>
[CreateAssetMenu(menuName = "Instructions/DelayInstruction")]
public class DelayInstruction : Instruction
{
    /// <summary>
    /// Dealy time in sceonds
    /// </summary>
    public float delayTime = 1f;
    /// <summary>
    /// Is this the first edit of a part of instruction?
    /// </summary>
    private bool isEditingDecimals = false;

    /// <summary>
    /// Default contructor
    /// Set standard info like type of instruction etc.
    /// </summary>
    public DelayInstruction()
    {
        type = InstructionType.DELAY;
        maxSelectablePartIndex = 1;
    }

    public DelayInstruction(DelayInstruction other) : base(other)
    {
        this.delayTime = other.delayTime;
    }

    protected override string generateInstructionSpecificText()
    {

        return "DLY " + delayTime.ToString("0.00");
    }

    public override void ProcessInput(string input)
    {
        if (isPartEditedFirstTime)
        {
            isPartEditedFirstTime = false;
            if (char.IsDigit(input[0]) && !isEditingDecimals)
            {

                delayTime = int.Parse(input);

            }
            else if (char.IsDigit(input[0]) && isEditingDecimals)
            {

                delayTime = delayTime + int.Parse(input) / 10f;

            }
            else if (input == ".")
            {
                delayTime = 0f;
                isEditingDecimals = true;
                isPartEditedFirstTime = true;
            }
        }
        else
        {
            if (char.IsDigit(input[0]))
            {
                if (!isEditingDecimals)
                {
                    delayTime = Mathf.Clamp(delayTime * 10 + int.Parse(input), 0, 99.99f);
                }
                else if (isEditingDecimals)
                {
                    delayTime = Mathf.Clamp(delayTime + int.Parse(input) / 100f, 0, 99.99f);

                }
            }
            else if (input == ".")
            {
                if (delayTime == Mathf.Floor(delayTime))
                {
                    isEditingDecimals = true;
                    isPartEditedFirstTime = true;
                }
            }
        }

        if (input == "PREV")
        {
            int wholePart = Mathf.FloorToInt(delayTime);
            int decimalPart = (int)((delayTime - wholePart) * 100f);

            if (decimalPart != 0)
            {
                if (decimalPart % 10 != 0)
                {
                    delayTime = wholePart + (float)((int)(decimalPart / 10)) / 10f;
                }
                else
                {
                    delayTime = wholePart;
                }
            }
            else
            {
                if (wholePart % 10 != 0)
                {
                    delayTime = (int)(wholePart / 10);
                }
                else
                {
                    delayTime = 0;
                }
            }
        }
    }

    protected override Tuple<int, int> selectPart(int part)
    {
        selectedPart = part;
        switch (part)
        {
            case 1:
                var startIndex = instructionText.IndexOf("Y ") + 1;
                var endIndex = instructionText.Length;
                return new Tuple<int, int>(startIndex + 1, endIndex);
        }
        return new Tuple<int, int>(0, 5);
    }
}
