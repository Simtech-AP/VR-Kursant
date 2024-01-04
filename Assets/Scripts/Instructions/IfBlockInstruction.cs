using System;
using UnityEngine;

public enum IfBlockInstructionType
{
    IF,
    ELSE_IF,
    ELSE,
    END_IF

}

public class IfBlockInstruction : Instruction
{
    public IfBlockInstructionType lineType { private set; get; }

    public uint DigitalInIndex = default;
    public bool IsComparingAsEqual = default;
    public bool ComparisonValue = default;

    public IfBlockInstruction()
    {
        type = InstructionType.IF_BLOCK;
        DigitalInIndex = 0;
        IsComparingAsEqual = true;
        ComparisonValue = true;
        SetBlockType(IfBlockInstructionType.IF);

    }

    public void SetBlockType(IfBlockInstructionType _type)
    {
        lineType = _type;
        switch (lineType)
        {
            case IfBlockInstructionType.IF:
                maxSelectablePartIndex = 3;
                lineIndentationEffect = IndentationEffect.BLOCK_OPENER;
                break;
            case IfBlockInstructionType.ELSE_IF:
                maxSelectablePartIndex = 3;
                lineIndentationEffect = IndentationEffect.BLOCK_INTERSECTOR;
                break;
            case IfBlockInstructionType.ELSE:
                maxSelectablePartIndex = 0;
                lineIndentationEffect = IndentationEffect.BLOCK_INTERSECTOR;
                break;
            case IfBlockInstructionType.END_IF:
                maxSelectablePartIndex = 0;
                lineIndentationEffect = IndentationEffect.BLOCK_ENDER;
                break;

        }
    }

    public IfBlockInstruction(IfBlockInstruction other) : base(other)
    {
        this.DigitalInIndex = other.DigitalInIndex;
        this.IsComparingAsEqual = other.IsComparingAsEqual;
        this.ComparisonValue = other.ComparisonValue;
    }

    protected override string generateInstructionSpecificText()
    {
        switch (lineType)
        {
            case IfBlockInstructionType.IF:
                return getIfInstructionText();
            case IfBlockInstructionType.ELSE_IF:
                return getElseIfInstructionText();
            case IfBlockInstructionType.ELSE:
                return getElseInstructionText();
            case IfBlockInstructionType.END_IF:
                return getEndIfInstructionText();
            default:
                return "...";
        }
    }

    private string getIfInstructionText()
    {
        var comparisonSign = IsComparingAsEqual ? "==" : "!=";
        var comparisonValueAsNumber = ComparisonValue ? "1" : "0";
        return "IF (DIN[" + DigitalInIndex + "] " + comparisonSign + " " + comparisonValueAsNumber + "):";
    }

    private string getElseIfInstructionText()
    {
        var comparisonSign = IsComparingAsEqual ? "==" : "!=";
        var comparisonValueAsNumber = ComparisonValue ? "1" : "0";
        return "ELSE IF (DIN[" + DigitalInIndex + "] " + comparisonSign + " " + comparisonValueAsNumber + "):";
    }

    private string getElseInstructionText()
    {
        return "ELSE:";
    }

    private string getEndIfInstructionText()
    {
        return "END IF";
    }

    public override void ProcessInput(string input)
    {
        if (lineType == IfBlockInstructionType.IF || lineType == IfBlockInstructionType.ELSE_IF)
        {
            ProcessInputForIfInstruction(input);
        }
    }

    private void ProcessInputForIfInstruction(string input)
    {
        if (selectedPart == 1)
        {
            if (char.IsDigit(input[0]))
            {
                if (isPartEditedFirstTime)
                {
                    DigitalInIndex = uint.Parse(input);
                    isPartEditedFirstTime = false;
                }
                else
                {
                    DigitalInIndex = DigitalInIndex * 10 + uint.Parse(input);
                }

                DigitalInIndex = (uint)Mathf.Clamp(DigitalInIndex, 0, DigitalInput.Instance.array.Count - 1);
            }
        }
        else if (selectedPart == 2)
        {
            if (input == "Enter")
            {
                IsComparingAsEqual = !IsComparingAsEqual;
            }
        }
        else if (selectedPart == 3)
        {
            if (input == "Enter")
            {
                ComparisonValue = !ComparisonValue;
            }
            else if (input.Equals("0"))
            {
                ComparisonValue = false;
            }
            else if (char.IsDigit(input[0]))
            {
                ComparisonValue = true;
            }
        }
    }

    protected override Tuple<int, int> selectPart(int part)
    {
        selectedPart = part;

        var _selectionRange = new Tuple<int, int>(0, 5);

        if (lineType == IfBlockInstructionType.IF || lineType == IfBlockInstructionType.ELSE_IF)
        {
            _selectionRange = selectPartForIfAndElseIf();
        }

        return _selectionRange;

    }

    private Tuple<int, int> selectPartForIfAndElseIf()
    {
        var indexDigitCount = 1;
        if (DigitalInIndex != 0)
        {
            indexDigitCount = Mathf.FloorToInt(Mathf.Log10(DigitalInIndex) + 1);
        }

        var _selectionRange = new Tuple<int, int>(0, 5);

        if (selectedPart == 1)
        {
            _selectionRange = new Tuple<int, int>(8, 8 + indexDigitCount);
        }
        else if (selectedPart == 2)
        {
            _selectionRange = new Tuple<int, int>(8 + indexDigitCount + 2, 8 + indexDigitCount + 4);
        }
        else if (selectedPart == 3)
        {
            _selectionRange = new Tuple<int, int>(8 + indexDigitCount + 5, 8 + indexDigitCount + 6);
        }

        if (lineType == IfBlockInstructionType.ELSE_IF)
        {
            _selectionRange = new Tuple<int, int>(_selectionRange.Item1 + 5, _selectionRange.Item2 + 5);
        }

        return _selectionRange;
    }
}