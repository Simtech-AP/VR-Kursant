using System;
using System.Linq;
using UnityEngine;

/// <summary>
///  Enum defining types of instructions
/// </summary>
public enum InstructionType
{
    TEXT,
    MOVE,
    DELAY,
    USE_TOOL,
    EMPTY,
    DIGITAL_IN,
    IF_BLOCK
}

/// <summary>
/// Enum defining possible effects a line can have on indentation
/// </summary>
public enum IndentationEffect
{
    NONE,
    BLOCK_OPENER,
    BLOCK_ENDER,
    BLOCK_INTERSECTOR   // this means the line is one indentation level to the left
                        // compared to lines above and below (for example 'ELSE')

}

/// <summary>
/// Base class for an instruction
/// Every instruction should base of it
/// </summary>
[System.Serializable]
public abstract class Instruction : ScriptableObject
{
    /// <summary>
    /// Type of an instruction
    /// </summary>
    public InstructionType type;

    /// <summary>
    /// Currently selected part of an instruction
    /// </summary>
    public int selectedPart = 0;

    /// <summary>
    /// Full instruction text shown in program
    /// </summary>
    protected string instructionText { private set; get; }

    /// <summary>
    /// What is the index of last selectable instruction segment 
    /// </summary>
    public int maxSelectablePartIndex = default;

    /// <summary>
    /// Is this the first edit of a part of instruction?
    /// </summary>
    public bool isPartEditedFirstTime = false;

    /// <summary>
    /// Is the instruction commented?
    /// </summary>
    public bool isCommented = false;

    /// <summary>
    /// How does the line affect indentation
    /// </summary>
    public IndentationEffect lineIndentationEffect = default;

    /// <summary>
    /// What is the visual level of indentation for this line
    /// </summary>
    public int IndentationLevel = 0;

    /// <summary>
    /// How many spaces are added to instruction text per indentation level. Common for all instructions. 
    /// </summary>
    public static int SpacesPerIndentationLevel = 2;

    public Instruction() { }

    /// <summary>
    /// Base copy constructor
    /// </summary>
    /// <param name="other">Object to copy</param>    
    public Instruction(Instruction other)
    {
        this.type = other.type;
        this.selectedPart = other.selectedPart;
        this.instructionText = other.instructionText;
        this.isCommented = other.isCommented;
        this.maxSelectablePartIndex = other.maxSelectablePartIndex;
        this.lineIndentationEffect = other.lineIndentationEffect;
    }

    /// <summary>
    /// Called when any input is sent to instruction
    /// </summary>
    /// <param name="input">String of an input</param>
    public abstract void ProcessInput(string input);

    /// <summary>
    /// Get instruction text to be displayed
    /// </summary>
    /// <returns>Entire instruction text as string</returns>
    public string GetInstructionText()
    {
        instructionText = "";

        if (isCommented)
        {
            instructionText += "!";
        }

        instructionText += generateInstructionSpecificText();

        return instructionText;
    }

    /// <summary>
    /// Returns instruction specific part of text to be displayed
    /// </summary>
    protected abstract string generateInstructionSpecificText();


    public Tuple<int, int> SelectPart(int _part)
    {
        var _commentOffset = 0; //isCommented ? 1 : 0;
        var _indentationOffset = IndentationLevel * Instruction.SpacesPerIndentationLevel;

        var selectedPartRange = selectPart(_part);

        selectedPartRange = new Tuple<int, int>(selectedPartRange.Item1 + _commentOffset + _indentationOffset, selectedPartRange.Item2 + _commentOffset + _indentationOffset);

        return selectedPartRange;
    }

    /// <summary>
    /// Set selected for editing part
    /// </summary>
    /// <param name="_part">Number of part to select</param>
    /// <returns>Indexes required to highlight text properly</returns>
    protected abstract Tuple<int, int> selectPart(int _part);
}
