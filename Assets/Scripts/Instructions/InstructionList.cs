using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Structure of a single element available in a list of instructions
/// </summary>
[System.Serializable]
public struct InstructionListElement
{
    /// <summary>
    /// Text shown in instruction list
    /// </summary>
    public MovementType option;
    public InstructionToolAction toolAction;
    /// <summary>
    /// Template for an instruction
    /// </summary>
    public Instruction instruction;
}

/// <summary>
/// List of all available instructions
/// </summary>
public class InstructionList : MonoBehaviour
{
    /// <summary>
    /// List providing all available instructions to select
    /// </summary>
    [SerializeField]
    private List<InstructionListElement> instructions = new List<InstructionListElement>();
    /// <summary>
    /// Reference to Pendant Controller
    /// </summary>
    private PendantController pendantController = default;

    /// <summary>
    /// Set up references
    /// </summary>
    private void Start()
    {
        pendantController = ControllersManager.Instance.GetController<PendantController>();
    }

    /// <summary>
    /// Chooses currently selected instruction and inserts it into the program
    /// </summary>
    /// <param name="index">Change instruction index to specified value</param>
    public void ChooseInstruction(int index)
    {
        var instruction = (Instruction)ScriptableObject.CreateInstance(instructions[index].instruction.GetType());
        if (instruction.type == InstructionType.MOVE)
        {
            if (instructions[index].option == MovementType.Joint)
            {
                ((MoveInstruction)instruction).movementType = InstructionMovementType.JOINT;
                ((MoveInstruction)instruction).Speed = 0.5f;
            }
            else
            {
                ((MoveInstruction)instruction).movementType = InstructionMovementType.LINEAR;
                ((MoveInstruction)instruction).Speed = 0.5f;
            }
            ((MoveInstruction)instruction).pointNumber = 0;
        }
        else if (instruction.type == InstructionType.USE_TOOL)
        {
            ((UseToolInstruction)instruction).actionType = instructions[index].toolAction;
        }
        else
        {
            ((DelayInstruction)instruction).delayTime = 1f;
        }
        pendantController.InsertInstructionAbove(instruction);
        gameObject.SetActive(false);
    }

}
