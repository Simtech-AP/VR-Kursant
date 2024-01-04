using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class EvaluatedSensor
{
    public int sesorId;
    public int sensorValue;
    public bool ignored;
}


/// <summary>
/// Class allowing for visual representation of sensors in application
/// </summary>
public class TouchSensors : MonoBehaviour
{
    private EvaluatedSensorsWeights evaluatedSensorsWeights = new EvaluatedSensorsWeights();
    private List<string> outLogs = new List<string>();

    /// <summary>
    /// List of currently touched sensors
    /// </summary>
    private List<int> touchedSensors = new List<int>();

    private List<EvaluatedSensor> touchedSensorsValues = new List<EvaluatedSensor>();
    [SerializeField] private Transform sensorsParent = default;
    /// <summary>
    /// List of all of the transform visuals in pendant object
    /// </summary>
    [SerializeField]
    private List<TouchSensor> sensors = new List<TouchSensor>();
    /// <summary>
    /// Reference to right hand on a pendant
    /// </summary>
    [SerializeField]
    private GameObject rightHand = default;
    /// <summary>
    /// Reference to left hand on a pendant
    /// </summary>
    [SerializeField]
    private GameObject leftHand = default;
    /// <summary>
    /// Right hand used when not touching any sensors
    /// </summary>
    [SerializeField]
    private GameObject defaultRightHand = default;
    /// <summary>
    /// Left hand used when not touching any sensors
    /// </summary>
    [SerializeField]
    private GameObject defaultLeftHand = default;
    /// <summary>
    /// Is the pendant using older, group-based implementation?
    /// </summary>
    public bool groupPendant = false;
    /// <summary>
    /// Variables for representing approxiated position of right hand on pendant
    /// </summary>
    private Vector3 handPositionRight = Vector3.zero;
    /// <summary>
    /// Variables for representing approxiated position of left hand on pendant
    /// </summary>
    private Vector3 handPositionLeft = Vector3.zero;

    private Tween rightHandMovement;
    private Tween leftHandMovement;

    private void Awake()
    {
        // sensors.Clear();
        // foreach (Transform sensor in sensorsParent)
        // {
        //     sensors.Add(sensor.GetComponent<TouchSensor>());
        // }
        CollectAndSortSensors();
    }

    public void ChangeSensorState(byte[] data)
    {
        touchedSensorsValues.Clear();

        var sensorNumbers = data.ToList().GetRange(1, 10);
        var sensorValues = data.ToList().GetRange(11, 5);


        for (int i = 0; i < sensorNumbers.Count; ++i)
        {
            var sensorId = (int)sensorNumbers[i];

            if (sensorId != 120)
            {
                var valueByteId = Mathf.FloorToInt(i / 2);
                var isFirstInPair = i % 2 == 0;

                var sensorValue = Convert.ToInt32(sensorValues[valueByteId]);
                var bits = Convert.ToString(sensorValue, 2);
                bits = bits.Insert(0, String.Concat(Enumerable.Repeat("0", (8 - bits.Length))));

                if (isFirstInPair)
                {
                    var value = Convert.ToInt32(bits.Substring(0, 4), 2);
                    touchedSensorsValues.Add(new EvaluatedSensor { sesorId = sensorId, sensorValue = value });
                }
                else
                {
                    var value = Convert.ToInt32(bits.Substring(4, 4), 2);
                    touchedSensorsValues.Add(new EvaluatedSensor { sesorId = sensorId, sensorValue = value });
                }
            }
        }

        string s = string.Empty;
        touchedSensorsValues.ForEach(x => { s += (";" + x.sensorValue); });
        // Debug.Log(string.Join(";", sensorNumbers) + " ||| " + s);

        UpdateSensorsVisualization();
    }



    public void UpdateSensorsVisualization()
    {
        int activeSensorsNumberRight = 0;
        int activeSensorsNumberLeft = 0;

        var rightHandUnscaledPos = Vector3.zero;
        var leftHandUnscaledPos = Vector3.zero;

        string outLog = string.Empty;
        touchedSensorsValues.ForEach(x => outLog += x.sesorId + " " + x.sensorValue + "; ");

        //
        // if (outLog != string.Empty)
        // {
        outLogs.Add(outLog);
        // }
        //
        // Debug.Log(outLog);


        evaluatedSensorsWeights.Clear();

        if (touchedSensorsValues.Count > 0)
        {
            foreach (var evaluatedSensor in touchedSensorsValues)
            {
                var sensor = sensors.Find(x => x.SensorId == evaluatedSensor.sesorId);

                evaluatedSensorsWeights.ProcessWeight(sensor, evaluatedSensor.sensorValue);
            }

            for (int i = 0; i < touchedSensorsValues.Count; ++i)
            {
                var sensor = sensors.Find(x => x.SensorId == touchedSensorsValues[i].sesorId);

                if (evaluatedSensorsWeights.ShouldIgnoreSensor(sensor))
                {
                    touchedSensorsValues[i].ignored = true;
                    continue;
                }

                if (sensor.SensorSide == SensorSide.RIGHT)
                {
                    if (activeSensorsNumberRight == 0)
                        rightHandUnscaledPos = Vector3.zero;
                    rightHandUnscaledPos += sensor.GetUnscaledLocalPosition();
                    activeSensorsNumberRight++;
                }
                else
                {
                    if (activeSensorsNumberLeft == 0)
                        leftHandUnscaledPos = Vector3.zero;
                    leftHandUnscaledPos += sensor.GetUnscaledLocalPosition();
                    activeSensorsNumberLeft++;
                }
            }
        }

        if (rightHandUnscaledPos != Vector3.zero && activeSensorsNumberRight > 0)
            rightHandUnscaledPos /= activeSensorsNumberRight;
        if (leftHandUnscaledPos != Vector3.zero && activeSensorsNumberLeft > 0)
            leftHandUnscaledPos /= activeSensorsNumberLeft;

        var rightHandScaledPos = ApplyScalesToHandPosition(rightHandUnscaledPos, SensorSide.RIGHT);
        var leftHandScaledPos = ApplyScalesToHandPosition(leftHandUnscaledPos, SensorSide.LEFT);

        handPositionRight = rightHandScaledPos;
        handPositionLeft = leftHandScaledPos;


        ProcessHandsEnableState(activeSensorsNumberRight, activeSensorsNumberLeft);
    }

    private void OnDestroy()
    {
        string docPath =
         Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "WriteLines.txt")))
        {
            foreach (var item in outLogs)
            {
                outputFile.WriteLine(item);
            }
        }
    }

    private Vector3 ApplyScalesToHandPosition(Vector3 unscaledCenter, SensorSide side)
    {
        var center = unscaledCenter;

        foreach (var evs in touchedSensorsValues)
        {
            if (evs.ignored) { continue; }

            var sensor = sensors.Find(x => x.SensorId == evs.sesorId);

            if (sensor.SensorSide == side)
            {
                center += (unscaledCenter - sensor.GetUnscaledLocalPosition()) * (1 - sensor.GetMultiplier(evs.sensorValue));
            }
        }

        return center;
    }

    private void ProcessHandsEnableState(int activeSensorsNumberRight, int activeSensorsNumberLeft)
    {
        if (activeSensorsNumberRight <= 0)
        {
            rightHand.SetActive(false);
            defaultRightHand.SetActive(true);
        }
        else if (!rightHand.activeSelf)
        {
            rightHand.SetActive(true);
            defaultRightHand.SetActive(false);
        }
        if (activeSensorsNumberLeft <= 0)
        {
            leftHand.SetActive(false);
            defaultLeftHand.SetActive(true);
        }
        else if (!leftHand.activeSelf)
        {
            leftHand.SetActive(true);
            defaultLeftHand.SetActive(false);
        }

        rightHandMovement.Kill();
        leftHandMovement.Kill();

        if (rightHand.transform.localPosition != Vector3.zero)
        {
            rightHandMovement = rightHand.transform.DOLocalMove(handPositionRight, 0.1f);
        }
        else
        {
            rightHand.transform.localPosition = handPositionRight;
        }

        if (leftHand.transform.localPosition != Vector3.zero)
        {
            leftHandMovement = leftHand.transform.DOLocalMove(handPositionLeft, 0.1f);
        }
        else
        {
            leftHand.transform.localPosition = handPositionLeft;
        }


        if (activeSensorsNumberRight <= 0 && rightHand.activeSelf)
        {
            rightHand.SetActive(false);
            defaultRightHand.SetActive(true);
        }
        if (activeSensorsNumberLeft <= 0 && leftHand.activeSelf)
        {
            leftHand.SetActive(false);
            defaultLeftHand.SetActive(true);
        }
    }

    /// <summary>
    /// Gets all sensors and orders them by name
    /// </summary>
    [ContextMenu("Find and sort sensors")]
    private void CollectAndSortSensors()
    {
        sensors.Clear();
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (int.TryParse(t.name, out _))
            {
                sensors.Add(t.GetComponent<TouchSensor>());
            }
        }
        sensors = sensors.OrderBy(t => t.SensorId).ToList();
    }
}
