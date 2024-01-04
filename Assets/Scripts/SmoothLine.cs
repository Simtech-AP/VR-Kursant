using UnityEngine;

/// <summary>
/// Allows to create a smooth line between 2 points with single point inbetween
/// </summary>
public class SmoothLine : MonoBehaviour
{
    /// <summary>
    /// Number of points for line renderer
    /// </summary>
    [SerializeField]
    private int numberOfPoints = 10;
    /// <summary>
    /// Starting point of a line
    /// </summary>
    [SerializeField]
    private Transform startPoint = default;
    /// <summary>
    /// Ending point of a line
    /// </summary>
    [SerializeField]
    private Transform endPoint = default;
    /// <summary>
    /// Middle point of a line, defines curve
    /// </summary>
    [SerializeField]
    private Transform middlePoint = default;

    /// <summary>
    /// Line renderer object to draw a line
    /// </summary>
    [SerializeField]
    private LineRenderer line = default;
    /// <summary>
    /// Reference to last position of this gameobject
    /// </summary>
    private Vector3 lastPosition;

    /// <summary>
    /// Create specified number of points at the start and set it for line renderer
    /// </summary>
    private void Start()
    {
        line.positionCount = numberOfPoints;
        Vector3[] points = new Vector3[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            points[i] = GetPoint(startPoint.position, middlePoint.position, endPoint.position, (float)i / numberOfPoints);
            if (i == numberOfPoints - 1)
            {
                points[i] = GetPoint(startPoint.position, middlePoint.position, endPoint.position, 1);
            }
        }
        line.SetPositions(points);
    }

    /// <summary>
    /// Every specified time recalculate line 
    /// </summary>
    private void FixedUpdate()
    {
        // if (lastPosition != transform.position)
        // {
        Vector3[] points = new Vector3[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            points[i] = GetPoint(startPoint.position, middlePoint.position, endPoint.position, (float)i / numberOfPoints);
            if (i == numberOfPoints - 1)
            {
                points[i] = GetPoint(startPoint.position, middlePoint.position, endPoint.position, 1);
            }
        }
        line.SetPositions(points);
        lastPosition = transform.position;
        // }
    }

    /// <summary>
    /// Gets point between two points and interpolates it using third, middle point
    /// </summary>
    /// <param name="p0">Start point</param>
    /// <param name="p1">Middle point</param>
    /// <param name="p2">Ending point</param>
    /// <param name="t">Point on the resulting line in 0.0-1.0 value</param>
    /// <returns>Position of point in specified lerped value</returns>
    private Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
    }

    /// <summary>
    /// Visualization of line available in Editor
    /// </summary>
    [ContextMenu("Show Line")]
    private void ShowLine()
    {
        Start();
    }
}
