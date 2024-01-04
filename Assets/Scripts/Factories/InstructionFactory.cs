using UnityEngine;

public class InstructionFactory
{
    public static Instruction CloneInstruction(Instruction instruction)
    {
        switch (instruction.type)
        {
            case (InstructionType.MOVE):
                return (Instruction)(new MoveInstruction((MoveInstruction)instruction));
            case (InstructionType.DELAY):
                return (Instruction)(new DelayInstruction((DelayInstruction)instruction));
            case (InstructionType.TEXT):
                return (Instruction)(new TextInstruction((TextInstruction)instruction));
            case (InstructionType.USE_TOOL):
                return (Instruction)(new UseToolInstruction((UseToolInstruction)instruction));
            case (InstructionType.DIGITAL_IN):
                return (Instruction)(new DigitalInInstruction((DigitalInInstruction)instruction));
            case (InstructionType.IF_BLOCK):
                return (Instruction)(new IfBlockInstruction((IfBlockInstruction)instruction));
            default:
                return (Instruction)(new EmptyInstruction());
        }
    }

    public static T CreateInstruction<T>() where T : Instruction
    {
        return (T)ScriptableObject.CreateInstance(typeof(T));
    }

    public static Instruction CreateBaseInstruction<T>() where T : Instruction
    {
        return (Instruction)ScriptableObject.CreateInstance(typeof(T));
    }

}