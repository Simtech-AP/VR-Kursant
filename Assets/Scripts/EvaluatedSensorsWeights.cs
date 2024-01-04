
using System.Collections.Generic;
using UnityEngine;

public class WeightedSensor
{
    public readonly SensorSide sensorSide;
    public readonly SensorSize sensorSize;
    public float weight;

    public WeightedSensor(SensorSide _side, SensorSize _size)
    {
        sensorSide = _side;
        sensorSize = _size;
    }
}

public class EvaluatedSensorsWeights
{

    private List<WeightedSensor> sensorDataCollection = new List<WeightedSensor>();
    private float smallSensorWeight = 1f;
    private float largeSensorWeight = 1f;
    private int smallSensorCutoutValue = 10;
    private int largeSensorCutoutValue = 10;
    private float ignoreValueForSmall = 0.33f;
    private float ignoreValueForLarge = 0.10f;
    private int smallSensorCount = 0;
    private int largeSensorCount = 0;

    public EvaluatedSensorsWeights()
    {
        sensorDataCollection.Add(new WeightedSensor(SensorSide.LEFT, SensorSize.LARGE));
        sensorDataCollection.Add(new WeightedSensor(SensorSide.RIGHT, SensorSize.LARGE));
        sensorDataCollection.Add(new WeightedSensor(SensorSide.LEFT, SensorSize.SMALL));
        sensorDataCollection.Add(new WeightedSensor(SensorSide.RIGHT, SensorSize.SMALL));
    }

    public void ProcessWeight(TouchSensor sensor, int value)
    {
        var sensorData = sensorDataCollection.Find(x => sensor.SensorSide == x.sensorSide && sensor.SensorSize == x.sensorSize);

        sensorData.weight += (sensor.SensorSize == SensorSize.LARGE ? largeSensorWeight : smallSensorWeight) * value;

        if (sensor.SensorSize == SensorSize.SMALL)
        {
            smallSensorCount += 1;
        }
        else
        {
            largeSensorCount += 1;
        }
    }

    private bool isTypeIgnored(SensorSide _side, SensorSize _size)
    {

        var thisTypeData = sensorDataCollection.Find(x => _side == x.sensorSide && _size == x.sensorSize);
        var adjacentTypeData = sensorDataCollection.Find(x => _side == x.sensorSide && _size != x.sensorSize);

        if (_size == SensorSize.SMALL && adjacentTypeData.weight / largeSensorCount > smallSensorCutoutValue) { return true; }
        if (_size == SensorSize.LARGE && thisTypeData.weight / largeSensorCount > largeSensorCutoutValue) { return false; }

        var summarySideWeight = thisTypeData.weight + adjacentTypeData.weight;

        if (summarySideWeight == 0) { return true; }

        var ignoredValue = _size == SensorSize.LARGE ? ignoreValueForLarge : ignoreValueForSmall;
        return thisTypeData.weight / summarySideWeight < ignoredValue;
    }


    public bool ShouldIgnoreSensor(TouchSensor sensor)
    {
        return isTypeIgnored(sensor.SensorSide, sensor.SensorSize);
    }

    public void Clear()
    {
        sensorDataCollection.ForEach(x => x.weight = 0);
        smallSensorCount = 0;
        largeSensorCount = 0;
    }
}