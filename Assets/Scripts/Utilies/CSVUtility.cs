using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Allows for creation of CSV files containing data of tests
/// </summary>
public static class CSVUtility
{
    /// <summary>
    /// Saves CSV file with data do specified location
    /// </summary>
    /// <param name="filePath">Path of file to save</param>
    /// <param name="testData">Data to place in file</param>
    public static void SaveCSV(string filePath, TestData testData)
    {
        if (testData.customId == null)
        {
            testData.customId = File.ReadAllLines(filePath).Length - 1;
            AppendCSVData(filePath, testData);
        }
        else
        {
            ReplaceCSVLine(filePath, testData);
        }

    }

    private static void ReplaceCSVLine(string filePath, TestData testData)
    {
        var lines = File.ReadAllLines(filePath).ToList();
        var lookUpIndex = lines.FindIndex((x) =>
        {
            Debug.Log(x);
            var split = x.Split(';');
            try
            {
                Int32.TryParse(split[0], out int outId);
                return outId == testData.customId;
            }
            catch
            {
                return false;
            }
        });
        Debug.Log(lookUpIndex);
        lines[lookUpIndex] = GetParsedData(testData);
        File.WriteAllLines(filePath, lines);

    }

    private static void AppendCSVData(string filePath, TestData testData)
    {
        StreamWriter outStream = File.AppendText(filePath);
        string value = GetParsedData(testData);
        outStream.WriteLine(value);
        outStream.Close();
    }

    private static string GetParsedData(TestData testData)
    {
        string value = String.Empty;
        value += testData.customId.ToString() + ";" + testData.Name + ";";
        Debug.Log(testData.TaskPoints.Count);
        foreach (var taskPoint in testData.TaskPoints)
        {
            value += taskPoint.Value.Item2.ToString() + ";" + taskPoint.Value.Item1.ToString() + ";";
        }
        value += testData.TotalPoints.ToString() + ";" + testData.TotalTime.ToString() + ";" + testData.Errors.Count.ToString() + ";" + DateTime.Now.ToString() + ";";
        return value;
    }

    public static int GetLastSavedId(string filePath)
    {
        return File.ReadAllLines(filePath).Length - 1;
    }
}
