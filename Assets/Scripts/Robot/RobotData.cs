using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Type of movement of a robot
/// </summary>
public enum MovementType
{
    Base,
    Tool,
    User,
    Joint
}

/// <summary>
/// Direction of a movement of a robot
/// </summary>
public enum MovementDirection
{
    Forward,
    Backward,
    Up,
    Down,
    Right,
    Left
}

/// <summary>
/// Movement mode of a robot
/// </summary>
public enum MovementMode
{
    AUTO = 0,
    T1 = 1,
    T2 = 2
}

public enum StepMode
{
    STEP,
    NORMAL
}

public class RobotData : Singleton<RobotData>
{
    /// <summary>
    /// Type of movement of a robot
    /// </summary>
    private MovementType movementType = default;
    /// <summary>
    /// Movement mode of robot
    /// </summary>
    private MovementMode movementMode = default;
    private StepMode stepMode = StepMode.NORMAL;
    /// <summary>
    /// Current speed of a robot
    /// </summary>
    private float robotSpeed = 0.50f;
    /// <summary>
    /// Maximum movement speed of robot
    /// </summary>
    private float maxRobotSpeed = 2000f;
    /// <summary>
    /// Maximum rotation speed of robot
    /// </summary>
    private float maxRobotJointSpeed = 10f;
    /// <summary>
    /// Currently loaded program in robot
    /// </summary>
    private Program loadedProgram = new Program();
    /// <summary>
    /// Is robot currently runnning a program?
    /// </summary>
    private bool isRunning = false;
    /// <summary>
    /// Home point of a robot
    /// </summary>
    private Point homePoint = new Point();
    /// <summary>
    /// Current point of 6th pivot
    /// </summary>
    private Point currentPoint = new Point();
    /// <summary>
    /// Index of curently running instruction in program
    /// </summary>
    private int currentRunningInstructionIndex = 0;
    /// <summary>
    /// TCP point list for tool movement calculation
    /// </summary>
    private Tuple<Vector3, Vector3>[] tcpPoints = new Tuple<Vector3, Vector3>[3]
    {
        new Tuple<Vector3, Vector3>(Vector3.zero, Vector3.zero),
        new Tuple<Vector3, Vector3>(Vector3.zero, Vector3.zero),
        new Tuple<Vector3, Vector3>(Vector3.zero, Vector3.zero)
    };
    /// <summary>
    /// Current rotations of axes
    /// </summary>
    private Angles currentAngles = new Angles(new List<float>() { 0, 0, 0, 0, 0, 0 });
    /// <summary>
    /// Target we are currently going to
    /// </summary>
    private Target currentTarget = new Target(Vector3.zero, Vector3.zero, new List<Quaternion>());

    /// <summary>
    /// Is the robot configured for joint or linear movement
    /// </summary>
    private bool isInJointConfiguration = default;
    /// <summary>
    /// Event run when anything in this object changes
    /// </summary>
    public static Action<RobotData> OnUpdatedData;
    /// <summary>
    /// Have we creted any TCP points?
    /// </summary>
    public Func<bool> IsTCPCollected;

    /// <summary>
    /// Checks if we collected any TCP points
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        IsTCPCollected = new Func<bool>(() =>
        {
            foreach (var elem in tcpPoints)
                if (elem.Item1 == Vector3.zero)
                    return false;
            return true;
        });
    }

    /// <summary>
    /// Public accessor for movement tye
    /// </summary>
    public MovementType MovementType
    {
        get => movementType;
        set
        {
            movementType = value;
            OnUpdatedData?.Invoke(this);
        }
    }

    /// <summary>
    /// Public accessor for movement mode
    /// </summary>
    public MovementMode MovementMode
    {
        get => movementMode;
        set
        {
            movementMode = value;
            OnUpdatedData?.Invoke(this);
        }
    }

    /// <summary>
    /// Public accessor for robot speed
    /// </summary>
    public float RobotSpeed
    {
        get => robotSpeed;
        set
        {
            robotSpeed = Mathf.Clamp(value, 0f, 1f);
            OnUpdatedData?.Invoke(this);
        }
    }

    /// <summary>
    /// Public accessor for maximum robot speed
    /// </summary>
    public float MaxRobotSpeed
    {
        get => maxRobotSpeed;
        set
        {
            maxRobotSpeed = value;
            OnUpdatedData?.Invoke(this);
        }
    }

    /// <summary>
    /// Public accessor for running program flag
    /// </summary>
    public bool IsRunning
    {
        get => isRunning;
        set
        {
            isRunning = value;
            OnUpdatedData?.Invoke(this);
        }
    }

    /// <summary>
    /// Public accessor for robot home point
    /// </summary>
    public Point HomePoint
    {
        get => homePoint;
        set
        {
            homePoint = value;
            OnUpdatedData?.Invoke(this);
        }
    }

    /// <summary>
    /// Public accessor for program loaded in robot memory
    /// </summary>
    public Program LoadedProgram
    {
        get => loadedProgram;
        set
        {
            loadedProgram = value;
            // currentRunningInstructionIndex = 0;
            OnUpdatedData?.Invoke(this);
        }
    }

    /// <summary>
    /// Public accessor for maximum robot rotation speed
    /// </summary>
    public float MaxRobotJointSpeed
    {
        get => maxRobotJointSpeed; set
        {
            maxRobotJointSpeed = value;
            OnUpdatedData?.Invoke(this);
        }
    }

    /// <summary>
    /// Public accessor for currently running instruction index
    /// </summary>
    public int CurrentRunningInstructionIndex
    {
        get => currentRunningInstructionIndex;
        set
        {
            currentRunningInstructionIndex = value;
            OnUpdatedData?.Invoke(this);
        }
    }

    public Instruction CurrentRunningInstruction
    {
        get => LoadedProgram.Instructions[CurrentRunningInstructionIndex];
    }

    /// <summary>
    /// Public accessor for TCP points list
    /// </summary>
    public Tuple<Vector3, Vector3>[] TCPPoint
    {
        get => tcpPoints;
        set
        {
            tcpPoints = value;
            OnUpdatedData?.Invoke(this);
        }
    }

    /// <summary>
    /// Public accessor for current axes rotations
    /// </summary>
    public Angles CurrentAngles
    {
        get => currentAngles;
        set
        {
            currentAngles = value;
            OnUpdatedData?.Invoke(this);
        }
    }

    /// <summary>
    /// Public accessor for current robot target
    /// </summary>
    public Target CurrentTarget
    {
        get => currentTarget;
        set
        {
            currentTarget = value;
            OnUpdatedData?.Invoke(this);
        }
    }


    public StepMode StepMode
    {
        get => stepMode;
        set
        {
            stepMode = value;
            OnUpdatedData?.Invoke(this);
        }
    }


    public bool IsInJointConfiguration
    {
        get;
        set;
    }
}
