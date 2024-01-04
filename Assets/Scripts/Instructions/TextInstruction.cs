using System;
using UnityEngine;

/// <summary>
/// Class allowing to create text instruction - comment
/// </summary>
[CreateAssetMenu(menuName = "Instructions/TextInstruction")]
public class TextInstruction : Instruction
{
    public string text;

    public TextInstruction()
    {
        isCommented = true;
        type = InstructionType.TEXT;
        text = "";
    }

    public TextInstruction(TextInstruction other) : base(other)
    {
        this.text = other.text;
    }

    protected override string generateInstructionSpecificText()
    {
        return text;
    }

    public override void ProcessInput(string input)
    {
        if (selectedPart == 1)
        {
            text += input[0].ToString();
        }
    }

    protected override Tuple<int, int> selectPart(int part)
    {
        selectedPart = part;
        switch (part)
        {
            case 1:
                int startIndex = 0;
                startIndex = 0;
                startIndex = instructionText.IndexOf('!', startIndex);
                return new Tuple<int, int>(startIndex + 1, instructionText.Length);
        }
        return new Tuple<int, int>(0, 5);
    }
}
