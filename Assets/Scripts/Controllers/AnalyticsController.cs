using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Allows to collect data form application 
/// </summary>
public class AnalyticsController : Controller
{
    /// <summary>
    /// Data container to save
    /// </summary>
    [SerializeField]
    public AnalyticsData data;
    /// <summary>
    /// Template for timer counting time a user spent in app
    /// </summary>
    private float timer;

    /// <summary>
    /// Template method for saving data
    /// </summary>
    public void SaveData()
    {
        StreamWriter streamWriter = new StreamWriter(Application.streamingAssetsPath + "/" + DateTime.Now.ToString("ddd dd MMMM yyyy HH mm ss") + ".csv");
        streamWriter.WriteLine("Medule,Step,Name,Value");
        foreach (var stepTime in data.stepTime)
        {
            streamWriter.WriteLine(String.Format("{0},{1},Step Time,{2}", stepTime.moduleName, stepTime.stepName, stepTime.time));
        }
        streamWriter.WriteLine(String.Format("Module3,Step1,BumperCap,{0}", data.bumperCap));
        streamWriter.WriteLine(String.Format("Module3,Step1,EStop,{0}", data.eStop));
        streamWriter.WriteLine(String.Format("Module3,Step1,Padlock,{0}", data.padlock));
        streamWriter.WriteLine(String.Format("Module3,Step1,Object removed,{0}", data.objectRemoved));
        streamWriter.WriteLine(String.Format("Module3,Step1,Time To EStop,{0}", data.timeToEStop));
        streamWriter.WriteLine(String.Format("Module3,Step1,Time To End,{0}", data.timeToEnd));
        streamWriter.Close();
    }
}
