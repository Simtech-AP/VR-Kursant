using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data calss allowing to store collected analytics data
/// </summary>
[CreateAssetMenu(menuName = "AnalyticsData")]
public class AnalyticsData : ScriptableObject
{
    /// <summary>
    /// Example bool to check if user is in training
    /// </summary>
    public bool isTraining;
    /// <summary>
    /// Example float time to store total time a user has spent in app
    /// </summary>
    public float totalTime;
    /// <summary>
    /// Example list that can be used to store time spent in every module
    /// </summary>
    public List<float> moduleTime;
    /// <summary>
    /// list of steps time
    /// </summary>
    public List<StepTime> stepTime = new List<StepTime>();
    /// <summary>
    /// is bumpercap taken during exam
    /// </summary>
    public bool bumperCap;
    /// <summary>
    /// is padlock on door during exam
    /// </summary>
    public bool padlock;
    /// <summary>
    /// is estop pressed during exam
    /// </summary>
    public bool eStop;
    /// <summary>
    /// is object removed during exam
    /// </summary>
    public bool objectRemoved;
    /// <summary>
    /// time to press estop
    /// </summary>
    public float timeToEStop;
    /// <summary>
    /// time of the exam
    /// </summary>
    public float timeToEnd;
    /// <summary>
    /// Structure to hold times for modules and steps
    /// </summary>
    public struct StepTime
    {
        /// <summary>
        /// Time that step took to finish
        /// </summary>
        public float time;
        /// <summary>
        /// Name of module step was in
        /// </summary>
        public string moduleName;
        /// <summary>
        /// Step name
        /// </summary>
        public string stepName;
    }
}
