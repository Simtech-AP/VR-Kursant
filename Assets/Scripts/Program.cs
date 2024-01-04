using System.Collections.Generic;

/// <summary>
/// Class representing program run and edited in application
/// </summary>
public class Program
{
    /// <summary>
    /// Program name
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Program description
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// List of instructions
    /// </summary>
    public List<Instruction> Instructions { get; set; } = new List<Instruction>();
    /// <summary>
    /// List of points saved in a program
    /// </summary>    
    public List<Point> SavedPoints { get; set; } = new List<Point>();

    /// <summary>
    /// Parametrized constructor for creatig new program
    /// </summary>
    public Program(string name, string description, List<Instruction> instructions, List<Point> points)
    {
        Name = name;
        Description = description;
        Instructions = instructions;
        SavedPoints = points;
    }

    public Program(Program other)
    {
        this.Name = other.Name;
        this.Description = other.Description;

        foreach (var instruction in other.Instructions)
        {
            this.Instructions.Add(InstructionFactory.CloneInstruction(instruction));
        }

        SavedPoints.Clear();

        foreach (var point in other.SavedPoints)
        {
            this.SavedPoints.Add(new Point(point.position, point.rotation, point.jointAngles));
        }
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public Program()
    {
        Name = "1";
        Description = " ";
        Instructions = new List<Instruction>();
        SavedPoints = new List<Point>();
    }

    /// <summary>
    /// Adds robot point to program memory
    /// </summary>
    /// <param name="newPoint">Point to add</param>
    public void AddPoint(Point newPoint)
    {
        SavedPoints.Add(newPoint);
    }

    /// <summary>
    /// Gets point from array with specified index
    /// </summary>
    /// <param name="index">Index of point to get</param>
    /// <returns>Point structure of needed point</returns>
    public Point GetPoint(int index)
    {
        return SavedPoints[index];
    }

    /// <summary>
    /// Allows changing saved points to new position and rotation
    /// </summary>
    /// <param name="index">Index of a point to change</param>
    /// <param name="newPoint">Values of point to set</param>
    public void ChangePoint(int index, Point newPoint)
    {
        SavedPoints[index] = newPoint;
    }
}