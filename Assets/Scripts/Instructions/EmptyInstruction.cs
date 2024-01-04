using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyInstruction : Instruction
{
    public EmptyInstruction()
    {
        isCommented = false;
        type = InstructionType.EMPTY;
    }

    protected override string generateInstructionSpecificText()
    {
        return "";
    }

    public override void ProcessInput(string input) { }

    protected override Tuple<int, int> selectPart(int part)
    {
        return new Tuple<int, int>(0, 5);
    }
}
