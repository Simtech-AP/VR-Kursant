using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movement type of a movement instruction
/// </summary>
public enum InstructionMovementType
{
    LINEAR,
    JOINT
}

/// <summary>
/// Class representing a point in space for robot movement
/// </summary>
[System.Serializable]
public class Point : IEquatable<Point>
{
    /// <summary>
    /// Position of point
    /// </summary>
    public Vector3 position = Vector3.zero;
    /// <summary>
    /// Rotation of point
    /// </summary>
    public Vector3 rotation = Vector3.zero;

    public List<Quaternion> jointAngles = new List<Quaternion>();

    public Point()
    {
        position = Vector3.zero;
        rotation = Vector3.zero;
        jointAngles = new List<Quaternion>();
    }

    public Point(Vector3 position, Vector3 rotation)
    {
        this.position = position;
        this.rotation = rotation;
        // TODO rework and reenable decoy mechanism for finding proper rotations in case this was needed
        jointAngles = getDefaultRotationList();
    }

    public Point(Vector3 position, Vector3 rotation, List<Quaternion> angles)
    {
        this.position = position;
        this.rotation = rotation;
        this.jointAngles = angles;
    }

    public Point(Target target)
    {
        this.position = target.position;
        this.rotation = target.rotation;
        this.jointAngles = target.jointAngles;
    }

    private List<Quaternion> getDefaultRotationList()
    {
        var rotations = new List<Quaternion>();
        for (int i = 0; i < 6; i++)
        {
            rotations.Add(Quaternion.identity);
        }

        return rotations;
    }

    public bool Equals(Point other)
    {
        Debug.Log(this.position + " " + other.position + " ::: " + this.rotation + " " + other.rotation + " " + (this.position == other.position && this.rotation == other.rotation).ToString());
        return this.position == other.position && this.rotation == other.rotation;
    }

    public override int GetHashCode()
    {
        int hash = (int)((this.position.x + this.position.y + this.position.z + this.rotation.x + this.position.y + this.position.z) * 1000);
        Debug.Log("hash: " + hash);
        return hash;
    }
}

[CreateAssetMenu(menuName = "Instructions/MoveInstruction")]
public class MoveInstruction : Instruction
{
    /// <summary>
    /// Movement type of an instruction
    /// </summary>
    public InstructionMovementType movementType;

    private float maxInstructionSpeed = 2000f;

    /// <summary>
    /// Speed used in movement
    /// </summary>
    private float speed;
    /// <summary>
    /// Approximation amount used in movement
    /// </summary>

    public float Speed
    {
        set
        {
            speed = value * maxInstructionSpeed;
        }
        get
        {
            return speed / maxInstructionSpeed;
        }
    }

    private float approximationAmount = 0f;

    public float ApproximationAmount
    {
        set
        {
            approximationAmount = Mathf.Clamp(value, 0, 100);
        }
        get
        {
            return approximationAmount;
        }
    }

    /// <summary>
    /// Index of a point used in movement
    /// </summary>
    public int pointNumber;

    /// <summary>
    /// Default contructor
    /// Set standard info like type of instruction etc.
    /// </summary>
    public MoveInstruction()
    {
        type = InstructionType.MOVE;
        maxSelectablePartIndex = 4;
    }

    public MoveInstruction(MoveInstruction other) : base(other)
    {
        this.movementType = other.movementType;
        this.maxInstructionSpeed = other.maxInstructionSpeed;
        this.speed = other.speed;
        this.approximationAmount = other.approximationAmount;
        this.pointNumber = other.pointNumber;
    }

    protected override string generateInstructionSpecificText()
    {
        var _text = "";
        if (movementType == InstructionMovementType.JOINT)
        {
            _text += "J  P[" + pointNumber + "] " + "SPD " + GetDisplaySpeedValue().ToString("F0") + "% ";
            if (speed.ToString().Length == 2)
                _text += " ";
        }
        else
        {
            _text += "L  P[" + pointNumber + "] " + "SPD " + speed.ToString("F0") + "mm/sec ";
            if (speed.ToString().Length == 3)
                _text += " ";
        }

        _text += "APX " + approximationAmount.ToString();

        return _text;
    }

    public override void ProcessInput(string input)
    {
        if (isPartEditedFirstTime)
        {
            isPartEditedFirstTime = false;
            if (char.IsDigit(input[0]))
            {
                if (selectedPart == 2)
                {
                    var pointsCount = PendantData.Instance.EditedProgram.SavedPoints.Count;
                    pointNumber = (int)Mathf.Clamp(int.Parse(input), 0, pointsCount - 1);
                }
                else if (selectedPart == 3)
                {
                    if (movementType == InstructionMovementType.JOINT)
                    {
                        speed = int.Parse(input) * (maxInstructionSpeed / 100);
                    }
                    else
                    {
                        speed = int.Parse(input);
                    }
                }
                else if (selectedPart == 4)
                {
                    approximationAmount = int.Parse(input);
                }
            }
        }
        else
        {
            if (char.IsDigit(input[0]))
            {
                if (selectedPart == 2)
                {
                    if (pointNumber < 10)
                    {
                        pointNumber = pointNumber * 10 + int.Parse(input);
                    }
                    var pointsCount = PendantData.Instance.EditedProgram.SavedPoints.Count;
                    pointNumber = (int)Mathf.Clamp(int.Parse(input), 0, pointsCount - 1);
                }
                else if (selectedPart == 3)
                {
                    if (movementType == InstructionMovementType.JOINT)
                    {
                        speed = speed * 10 + int.Parse(input) * (maxInstructionSpeed / 100);
                    }
                    else
                    {
                        speed = speed * 10 + int.Parse(input);
                    }
                    speed = Mathf.Clamp(speed, 0, maxInstructionSpeed);

                }
                else if (selectedPart == 4)
                {
                    approximationAmount = (int)Mathf.Clamp(approximationAmount * 10 + int.Parse(input), 0, 100);
                }
            }
        }

        if (input.Equals("PREV"))
        {
            if (selectedPart == 3)
            {
                if (movementType == InstructionMovementType.JOINT)
                {
                    float percent = speed / maxInstructionSpeed * 100;
                    percent = Mathf.FloorToInt(percent / 10);
                    speed = (percent / 100) * maxInstructionSpeed;
                }
                else
                {

                    speed = Mathf.Floor(speed / 10f);
                }
                Debug.Log(speed);
            }
            else if (selectedPart == 4)
            {
                approximationAmount = Mathf.Floor(approximationAmount / 10f);
            }
        }

        if (input.Equals("Enter"))
        {
            if (selectedPart == 1)
            {
                movementType = (InstructionMovementType)(((int)movementType + 1) % 2);
            }
            selectedPart = 0;
            PendantData.OnUpdatedData?.Invoke(PendantData.Instance);
        }
    }

    protected override Tuple<int, int> selectPart(int part)
    {
        selectedPart = part;
        switch (part)
        {
            case 1:
                int startIndex = instructionText.IndexOf('J', 0);
                if (startIndex == -1)
                    startIndex = instructionText.IndexOf('L', 0);
                return new Tuple<int, int>(startIndex, startIndex + 1);
            case 2:
                startIndex = 0;
                startIndex = instructionText.IndexOf('[', startIndex);
                return new Tuple<int, int>(startIndex + 1, instructionText.IndexOf(']', startIndex));
            case 3:
                startIndex = 0;
                startIndex = instructionText.IndexOf(']', startIndex);
                startIndex += 6;
                int endIndex = startIndex + 1;
                for (int i = startIndex; i < instructionText.Length; i++)
                {
                    if (!char.IsDigit(instructionText[i]))
                    {
                        endIndex = i;
                        break;
                    }
                }
                return new Tuple<int, int>(startIndex, endIndex);
            case 4:
                endIndex = instructionText.Length;
                startIndex = endIndex - 2;
                for (int i = startIndex; i > 0; i--)
                {
                    if (!char.IsLetterOrDigit(instructionText[i]))
                    {
                        startIndex = i;
                        break;
                    }
                }
                return new Tuple<int, int>(startIndex + 1, endIndex);
        }
        return new Tuple<int, int>(0, 5);
    }

    /// <summary>
    /// Sets commented status of instruction
    /// </summary>
    /// <param name="comment">Should the instruction be commented?</param>
    public void SetCommentedStatus(bool comment)
    {
        if (comment)
        {
            isCommented = true;
        }
        else
        {
            isCommented = false;
        }
    }

    public float GetDisplaySpeedValue()
    {
        if (movementType == InstructionMovementType.LINEAR)
        {
            return speed;
        }
        else
        {
            return speed / maxInstructionSpeed * 100;
        }
    }
}
