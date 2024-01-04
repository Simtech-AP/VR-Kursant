using UnityEngine;

public enum SensorSize
{
    SMALL,
    LARGE
}

public enum SensorSide
{
    LEFT,
    RIGHT
}

public class TouchSensor : MonoBehaviour
{
    [SerializeField] private int sensorId;
    public int SensorId { private set { sensorId = value; } get { return sensorId; } }
    [SerializeField] private SensorSize sensorSize;
    public SensorSize SensorSize { private set { sensorSize = value; } get { return sensorSize; } }
    [SerializeField] private SensorSide sensorSide;
    public SensorSide SensorSide { private set { sensorSide = value; } get { return sensorSide; } }
    [SerializeField] private Transform sensorTransform;
    public static float smallSensorModifier = 0.0666666f;
    public static float largeSensorModifier = 0.0666666f;
    public static int lowerCutoutValueSmall = 2;
    public static int lowerCutoutValueLarge = 2;
    public static int upperCutOutValueSmall = 15;
    public static int upperCutOutValueLarge = 14;

    public Vector3 GetUnscaledLocalPosition()
    {
        return sensorTransform.localPosition;
    }

    public float GetMultiplier(int sensorValue)
    {

        var lowerCutout = sensorSize == SensorSize.LARGE ? lowerCutoutValueLarge : lowerCutoutValueSmall;
        var upperCutout = sensorSize == SensorSize.LARGE ? upperCutOutValueLarge : upperCutOutValueSmall;
        var sensitivityModifier = sensorSize == SensorSize.LARGE ? largeSensorModifier : smallSensorModifier;


        float multiplier = 0f;

        if (sensorValue < lowerCutout)
        {
            multiplier = 0f;
        }
        else if (sensorValue > upperCutout)
        {
            multiplier = 1f;
        }
        else
        {
            multiplier = (float)sensorValue * sensitivityModifier;
        }

        return multiplier;
    }
}