using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;

/// <summary>
/// Class for holding and maintaining data of pendant
/// </summary>
public class PendantData : Singleton<PendantData>
{
    /// <summary>
    /// Instruction index of currently edited program 
    /// </summary>
    private int currentInstructionIndex = default;
    /// <summary>
    /// Reference to currently edited program
    /// </summary>
    private Program editedProgam = default;
    private List<Program> savedPrograms = new List<Program>();

    /// <summary>
    /// Action allowing to update data for objects
    /// </summary>
    public static Action<PendantData> OnUpdatedData;

    /// <summary>
    /// Public accessor for currently selected insruction index
    /// </summary>
    public int CurrentInstructionIndex
    {
        get => currentInstructionIndex;
        set
        {
            currentInstructionIndex = value;
            OnUpdatedData?.Invoke(this);
        }
    }

    public Instruction CurrentInstruction
    {
        get => editedProgam.Instructions[currentInstructionIndex];
    }

    /// <summary>
    /// Public accessor for reference to edited program
    /// </summary>
    public Program EditedProgram
    {
        get => editedProgam;
        set
        {
            editedProgam = value;
            OnUpdatedData?.Invoke(this);
        }
    }

    public List<Program> SavedPrograms
    {
        get => savedPrograms;
        set
        {
            savedPrograms = value;
            OnUpdatedData?.Invoke(this);
        }
    }

    private void Awake()
    {
        base.Awake();
        savedPrograms = new List<Program>();
        loadProgramsFromFile();
    }

    private void OnDestroy()
    {
        saveProgramsToFile();
    }

    private void loadProgramsFromFile()
    {
        var filePath = Application.dataPath + "/SaveData/Programms/savedPrograms.json";
        string serializedPrograms = File.ReadAllText(filePath);
        JsonConverter[] converters = { new InstructionConverter(), new QuaternionConverter() };
        savedPrograms = JsonConvert.DeserializeObject<List<Program>>(
            serializedPrograms,
            new JsonSerializerSettings
            {
                Converters = converters
            });
    }

    private void saveProgramsToFile()
    {
        var filePath = Application.dataPath + "/SaveData/Programms/savedPrograms.json";
        JsonConverter[] converters = { new InstructionConverter(), new QuaternionConverter() };
        string serializedPrograms = JsonConvert.SerializeObject(savedPrograms, new JsonSerializerSettings
        {
            Converters = converters
        });
        File.WriteAllText(filePath, serializedPrograms);
    }
}
