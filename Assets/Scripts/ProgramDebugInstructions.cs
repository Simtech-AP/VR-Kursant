using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

/// <summary>
/// Simple program to show robot operation
/// </summary>
public class ProgramDebugInstructions : MonoBehaviour
{
    /// <summary>
    /// Should the program run at start of an application?
    /// </summary>
    public bool runProgram = true;
    private Program program = new Program();
    /// <summary>
    /// List of programs loaded onto separate robotss
    /// </summary>
    private Dictionary<string, Action> RobotsDefaultPrograms = new Dictionary<string, Action>();

    private RobotController robotController;
    private ProgramController programController;
    private PendantController pendantController;

    /// <summary>
    /// Set up few instructions to move robot in specified way
    /// </summary>
    private void Awake()
    {
        programController = ControllersManager.Instance.GetController<ProgramController>();
        pendantController = ControllersManager.Instance.GetController<PendantController>();

        program.Name = "DEBUG1";
        program.Description = "First debug program";

        Quaternion[] point1_angles = {  new Quaternion(0f, 0f, -0.9342579f, 0.356598079f),
                                        new Quaternion(0f, -0.1461041f, 0f, 0.989269257f),
                                        new Quaternion(0f, -0.207571089f, 0f, 0.97822f),
                                        new Quaternion(0.696578f, 0f, 0f, 0.717481f),
                                        new Quaternion(0f, 0.695076048f, 0f, 0.718936265f),
                                        new Quaternion(-0.93285656f, 0f, 0f, 0.360248178f),
                                        new Quaternion(0.497061044f, -0.5097087f, -0.490275562f, 0.502749741f)
                                        };

        Quaternion[] point2_angles = {  new Quaternion(0f, 0f, -0.3037144f, 0.9527632f),
                                        new Quaternion(0f, -0.322599f, 0f, 0.9465357f),
                                        new Quaternion(0f, 0.07203861f, 0f, 0.997401834f),
                                        new Quaternion(-0.253255218f, 0f, 0f, 0.9673995f),
                                        new Quaternion(0f, 0.282984227f, 0f, 0.959124565f),
                                        new Quaternion(-0.5246127f, 0f, 0f, 0.85134095f),
                                        new Quaternion(0.4970611f, -0.509708643f, -0.490275621f, 0.5027497f)
                                        };

        Quaternion[] point3_angles = {  new Quaternion(0f, 0f, -0.654598534f, 0.755976737f),
                                        new Quaternion(0f, -0.128490686f, 0f, 0.9917108f),
                                        new Quaternion(0f, 0.08575487f, 0f, 0.9963163f),
                                        new Quaternion(0.204257414f, 0f, 0f, 0.9789173f),
                                        new Quaternion(0f, 0.500440061f, 0f, 0.865771234f),
                                        new Quaternion(-0.931233f, 0f, 0f, 0.3644244f),
                                        new Quaternion(0.497061044f, -0.5097088f, -0.490275621f, 0.5027496f)
                                        };

        Point point1 = new Point(new Vector3(0.7f, 0.8f, 1f), new Vector3(-50, -90, 0), point1_angles.ToList());
        Point point2 = new Point(new Vector3(-1.2f, 0.9f, 1f), new Vector3(-50, -90, 0), point2_angles.ToList());
        Point point3 = new Point(new Vector3(-0.2f, 1.2f, 1.6f), new Vector3(-30, -30, -30), point3_angles.ToList());

        program.AddPoint(point1);
        program.AddPoint(point2);
        program.AddPoint(point3);


        MoveInstruction test0 = ScriptableObject.CreateInstance<MoveInstruction>();
        test0.movementType = InstructionMovementType.LINEAR;
        test0.ApproximationAmount = 0f;
        test0.pointNumber = 0;
        test0.Speed = 0.5f;

        program.Instructions.Add(test0);


        DigitalInInstruction test1 = ScriptableObject.CreateInstance<DigitalInInstruction>();
        test1.bitIndex = 0;
        test1.targetState = true;

        program.Instructions.Add(test1);


        DelayInstruction test2 = ScriptableObject.CreateInstance<DelayInstruction>();
        test2.delayTime = 1f;

        program.Instructions.Add(test2);


        MoveInstruction test3 = ScriptableObject.CreateInstance<MoveInstruction>();
        test3.movementType = InstructionMovementType.JOINT;
        test3.ApproximationAmount = 0f;
        test3.pointNumber = 1;
        test3.Speed = 0.5f;

        program.Instructions.Add(test3);


        DigitalInInstruction test4 = ScriptableObject.CreateInstance<DigitalInInstruction>();
        test4.bitIndex = 1;
        test4.targetState = true;

        program.Instructions.Add(test4);


        DelayInstruction test5 = ScriptableObject.CreateInstance<DelayInstruction>();
        test5.delayTime = 1f;

        program.Instructions.Add(test5);


        MoveInstruction test6 = ScriptableObject.CreateInstance<MoveInstruction>();
        test6.movementType = InstructionMovementType.LINEAR;
        test6.ApproximationAmount = 0f;
        test6.pointNumber = 2;
        test6.Speed = 0.5f;

        program.Instructions.Add(test6);


        DelayInstruction test7 = ScriptableObject.CreateInstance<DelayInstruction>();
        test7.delayTime = 1f;


        IfBlockInstruction test8 = InstructionFactory.CreateInstruction<IfBlockInstruction>();
        test8.SetBlockType(IfBlockInstructionType.IF);
        test8.DigitalInIndex = 1;
        test8.IsComparingAsEqual = false;
        test8.ComparisonValue = false;

        program.Instructions.Add(test8);

        DelayInstruction test8_1 = ScriptableObject.CreateInstance<DelayInstruction>();
        test8_1.delayTime = 15f;
        test8_1.isCommented = true;

        program.Instructions.Add(test8_1);

        EmptyInstruction test8_2 = InstructionFactory.CreateInstruction<EmptyInstruction>();
        program.Instructions.Add(test8_2);


        IfBlockInstruction test10 = InstructionFactory.CreateInstruction<IfBlockInstruction>();
        test10.SetBlockType(IfBlockInstructionType.ELSE_IF);
        test10.DigitalInIndex = 0;
        test10.IsComparingAsEqual = true;
        test10.ComparisonValue = true;

        program.Instructions.Add(test10);


        DelayInstruction test9_1 = ScriptableObject.CreateInstance<DelayInstruction>();
        test9_1.delayTime = 10f;

        program.Instructions.Add(test9_1);

        IfBlockInstruction test11 = InstructionFactory.CreateInstruction<IfBlockInstruction>();
        test11.SetBlockType(IfBlockInstructionType.END_IF);

        program.Instructions.Add(test11);

        // programController.AddProgram(program);
    }

    private void Start()
    {
        robotController = ControllersManager.Instance.GetController<RobotController>();
        StartCoroutine(RunDelayed());
    }

    /// <summary>
    /// Couroutine loading and running program onto the robot
    /// </summary>
    /// <returns>Handle to coroutine</returns>
    private IEnumerator RunDelayed()
    {
        yield return new WaitForSeconds(1f);
        programController.EditProgram(ref program);
        robotController.LoadProgram(program);
        if (runProgram)
            programController.RunProgram(program);
        pendantController.ChangeInstructionIndex(false);
    }
}
