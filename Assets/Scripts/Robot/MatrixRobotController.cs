using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

#pragma warning disable CS0219
/// <summary>
/// Robot target structure
/// </summary>
[System.Serializable]
public struct Target
{
    public Vector3 position;
    public Vector3 rotation;
    public List<Quaternion> jointAngles;

    public Target(Vector3 position, Vector3 rotation, List<Quaternion> jointAngles)
    {
        this.position = position;
        this.rotation = rotation;
        this.jointAngles = jointAngles;
    }
}

/// <summary>
/// Robot angles structure used in controlling joints
/// </summary>
[System.Serializable]
public struct Angles
{
    public List<float> angles;

    public Angles(List<float> angles)
    {
        if (angles == null)
        {
            this.angles = new List<float>() { 0, 0, 0, 0, 0, 0 };
        }
        else
        {
            this.angles = angles;
        }
    }

    public static bool operator ==(Angles a1, Angles a2)
    {
        if (a1.angles.Count != a2.angles.Count) return false;
        for (int i = 0; i < a1.angles.Count; i++)
        {
            if (a1.angles[i] != a2.angles[i]) return false;
        }
        return true;
    }

    public static bool operator !=(Angles a1, Angles a2)
    {
        if (a1.angles.Count != a2.angles.Count) return true;
        for (int i = 0; i < a1.angles.Count; i++)
        {
            if (a1.angles[i] != a2.angles[i]) return true;
        }
        return false;
    }
}

/// <summary>
/// Enum used in limiting joint movements
/// </summary>
[System.Serializable]
public enum JointConfiguration
{
    Plus,
    Minus
}

/// <summary>
/// Class allowing for controlling robot using matrix transformations
/// </summary>
public class MatrixRobotController : MonoBehaviour
{
    /// <summary>
    /// Current target to which robot will go to
    /// </summary>
    public Target currentTarget;
    /// <summary>
    /// Matrix derived from target position and rotation
    /// </summary>
    public Matrix4x4 targetMatrix;
    /// <summary>
    /// Joint configuration for first joint
    /// </summary>
    public JointConfiguration joint1;
    /// <summary>
    /// Joint configuration for third joint
    /// </summary>
    public JointConfiguration joint3;
    /// <summary>
    /// Joint configuration for fifth joint
    /// </summary>
    public JointConfiguration joint5;
    /// <summary>
    /// Lambda specifiers for matrix 
    /// </summary>
    public float lambda1 = 0.475f, lambda4 = 0.720f, lambda6 = 0.085f;
    /// <summary>
    /// Length specifiers for matrix
    /// </summary>
    public float li1 = 0.15f, li2 = 0.6f, li3 = 0.12f;
    /// <summary>
    /// Variable for counting radius of workspace
    /// </summary>
    private float r;
    /// <summary>
    /// Are we testing data when calculated?
    /// </summary>
    public bool testData = true;
    /// <summary>
    /// List of possible angles of joint axes
    /// </summary>
    public List<float> possibleFi1 = new List<float>(), possibleFi2 = new List<float>(), possibleFi3 = new List<float>(), possibleFi4 = new List<float>(), possibleFi5 = new List<float>(), possibleFi6 = new List<float>();
    /// <summary>
    /// List of possible angles of axes
    /// </summary>
    [SerializeField]
    private List<Angles> possibleAngles = new List<Angles>();
    /// <summary>
    /// Selected angles for axes to pass to visualization
    /// </summary>
    public List<Angles> selectedSolutions = new List<Angles>();
    /// <summary>
    /// List of solutions for testing new targets
    /// </summary>
    public List<Angles> newTargetSolutions = new List<Angles>();
    /// <summary>
    /// List of minimal limits of axes
    /// </summary>
    public List<float> minAxesLimits = new List<float>();
    /// <summary>
    /// List of maximal limits of axes
    /// </summary>
    public List<float> maxAxesLimits = new List<float>();
    /// <summary>
    /// Chosen solution for visualization
    /// </summary>
    public Angles solution = new Angles();
    /// <summary>
    /// Temporary variables for axes
    /// </summary>
    public float fi1, fi2, fi3, fi4, fi5, fi6;
    /// <summary>
    /// Temporary variables for matrix variables
    /// </summary>
    float dx, dy, dz;
    /// <summary>
    /// Augmented variables for calculations
    /// </summary>
    public float dxp, dyp, dzp;
    /// <summary>
    /// Temporary variables for matrix calculation
    /// </summary>
    float ax, ay, az, bx, by, bz, cx, cy, cz;
    /// <summary>
    /// Temporary variables helping calcualting angles
    /// </summary>
    float w1, w2;
    /// <summary>
    /// Variables allowing defining workspace direction
    /// </summary>
    public float p, pp;
    /// <summary>
    /// Temporary variables for helper matrix calculation
    /// </summary>
    float Rax, Ray, Raz, Rby, Rbz, Rcy, Rcz;
    /// <summary>
    /// List of matrices calculated back from ending axes
    /// </summary>
    private List<Matrix4x4> testMatrices = new List<Matrix4x4>();
    /// <summary>
    /// Translations used in interpolation of movement
    /// </summary>
    private Target currentTranslation, traversingTo, startTranslation;
    /// <summary>
    /// Timers used in interpolation of movement
    /// </summary>
    private float progress = 1f, distanceToTarget = 0f, timeToTarget = 1f;
    /// <summary>
    /// Are we currently interpolating translation?
    /// </summary>
    public bool interpolate = true;
    /// <summary>
    /// Are we testing solutions with back-calculated matrices?
    /// </summary>
    public bool testSolutions = true;
    /// <summary>
    /// Reference to 6th pivot point
    /// </summary>
    private FollowPivot6 endingPoint = default;
    /// <summary>
    /// List containing last targets that were possible to reach
    /// </summary>
    private List<Target> lastProperTargets = new List<Target>();
    /// <summary>
    /// List containing last joint rotations that were able to reach
    /// </summary>
    private Angles lastProperAngles = new Angles(new List<float>());
    /// <summary>
    /// Maximum angle difference for checking if we have exceeded configuration limitations
    /// </summary>
    public float maxAngleDifference = 15f;
    /// <summary>
    /// Are we testing only angle differences or also testing configuration limits?
    /// </summary>
    public bool onlyTestDifferences = false;
    // DEBUG
    /// <summary>
    /// Testing target for checking if any errors occur
    /// </summary>
    public Target newTarget;
    /// <summary>
    /// Are we checking the new target?
    /// </summary>
    public bool checkSpecifiedTarget = false;
    /// <summary>
    /// Speed of interpolation of movement
    /// </summary>
    private float interpolationSpeed = 0.5f;
    /// <summary>
    /// Are we in linera movement?
    /// </summary>
    public bool inLinearMoveType = true;
    /// <summary>
    /// List of starting angles of axes in joint movement
    /// </summary>
    private List<float> startJointAngles = new List<float>();
    /// <summary>
    /// List of target rotations of axes in joint movement
    /// </summary>
    private List<float> targetJointAngles = new List<float>();
    /// <summary>
    /// Maximum difference of angles in two frames that a joint can have before signaling an error
    /// </summary>
    private float maxDifference = 0f;
    /// <summary>
    /// Testable visual representation for target debugging
    /// </summary>
    [SerializeField]
    private Transform debugTargetVis = default;
    /// <summary>
    /// Are we checking the difference between axes in concrrent frames?
    /// </summary>
    public bool checkDifferences;
    /// <summary>
    /// Rotations visible in editor for debugging
    /// </summary>
    public Vector3 debugRotations = Vector3.zero;
    /// <summary>
    /// Reference to object allowing for clamping angles rotations
    /// </summary>
    [SerializeField]
    private ClampRobotLimitAngle clampRobot = default;
    /// <summary>
    /// Refrence to object allowing detection of axes singularities
    /// </summary>
    [SerializeField]
    private ClampRobotSingularityAngle singularityAngle = default;

    /// <summary>
    /// Sets up starting values
    /// Sets up references
    /// Calculates starting position of joints
    /// Delays checking errors in calculated angles
    /// </summary>
    private void Start()
    {
        currentTranslation = traversingTo = startTranslation = currentTarget;
        endingPoint = GetComponentInChildren<FollowPivot6>();
        CalculateAxes();
        StartCoroutine(DelayCheckingSolutions());
        RobotData.Instance.CurrentTarget = currentTarget;
    }

    /// <summary>
    /// Allows of interpolation of axes to specified target
    /// Moves robot when specified buttons are pressed
    /// </summary>
    private void Update()
    {
        //debugTargetVis.localPosition = new Vector3(-currentTarget.position.x, currentTarget.position.y, currentTarget.position.z);
        if (inLinearMoveType)
        {
            if (interpolate)
            {
                if (currentTarget.position != traversingTo.position || currentTarget.rotation != traversingTo.rotation)
                {
                    traversingTo.position = currentTarget.position;
                    traversingTo.rotation = currentTarget.rotation;
                    startTranslation.position = currentTranslation.position;
                    startTranslation.rotation = currentTranslation.rotation;
                    distanceToTarget = Vector3.Distance(traversingTo.position, currentTranslation.position);
                    timeToTarget = distanceToTarget / interpolationSpeed;
                    progress = 0f;
                }
                else
                {
                    progress += Time.deltaTime * interpolationSpeed;
                    currentTranslation.position = Vector3.Lerp(startTranslation.position, currentTarget.position, progress / timeToTarget);
                    currentTranslation.rotation = Vector3.Slerp(startTranslation.rotation, currentTarget.rotation, progress / timeToTarget);
                    if (progress / timeToTarget >= 1)
                    {
                        if (lastProperTargets.Count > 20)
                        {
                            lastProperTargets.RemoveAt(0);
                        }
                        lastProperTargets.Add(currentTarget);
                    }
                }
            }
            else
            {
                currentTranslation = currentTarget;
                if (selectedSolutions.Count > 0)
                {
                    if (lastProperTargets.Count > 20)
                    {
                        lastProperTargets.RemoveAt(0);
                    }
                    if (FindObjectOfType<MatrixRobot>())
                        FindObjectOfType<MatrixRobot>().UpdatePositions();
                    lastProperTargets.Add(currentTarget);
                    // singularityAngle.ResetSingularity();
                }
                else
                {
                    Debug.Log("Error: No possible solutions in current configuration");
                    //singularityAngle.SetSingularity();
                    if (lastProperTargets.Count > 0)
                    {
                        currentTranslation = currentTarget = lastProperTargets[lastProperTargets.Count - 1];
                        lastProperTargets.RemoveAt(lastProperTargets.Count - 1);
                        lastProperTargets.Add(lastProperTargets[lastProperTargets.Count - 1]);
                    }
                    TestIfPossibleToMoveInOtherConfigurations();
                }
            }
            CalculateAxes();
            if (checkSpecifiedTarget)
            {
                bool can = CheckIfPossibleToMoveTo(newTarget);
                if (can) Debug.Log("Can move robot in current configuration");
                checkSpecifiedTarget = false;
            }
            TransformRobot();
        }
        else
        {
            if (interpolate)
            {
                if (progress == 0f)
                {
                    maxDifference = 0f;
                    for (int i = 0; i < startJointAngles.Count; i++)
                    {
                        if (Mathf.Abs(targetJointAngles[i] - startJointAngles[i]) > maxDifference)
                        {
                            maxDifference = Mathf.Abs(targetJointAngles[i] - startJointAngles[i]);
                        }
                    }
                    if (maxDifference == 0)
                    {
                        return;
                    }
                }
                progress += (Time.deltaTime / maxDifference) * interpolationSpeed * RobotData.Instance.MaxRobotJointSpeed;
                for (int j = 0; j < selectedSolutions[0].angles.Count; j++)
                {
                    selectedSolutions[0].angles[j] = Mathf.Lerp(startJointAngles[j], targetJointAngles[j], progress);
                }
                RobotData.Instance.CurrentAngles = selectedSolutions[0];
                currentTarget.position = endingPoint.transform.localPosition / 10f;
                currentTarget.position.x *= -1;
                currentTarget.rotation = GetRotationFromAxes(GetAngleData());
                currentTarget.rotation.x *= -1;
                currentTranslation = traversingTo = startTranslation = currentTarget;
                FindObjectOfType<MatrixRobot>().UpdatePositions(false);
            }
            else
            {
                FindObjectOfType<MatrixRobot>().UpdatePositions(false);
            }
        }
    }

    /// <summary>
    /// Rotates specified joint of robot by a degree amount
    /// </summary>
    /// <param name="axe">Index of joint to rotate</param>
    /// <param name="amount">Amount of degrees to rotate</param>
    public void RotateAxe(int axe, float amount)
    {
        if (selectedSolutions.Count > 0)
        {
            Angles newRotation = selectedSolutions[0];
            if (newRotation.angles[axe] + amount < maxAxesLimits[axe] && newRotation.angles[axe] + amount > minAxesLimits[axe])
            {
                newRotation.angles[axe] += amount;
                clampRobot.ResetLimit();
            }
            else
            {
                clampRobot.SetLimit(axe, newRotation.angles[axe] + amount < maxAxesLimits[axe]);
            }
            selectedSolutions[0] = newRotation;
            RobotData.Instance.CurrentTarget = currentTarget;
            currentTarget.position = endingPoint.transform.localPosition / 10f;
            currentTarget.position.x *= -1;
            currentTarget.rotation = GetRotationFromAxes(selectedSolutions[0].angles);
            currentTarget.rotation.x *= -1;
            lastProperAngles.angles = selectedSolutions[0].angles;
        }
    }

    /// <summary>
    /// Delays checking for errors in robot movement
    /// </summary>
    /// <returns>Handle to coroutine</returns>
    private IEnumerator DelayCheckingSolutions()
    {
        yield return new WaitForSeconds(1f);
        checkDifferences = true;
    }

    /// <summary>
    /// Gets curently calculated angles data
    /// </summary>
    /// <returns>List of angles to visualize</returns>
    public List<float> GetAngleData()
    {
        if (selectedSolutions.Count >= 1)
        {
            RobotData.Instance.CurrentAngles = selectedSolutions[0];
            return selectedSolutions[0].angles;
        }
        else
        {
            return new List<float>();
        }
    }

    /// <summary>
    /// Gets currently setup linis for angles
    /// </summary>
    /// <returns>List of minimal and maximal angles for each joint</returns>
    public List<Tuple<float, float>> GetAxeLimits()
    {
        var returnable = new List<Tuple<float, float>>();
        for (int i = 0; i < minAxesLimits.Count; i++)
        {
            returnable.Add(new Tuple<float, float>(minAxesLimits[i], maxAxesLimits[i]));
        }
        return returnable;
    }

    /// <summary>
    /// Main method for calculating angles of joints according to current target
    /// </summary>
    /// <param name="visualizeOnRobot">Are we using the angles to send to visualization robot?</param>
    /// <param name="testTarget">Target used in testing is robot is able to get to in curent configuration</param>
    /// <returns></returns>
    private List<float> CalculateAxes(bool visualizeOnRobot = true, Target testTarget = default)
    {
        possibleAngles.Clear();
        if (testTarget.Equals((Target)default))
        {
            targetMatrix = ChangeToCosinusMatrix(currentTranslation.rotation);
            targetMatrix.SetColumn(3, new Vector4(currentTranslation.position.x, -currentTranslation.position.y, currentTranslation.position.z, 1));
        }
        else
        {
            targetMatrix = ChangeToCosinusMatrix(testTarget.rotation);
            targetMatrix.SetColumn(3, new Vector4(testTarget.position.x, -testTarget.position.y, testTarget.position.z, 1));
        }
        GetDataForNextSteps();
        CheckIfWithinWorkSpace();
        if (p == 0 && pp == 0)
        {
            Debug.Log("Point not in workspace");
            if (visualizeOnRobot)
            {
                selectedSolutions.Clear();
            }
            else
            {
                newTargetSolutions.Clear();
            }
            if (lastProperTargets.Count > 0)
            {
                currentTranslation = currentTarget = lastProperTargets[lastProperTargets.Count - 1];
                lastProperTargets.RemoveAt(lastProperTargets.Count - 1);
                lastProperTargets.Add(lastProperTargets[lastProperTargets.Count - 1]);
            }
            return null;
        }
        GetAngle1();
        if (!onlyTestDifferences)
        {
            if (testData)
                TestAngles();
            if (testSolutions)
            {
                GetConfigurationSolutions(visualizeOnRobot);
            }
            else
            {
                Angles[] temp = new Angles[possibleAngles.Count];
                possibleAngles.CopyTo(temp);
                selectedSolutions = new List<Angles>(temp);
                RobotData.Instance.CurrentAngles = selectedSolutions[0];
            }
        }
        else
        {
            if (testData)
                TestAngles();
            GetConfigurationSolutions(visualizeOnRobot);
            CheckAxeDifferences();
        }
        if (visualizeOnRobot)
        {
            if (selectedSolutions.Count > 0)
            {
                lastProperAngles = selectedSolutions[0];
                //debugRotations = GetRotationFromAxes(selectedSolutions[0].angles);
                return selectedSolutions[0].angles;
            }
            else
            {
                return null;
            }
        }
        else
        {
            if (newTargetSolutions.Count > 0)
                return newTargetSolutions[0].angles;
            else
                return null;
        }
    }

    /// <summary>
    /// Tests if the angle difference in joint is bigger than maximum value
    /// </summary>
    /// <returns>Angles adjusted to error check</returns>
    private Angles CheckAxeDifferences()
    {
        Angles solution = new Angles();
        if (selectedSolutions.Count > 0 && lastProperAngles.angles.Count > 0)
        {
            foreach (Angles a in selectedSolutions)
            {
                bool proper = true;
                for (int i = 0; i < a.angles.Count; i++)
                {
                    if (Mathf.Abs(a.angles[i] - lastProperAngles.angles[i]) > maxAngleDifference)
                    {
                        proper = false;
                    }
                }
                if (proper)
                {
                    solution = a;
                    break;
                }
            }
            if (solution == new Angles())
            {
                selectedSolutions.Clear();
                Debug.Log("Error: Big diference in angles and no solutions, returning to last proper angles");
                if (lastProperTargets.Count > 0)
                {
                    currentTarget = currentTranslation = lastProperTargets[lastProperTargets.Count - 1];
                    RobotData.Instance.CurrentTarget = currentTarget;
                    lastProperTargets.RemoveAt(lastProperTargets.Count - 1);
                    lastProperTargets.Add(lastProperTargets[lastProperTargets.Count - 1]);
                }
            }
        }
        return solution;
    }

    /// <summary>
    /// Sets visual representation of robot to calculated axes
    /// </summary>
    private void TransformRobot()
    {
        if (selectedSolutions.Count > 0)
        {
            FindObjectOfType<MatrixRobot>().UpdatePositions();
            //singularityAngle.ResetSingularity();
        }
        else
        {
            Debug.Log("Error: No possible solutions");
            // singularityAngle.SetSingularity();
            if (lastProperTargets.Count > 0)
            {
                currentTranslation = currentTarget = lastProperTargets[lastProperTargets.Count - 1];
                lastProperTargets.RemoveAt(lastProperTargets.Count - 1);
                lastProperTargets.Add(lastProperTargets[lastProperTargets.Count - 1]);
            }
        }
    }

    /// <summary>
    /// Calculates rotation matrix for angles calculation
    /// </summary>
    /// <param name="rotation">Rotation in euler angles</param>
    /// <returns>Transfrmation matrix containing rotation</returns>
    private Matrix4x4 ChangeToCosinusMatrix(Vector3 rotation)
    {
        var matrix = new Matrix4x4();

        var sin1 = Sin(rotation.x);
        var sin2 = Sin(rotation.y + 90);
        var sin3 = Sin(rotation.z);

        var cos1 = Cos(rotation.x);
        var cos2 = Cos(rotation.y + 90);
        var cos3 = Cos(rotation.z);

        Vector4 row1 = new Vector4(
            cos2 * cos3,
            -cos1 * sin3 + sin1 * sin2 * cos3,
            sin1 * sin3 + cos1 * sin2 * cos3,
            0);
        Vector4 row2 = new Vector4(
            cos2 * sin3,
            cos1 * cos3 + sin1 * sin2 * sin3,
            -sin1 * cos3 + cos1 * sin2 * sin3,
           0);
        Vector4 row3 = new Vector4(
            -sin2,
            sin1 * cos2,
            cos1 * cos2,
            0);
        Vector4 row4 = new Vector4(0, 0, 0, 1);
        matrix.SetRow(0, row1);
        matrix.SetRow(1, row2);
        matrix.SetRow(2, row3);
        matrix.SetRow(3, row4);
        return matrix;
    }

    /// <summary>
    /// Calculated data for each step of axe calculation
    /// </summary>
    private void GetDataForNextSteps()
    {
        ax = targetMatrix.GetColumn(0).x;
        ay = targetMatrix.GetColumn(0).y;
        az = targetMatrix.GetColumn(0).z;
        bx = targetMatrix.GetColumn(1).x;
        by = targetMatrix.GetColumn(1).y;
        bz = targetMatrix.GetColumn(1).z;
        cx = targetMatrix.GetColumn(2).x;
        cy = targetMatrix.GetColumn(2).y;
        cz = targetMatrix.GetColumn(2).z;
        dx = targetMatrix.GetColumn(3).x;
        dy = targetMatrix.GetColumn(3).y;
        dz = targetMatrix.GetColumn(3).z;
        dxp = dx - lambda6 * cx;
        dyp = dy - lambda6 * cy;
        dzp = dz - lambda6 * cz;
        r = Mathf.Sqrt(dxp * dxp + dyp * dyp);
    }

    /// <summary>
    /// Checks if specified target is within robot workspace
    /// </summary>
    private void CheckIfWithinWorkSpace()
    {
        float p1r = 1.006f, p1d = -0.137f;
        float p2r = 0.281f, p2d = -1.053f;
        float p3r = -0.169f, p3d = 0.455f;
        float p4r = 1.006f, p4d = -0.137f;
        float p5r = 0.150f, p5d = 1.948f;
        float p6r = 1.512f, p6d = 1.080f;
        float p7r = 1.588f, p7d = 0.741f;
        float p8r = 0.784f, p8d = -0.062f;
        float p9r = 0.154f, p9d = 0.244f;
        float p10r = -0.592f, p10d = -0.277f;
        float p11r = 0.150f, p11d = 0.646f;
        float p12r = 0.150f, p12d = -1.058f;
        float s1r = 0.211f, s1d = -0.252f;
        float s2r = 0.150f, s2d = 0.445f;
        float s3r = 0.784f, s3d = 0.741f;
        float s4r = s2r, s4d = s2d;
        float s5r = s4r, s5d = s4d;
        float r1 = 0.803f, r2 = 1.503f, r3 = r1, r4 = 1.035f, r5 = 0.201f;
        p = 0; pp = 0;

        if (p6d <= dzp && dzp <= p5d && ((r - s2r) * (r - s2r) + (dz - s2d) * (dz - s2d) <= r2 * r2)) p = 1;
        if (p6d <= dzp && dzp <= p5d && ((-r - s2r) * (-r - s2r) + (dz - s2d) * (dz - s2d) <= r2 * r2)) pp = 1;

        if (p11d <= dzp && dzp <= p6d && (r <= (s3r + Mathf.Sqrt(r3 * r3 - (dz - s3d) * (dz - s3d))))) p = 1;
        if (p11d <= dzp && dzp <= p6d && (-r > (s2r - Mathf.Sqrt(r2 * r2 - (dz - s2d) * (dz - s2d))))) pp = 1;

        if (p3d <= dzp && dzp <= p11d && (s5r + Mathf.Sqrt(r5 * r5 - (dz - s5d) * (dz - s5d)) <= r && r <= s3r + Mathf.Sqrt(r3 * r3 - (dz - s3d) * (dz - s3d)))) p = 1;
        if (p3d <= dzp && dzp <= p11d && (s2r - Mathf.Sqrt(r2 * r2 - (dz - s2d) * (dz - s2d)) <= -r && -r <= s5r - Mathf.Sqrt(r5 * r5 - (dz - s5d) * (dz - s5d)))) pp = 1;

        if (p9d <= dzp && dzp <= p3d && (s5r + Mathf.Sqrt(r5 * r5 - (dz - s5d) * (dz - s5d)) <= r && r <= s3r + Mathf.Sqrt(r3 * r3 + (dz - s3d) * (dz - s3d)))) p = 1;
        if (p9d <= dzp && dzp <= p3d && (s2r - Mathf.Sqrt(r2 * r2 - (dz - s2d) * (dz - s2d)) <= -r && -r <= s1r - Mathf.Sqrt(r1 * r1 - (dz - s1d) * (dz - s1d)))) pp = 1;

        if (p8d <= dzp && dzp <= p9d && (s3r - Mathf.Sqrt(r3 * r3 - (dz - s3d) * (dz - s3d)) <= r && r <= s3r + Mathf.Sqrt(r3 * r3 + (dz - s3d) * (dz - s3d)))) p = 1;
        if (p8d <= dzp && dzp <= p9d && (s2r - Mathf.Sqrt(r2 * r2 - (dz - s2d) * (dz - s2d)) <= -r && -r <= s1r - Mathf.Sqrt(r1 * r1 - (dz - s1d) * (dz - s1d)))) pp = 1;

        if (p1d <= dzp && dzp <= p8d && (s2r - Mathf.Sqrt(r2 * r2 - (dz - s2d) * (dz - s2d)) <= -r && -r <= s1r - Mathf.Sqrt(r1 * r1 - (dz - s1d) * (dz - s1d)))) pp = 1;

        if (p10d <= dzp && dzp <= p1d && (s4r + Mathf.Sqrt(r4 * r4 - (dz - s4d) * (dz - s4d)) <= r && r <= s1r + Mathf.Sqrt(r1 * r1 + (dz - s1d) * (dz - s1d)))) p = 1;
        if (p10d <= dzp && dzp <= p1d && (s2r - Mathf.Sqrt(r2 * r2 - (dz - s2d) * (dz - s2d)) <= -r && -r <= s1r - Mathf.Sqrt(r1 * r1 - (dz - s1d) * (dz - s1d)))) pp = 1;

        if (p2d <= dzp && dzp <= p10d && (r4 * r4 <= (r - s4r) * (r - s4r) + (dz - s4d) * (dz - s4d)) && (r - s1r) * (r - s1r) + (dz - s1d) * (dz - s1d) <= r1 * r1) p = 1;
        if (p2d <= dzp && dzp <= p10d && (r4 * r4 <= (-r - s4r) * (-r - s4r) + (dz - s4d) * (dz - s4d)) && (-r - s1r) * (-r - s1r) + (dz - s1d) * (dz - s1d) <= r1 * r1) pp = 1;

        if (p12d <= dzp && dzp <= p2d && (r2 * r2 <= (r - s2r) * (r - s2r) + (dz - s2d) * (dz - s2d))) p = 1;
        if (p12d <= dzp && dzp <= p2d && (r2 * r2 <= (-r - s2r) * (-r - s2r) + (dz - s2d) * (dz - s2d))) pp = 1;
    }

    /// <summary>
    /// Calculates first joint rotation
    /// </summary>
    private void GetAngle1()
    {
        possibleFi1.Clear();
        float fi1p = 0;
        fi1 = 0;
        fi1p = Atan2(dyp, dxp); ;
        if (dxp < 0 && dyp >= 0)
            fi1p += 180;
        else if (dxp < 0 && dyp <= 0)
            fi1p -= 180;
        if (p == 1 && pp == 0 && dxp * dxp + dyp * dyp > 0)
        {
            fi1 = fi1p;
            if (-180 <= fi1 && fi1 <= 180)
                possibleFi1.Add(fi1);
            fi1 += 360;
            if (-180 <= fi1 && fi1 <= 180)
                possibleFi1.Add(fi1);
            fi1 -= 720;
            if (-180 <= fi1 && fi1 <= 180)
                possibleFi1.Add(fi1);

            //Posssible 180 degree fix
            fi1 += 540;
            if (-180 <= fi1 && fi1 <= 180)
                possibleFi1.Add(fi1);
            fi1 += 360;
            if (-180 <= fi1 && fi1 <= 180)
                possibleFi1.Add(fi1);
            fi1 -= 720;
            if (-180 <= fi1 && fi1 <= 180)
                possibleFi1.Add(fi1);
        }
        else if (p == 0 && pp == 1 && dxp * dxp + dyp * dyp > 0)
        {
            fi1 = fi1p - Mathf.Sign(fi1p) * 180;
            if (-180 <= fi1 && fi1 <= 180)
                possibleFi1.Add(fi1);
            fi1 += 360;
            if (-180 <= fi1 && fi1 <= 180)
                possibleFi1.Add(fi1);
            fi1 -= 720;
            if (-180 <= fi1 && fi1 <= 180)
                possibleFi1.Add(fi1);
            //Posssible 180 degree fix
            fi1 += 540;
            if (-180 <= fi1 && fi1 <= 180)
                possibleFi1.Add(fi1);
            fi1 += 360;
            if (-180 <= fi1 && fi1 <= 180)
                possibleFi1.Add(fi1);
            fi1 -= 720;
            if (-180 <= fi1 && fi1 <= 180)
                possibleFi1.Add(fi1);
        }
        else if (p == 1 && pp == 1 && dxp * dxp + dyp * dyp > 0)
        {
            if (-180 <= fi1p && fi1p <= 180)
                possibleFi1.Add(fi1p);
            fi1p += 360;
            if (-180 <= fi1p && fi1p <= 180)
                possibleFi1.Add(fi1p);
            fi1p -= 720;
            if (-180 <= fi1p && fi1p <= 180)
                possibleFi1.Add(fi1p);
            fi1p += 360;

            fi1p = fi1p - Mathf.Sign(fi1p) * 180;
            if (-180 <= fi1p && fi1p <= 180)
                possibleFi1.Add(fi1p);
            fi1p += 360;
            if (-180 <= fi1p && fi1p <= 180)
                possibleFi1.Add(fi1p);
            fi1p -= 720;
            if (-180 <= fi1p && fi1p <= 180)
                possibleFi1.Add(fi1p);
        }
        else if (dxp * dxp + dyp * dyp == 0)
        {
            possibleFi1.Add(0);
        }
        if (possibleFi1.Count == 0)
        {
            Debug.Log("No solutions for axis 1");
        }
        foreach (float fi1 in possibleFi1)
        {
            GetAngle3(fi1);
        }
    }

    /// <summary>
    /// Calculates second joint rotation
    /// </summary>
    /// <param name="fi1">Rotation of first joint</param>
    /// <param name="fi3">Rotation of third joint</param>
    private void GetAngle2(float fi1, float fi3)
    {
        possibleFi2.Clear();
        fi2 = 0;
        float s2 = (w1 * (li2 + li3 * Cos(fi3) + lambda4 * Sin(fi3)) - w2 * (-li3 * Sin(fi3) + lambda4 * Cos(fi3))) /
            (-(li2 * li2 + li3 * li3 + lambda4 * lambda4 + 2 * li2 * ((li3 * Cos(fi3) + lambda4 * Sin(fi3)))));
        float c2 = (-w1 * (-li3 * Sin(fi3) + lambda4 * Cos(fi3)) - w2 * (li2 + li3 * Cos(fi3) + lambda4 * Sin(fi3))) /
            (-(li2 * li2 + li3 * li3 + lambda4 * lambda4 + 2 * li2 * (li3 * Cos(fi3) + lambda4 * Sin(fi3))));
        fi2 = Atan2(s2, c2);
        if (s2 < 0 && c2 < 0) fi2 -= 180;
        else if (s2 > 0 && c2 < 0) fi2 += 180;
        if (-155f <= fi2 && fi2 <= 95f)
            possibleFi2.Add(fi2);
        fi2 += 360;
        if (-155f <= fi2 && fi2 <= 95f)
            possibleFi2.Add(fi2);
        fi2 -= 720;
        if (-155f <= fi2 && fi2 <= 95f)
            possibleFi2.Add(fi2);
        if (possibleFi2.Count == 0)
        {
            Debug.Log("No solutions for axis 2");
        }
        foreach (float fi2 in possibleFi2)
        {
            GetHelperMatrix(fi1, fi2, fi3);
            if (Ray * Ray + Raz * Raz > 0)
            {
                GetAngle4(fi1, fi2, fi3);
            }
            else
            {
                fi5 = 0;
                GetAngles46(fi1, fi2, fi3, fi5);
            }
        }
    }

    /// <summary>
    /// Calculates rotation of third joint
    /// </summary>
    /// <param name="fi1">ROtation of first joint</param>
    private void GetAngle3(float fi1)
    {
        possibleFi3.Clear();
        fi3 = 0;
        float a = 2 * li2 * li3;
        float b = 2 * li2 * lambda4;
        w1 = -li1 + Sin(fi1) * dy + Cos(fi1) * dx - lambda6 * (Cos(fi1) * cx + Sin(fi1) * cy);
        w2 = -lambda1 + dz - lambda6 * cz;
        float o1o5 = w1 * w1 + w2 * w2;
        float c = o1o5 - li2 * li2 - li3 * li3 - lambda4 * lambda4;
        if (a + c == 0)
        {
            fi3 = 2 * Atan2(c - a, 2 * b);
            if (-75f <= fi3 && fi3 <= 180f)
                possibleFi3.Add(fi3);
            fi3 += 360;
            if (-75f <= fi3 && fi3 <= 180f)
                possibleFi3.Add(fi3);
            fi3 -= 720;
            if (-75f <= fi3 && fi3 <= 180f)
                possibleFi3.Add(fi3);
        }
        else
        {
            float fi3temp = 2 * Atan2(b - Mathf.Sqrt(a * a + b * b - c * c), a + c);
            if (-75f <= fi3temp && fi3temp <= 180f)
                possibleFi3.Add(fi3temp);
            fi3temp += 360;
            if (-75f <= fi3temp && fi3temp <= 180f)
                possibleFi3.Add(fi3temp);
            fi3temp -= 720;
            if (-75f <= fi3temp && fi3temp <= 180f)
                possibleFi3.Add(fi3temp);
            fi3temp = 2 * Atan2(b + Mathf.Sqrt(a * a + b * b - c * c), a + c);
            if (-75f <= fi3temp && fi3temp <= 180f)
                possibleFi3.Add(fi3temp);
            fi3temp += 360;
            if (-75f <= fi3temp && fi3temp <= 180f)
                possibleFi3.Add(fi3temp);
            fi3temp -= 720;
            if (-75f <= fi3temp && fi3temp <= 180f)
                possibleFi3.Add(fi3temp);
        }
        if (possibleFi3.Count == 0)
        {
            Debug.Log("No solutions for axis 3");
        }
        foreach (float fi3 in possibleFi3)
        {
            GetAngle2(fi1, fi3);
        }
    }

    /// <summary>
    /// Calculates matrix used in calculating rotations of 4th, 5th and 6th joint
    /// </summary>
    /// <param name="fi1">Rotation of first joint</param>
    /// <param name="fi2">Rotation of second joint</param>
    /// <param name="fi3">Rotation of third joint</param>
    private void GetHelperMatrix(float fi1, float fi2, float fi3)
    {
        Rax = Cos(fi2 + fi3) * (Cos(fi1) * cx + Sin(fi1) * cy) + Sin(fi2 + fi3) * cz;
        Ray = -Sin(fi2 + fi3) * (Cos(fi1) * cx + Sin(fi1) * cy) + Cos(fi2 + fi3) * cz;
        Raz = Sin(fi1) * cx - Cos(fi1) * cy;
        Rby = -Sin(fi2 + fi3) * (Cos(fi1) * ax + Sin(fi1) * ay) + Cos(fi2 + fi3) * az;
        Rbz = Sin(fi1) * ax - Cos(fi1) * ay;
        Rcy = -Sin(fi2 + fi3) * (Cos(fi1) * bx + Sin(fi1) * by) + Cos(fi2 + fi3) * bz;
        Rcz = Sin(fi1) * bx - Cos(fi1) * by;
    }

    /// <summary>
    /// Calculates rotation of fourth joint
    /// </summary>
    /// <param name="fi1">Rotation of first joint</param>
    /// <param name="fi2">Rotation of second joint</param>
    /// <param name="fi3">Rotation of third joint</param>
    private void GetAngle4(float fi1, float fi2, float fi3)
    {
        possibleFi4.Clear();
        fi4 = 0;
        float fi4p = Atan2(Raz, Ray);
        if (-400 <= fi4p + 360 && fi4p + 360 <= 400) possibleFi4.Add(fi4p + 360);
        if (-400 <= fi4p + 180 && fi4p + 180 <= 400) possibleFi4.Add(fi4p + 180);
        if (-400 <= fi4p && fi4p <= 400) possibleFi4.Add(fi4p);
        if (-400 <= fi4p - 180 && fi4p - 180 <= 400) possibleFi4.Add(fi4p - 180);
        if (-400 <= fi4p - 360 && fi4p - 360 <= 400) possibleFi4.Add(fi4p - 360);
        if (possibleFi4.Count == 0)
        {
            Debug.Log("No solutions for axis 4");
        }
        foreach (float fi4 in possibleFi4)
        {
            GetAngle5(fi1, fi2, fi3, fi4);
        }
    }

    /// <summary>
    /// Calculates rotation of fith joint
    /// </summary>
    /// <param name="fi1">Rotation of first joint</param>
    /// <param name="fi2">Rotation of second joint</param>
    /// <param name="fi3">Rotation of third joint</param>
    /// <param name="fi4">Rotation of fourth joint</param>
    private void GetAngle5(float fi1, float fi2, float fi3, float fi4)
    {
        possibleFi5.Clear();
        float s5 = Cos(fi4) * Ray + Sin(fi4) * Raz;
        float c5 = Rax;
        float fi5p = Atan2(s5, c5);
        if (c5 >= 0)
            fi5 = fi5p;
        else if (c5 < 0 && s5 < 0) fi5 = fi5p - 180;
        else fi5 = fi5p + 180;
        if (-120 <= fi5 && fi5 <= 120)
            possibleFi5.Add(fi5);
        fi5 += 360;
        if (-120 <= fi5 && fi5 <= 120)
            possibleFi5.Add(fi5);
        fi5 -= 720;
        if (-120 <= fi5 && fi5 <= 120)
            possibleFi5.Add(fi5);
        if (possibleFi5.Count == 0)
        {
            Debug.Log("No solutions for axis 5");
        }
        foreach (float fi5 in possibleFi5)
        {
            GetAngle6(fi1, fi2, fi3, fi4, fi5);
        }
    }

    /// <summary>
    /// Calculates rotation of sixth joint
    /// </summary>
    /// <param name="fi1">Rotation of first joint</param>
    /// <param name="fi2">Rotation of saecond joint</param>
    /// <param name="fi3">Rotation of third joint</param>
    /// <param name="fi4">Rotation of fourth joint</param>
    /// <param name="fi5">Rotation of fifth joint</param>
    private void GetAngle6(float fi1, float fi2, float fi3, float fi4, float fi5)
    {
        possibleFi6.Clear();
        float s6 = -Sin(fi4) * Rby + Cos(fi4) * Rbz;
        float c6 = -Sin(fi4) * Rcy + Cos(fi4) * Rcz;
        float fi6p = Atan2(s6, c6);
        if (c6 > 0 && s6 != 0)
        {
            if (-400 <= fi6p + 720 && fi6p + 720 <= 400) possibleFi6.Add(fi6p + 720);
            if (-400 <= fi6p + 360 && fi6p + 360 <= 400) possibleFi6.Add(fi6p + 360);
            if (-400 <= fi6p && fi6p <= 400) possibleFi6.Add(fi6p);
            if (-400 <= fi6p - 360 && fi6p - 360 <= 400) possibleFi6.Add(fi6p - 360);
            if (-400 <= fi6p - 720 && fi6p - 720 <= 400) possibleFi6.Add(fi6p - 720);
        }
        else if (c6 <= 0 && s6 != 0)
        {
            if (-274 <= fi6p + 540 && fi6p + 540 <= 274) possibleFi6.Add(fi6p + 540);
            if (-274 <= fi6p + 180 && fi6p + 180 <= 274) possibleFi6.Add(fi6p + 180);
            if (-274 <= fi6p - 180 && fi6p - 180 <= 274) possibleFi6.Add(fi6p - 180);
            if (-274 <= fi6p - 540 && fi6p - 540 <= 274) possibleFi6.Add(fi6p - 540);
        }
        else
        {
            possibleFi6.Add(-360);
            possibleFi6.Add(-180);
            possibleFi6.Add(0);
            possibleFi6.Add(180);
            possibleFi6.Add(360);
        }
        if (possibleFi6.Count == 0)
        {
            Debug.Log("No solutions for axis 6");
        }
        foreach (float fi6 in possibleFi6)
        {
            possibleAngles.Add(new Angles() { angles = new List<float>() { fi1, fi2, fi3, fi4, fi5, fi6 } });
        }
    }

    /// <summary>
    /// Calculates sum of fourth and sixth rotations
    /// </summary>
    /// <param name="fi1">Rotation of first joint</param>
    /// <param name="fi2">Rotation of second joint</param>
    /// <param name="fi3">Rotation of third joint</param>
    /// <param name="fi5">Rotation of fifth joint</param>
    private void GetAngles46(float fi1, float fi2, float fi3, float fi5)
    {
        possibleFi6.Clear();
        possibleFi4.Clear();
        possibleFi4.Add(fi4);
        possibleFi4.Add(0);
        float sum46 = Atan2(-Rcy, Rcz);
        if (Rcz > 0 && Rcy != 0)
        {
            foreach (float fi4 in possibleFi4)
            {
                if (-800 <= sum46 && sum46 <= 800)
                {
                    possibleFi6.Add(sum46 - fi4);
                    possibleAngles.Add(new Angles() { angles = new List<float>() { fi1, fi2, fi3, fi4, fi5, sum46 - fi4 } });
                }
                if (-800 <= sum46 - 360 && sum46 - 360 <= 800)
                {
                    possibleFi6.Add(sum46 - 360 - fi4);
                    possibleAngles.Add(new Angles() { angles = new List<float>() { fi1, fi2, fi3, fi4, fi5, sum46 - 360 - fi4 } });

                }
                if (-800 <= sum46 - 720 && sum46 - 720 <= 800)
                {
                    possibleFi6.Add(sum46 - 720 - fi4);
                    possibleAngles.Add(new Angles() { angles = new List<float>() { fi1, fi2, fi3, fi4, fi5, sum46 - 720 - fi4 } });
                }
            }
        }
        else if (Rcz <= 0 && Rcy != 0)
        {
            foreach (float fi4 in possibleFi4)
            {
                if (-800 <= sum46 - 180 && sum46 - 180 <= 800)
                {
                    possibleFi6.Add(sum46 - 180 - fi4);
                    possibleAngles.Add(new Angles() { angles = new List<float>() { fi1, fi2, fi3, fi4, fi5, sum46 - 180 - fi4 } });

                }
                if (-800 <= sum46 + 180 && sum46 + 180 <= 800)
                {
                    possibleFi6.Add(sum46 + 180 - fi4);
                    possibleAngles.Add(new Angles() { angles = new List<float>() { fi1, fi2, fi3, fi4, fi5, sum46 + 180 - fi4 } });
                }
                if (-800 <= sum46 - 540 && sum46 - 540 <= 800)
                {
                    possibleFi6.Add(sum46 - 540 - fi4);
                    possibleAngles.Add(new Angles() { angles = new List<float>() { fi1, fi2, fi3, fi4, fi5, sum46 - 540 - fi4 } });
                }
                if (-800 <= sum46 + 540 && sum46 + 540 <= 800)
                {
                    possibleFi6.Add(sum46 + 540 - fi4);
                    possibleAngles.Add(new Angles() { angles = new List<float>() { fi1, fi2, fi3, fi4, fi5, sum46 + 540 - fi4 } });
                }
            }
        }
        else
        {
            foreach (float fi4 in possibleFi4)
            {
                if (-800 <= fi4 - 720 && fi4 - 720 <= 800)
                    possibleFi6.Add(-(fi4 - 720));
                if (-800 <= fi4 - 540 && fi4 - 540 <= 800)
                    possibleFi6.Add(-(fi4 - 540));
                if (-800 <= fi4 - 360 && fi4 - 360 <= 800)
                    possibleFi6.Add(-(fi4 - 360));
                if (-800 <= fi4 - 180 && fi4 - 180 <= 800)
                    possibleFi6.Add(-(fi4 - 180));
                if (-800 <= fi4 && fi4 <= 800)
                    possibleFi6.Add(-fi4);
                if (-800 <= fi4 + 720 && fi4 + 720 <= 800)
                    possibleFi6.Add(-(fi4 + 720));
                if (-800 <= fi4 + 540 && fi4 + 540 <= 800)
                    possibleFi6.Add(-(fi4 + 540));
                if (-800 <= fi4 + 360 && fi4 + 360 <= 800)
                    possibleFi6.Add(-(fi4 + 360));
                if (-800 <= fi4 + 180 && fi4 + 180 <= 800)
                    possibleFi6.Add(-(fi4 + 180));
                foreach (float fi6 in possibleFi6)
                {
                    possibleAngles.Add(new Angles() { angles = new List<float>() { fi1, fi2, fi3, fi4, fi5, fi6 } });
                }
            }
        }
    }

    /// <summary>
    /// Tests calculated angles
    /// </summary>
    private void TestAngles()
    {
        testMatrices.Clear();
        for (int i = 0; i < possibleAngles.Count; i++)
        {
            Matrix4x4 testMatrix = Matrix4x4.zero;
            var a = possibleAngles[i].angles;
            float ax = Cos(a[0]) * (
                -Sin(a[1] + a[2]) * (
                Cos(a[3]) * Cos(a[4]) * Cos(a[5]) - Sin(a[3]) * Sin(a[5])
                ) - Cos(a[1] + a[2]) * Sin(a[4]) * Cos(a[5])
                ) + Sin(a[0]) * (
                Sin(a[3]) * Cos(a[4]) * Cos(a[5]) + Cos(a[3]) * Sin(a[5])
                );
            float ay = Sin(a[0]) * (
                -Sin(a[1] + a[2]) * (
                Cos(a[3]) * Cos(a[4]) * Cos(a[5]) - Sin(a[3]) * Sin(a[5])
                ) - Cos(a[1] + a[2]) * Sin(a[4]) * Cos(a[5])
                ) - Cos(a[0]) * (
                Sin(a[3]) * Cos(a[4]) * Cos(a[5]) + Cos(a[3]) * Sin(a[5])
                );
            float az = Cos(a[1] + a[2]) * (
                Cos(a[3]) * Cos(a[4]) * Cos(a[5]) - Sin(a[3]) * Sin(a[5])
                ) - Sin(a[1] + a[2]) * Sin(a[4]) * Cos(a[5]);

            float bx = Cos(a[0]) * (
                -Sin(a[1] + a[2]) * (
                -Cos(a[3]) * Cos(a[4]) * Sin(a[5]) - Sin(a[3]) * Cos(a[5])
                ) + Cos(a[1] + a[2]) * Sin(a[4]) * Sin(a[5])
                ) + Sin(a[0]) * (
                -Sin(a[3]) * Cos(a[4]) * Sin(a[5]) + Cos(a[3]) * Cos(a[5])
                );
            float by = Sin(a[0]) * (
                -Sin(a[1] + a[2]) * (
                -Cos(a[3]) * Cos(a[4]) * Sin(a[5]) - Sin(a[3]) * Cos(a[5])
                ) + Cos(a[1] + a[2]) * Sin(a[4]) * Sin(a[5])
                ) - Cos(a[0]) * (
                -Sin(a[3]) * Cos(a[4]) * Sin(a[5]) + Cos(a[3]) * Cos(a[5])
                );
            float bz = Cos(a[1] + a[2]) * (
                -Cos(a[3]) * Cos(a[4]) * Sin(a[5]) - Sin(a[3]) * Cos(a[5])
                ) + Sin(a[1] + a[2]) * Sin(a[4]) * Sin(a[5]);

            float cx = Cos(a[0]) * (
                -Sin(a[1] + a[2]) * Cos(a[3]) * Sin(a[4]) + Cos(a[1] + a[2]) * Cos(a[4])
                ) + Sin(a[0]) * Sin(a[3]) * Sin(a[4]);
            float cy = Sin(a[0]) * (
                -Sin(a[1] + a[2]) * Cos(a[3]) * Sin(a[4]) + Cos(a[1] + a[2]) * Cos(a[4])
                ) - Cos(a[0]) * Sin(a[3]) * Sin(a[4]);
            float cz = Cos(a[1] + a[2]) * Cos(a[3]) * Sin(a[4]) + Sin(a[1] + a[2]) * Cos(a[4]);

            float dx = (li1 - li2 * Sin(a[1]) - li3 * Sin(a[1] + a[2]) + lambda4 * Cos(a[1] + a[2])) * Cos(a[0]);
            float dy = (li1 - li2 * Sin(a[1]) - li3 * Sin(a[1] + a[2]) + lambda4 * Cos(a[1] + a[2])) * Sin(a[0]);
            float dz = lambda1 + li2 * Cos(a[1]) + li3 * Cos(a[1] + a[2]) + lambda4 * Sin(a[1] + a[2]);

            testMatrix.m00 = ax;
            testMatrix.m10 = targetMatrix.m10;
            testMatrix.m20 = targetMatrix.m20;
            testMatrix.m30 = 0;
            testMatrix.m01 = bx;
            testMatrix.m11 = targetMatrix.m11;
            testMatrix.m21 = targetMatrix.m21;
            testMatrix.m31 = 0;
            testMatrix.m02 = targetMatrix.m02;
            testMatrix.m12 = cy;
            testMatrix.m22 = cz;
            testMatrix.m32 = 0;
            testMatrix.m03 = dx;
            testMatrix.m13 = dy;
            testMatrix.m23 = dz;
            testMatrix.m33 = 1;

            targetMatrix.m03 = dxp;
            targetMatrix.m13 = dyp;
            targetMatrix.m23 = dzp;
            targetMatrix.m33 = 1;

            testMatrices.Add(testMatrix);

            var detMatrix = SubdivideMatrices(targetMatrix, testMatrix);
            var det = detMatrix[0, 0] + detMatrix[0, 1] + detMatrix[0, 2] + detMatrix[0, 3] +
                detMatrix[1, 0] + detMatrix[1, 1] + detMatrix[1, 2] + detMatrix[1, 3] +
                detMatrix[2, 0] + detMatrix[2, 1] + detMatrix[2, 2] + detMatrix[2, 3] +
                detMatrix[3, 0] + detMatrix[3, 1] + detMatrix[3, 2] + detMatrix[3, 3];
            if (Mathf.Abs(det) > 0.001f)
            {
                possibleAngles.RemoveAt(i);
                i -= 1;
            };
        }
    }

    /// <summary>
    /// Gets all solutions that are able to be fulfilled in current configuration
    /// </summary>
    /// <param name="visualize">Are we visualizing it on robot mesh?</param>
    private void GetConfigurationSolutions(bool visualize = true)
    {
        if (visualize)
        {
            selectedSolutions.Clear();
        }
        else
        {
            newTargetSolutions.Clear();
        }
        foreach (Angles a in possibleAngles)
        {
            bool addToSolutions = true;
            if (joint1 == JointConfiguration.Minus)
            {
                if (Mathf.Abs(a.angles[0]) <= 90)
                {
                    addToSolutions = false;
                }
            }
            else
            {
                if (Mathf.Abs(a.angles[0]) >= 90)
                {
                    addToSolutions = false;
                }
            }
            if (joint3 == JointConfiguration.Minus)
            {
                if (a.angles[2] <= 90)
                {
                    addToSolutions = false;
                }
            }
            else
            {
                if (a.angles[2] >= 90)
                {
                    addToSolutions = false;
                }
            }
            if (joint5 == JointConfiguration.Minus)
            {
                if (a.angles[4] >= 0)
                {
                    addToSolutions = false;
                }
            }
            else
            {
                if (a.angles[4] <= 0)
                {
                    addToSolutions = false;
                }
            }
            if (addToSolutions)
            {
                if (visualize)
                {
                    selectedSolutions.Add(a);
                }
                else
                {
                    newTargetSolutions.Add(a);
                }
            }
        }
    }

    /// <summary>
    /// Tests if specified target can be reached in other configurations
    /// </summary>
    public void TestIfPossibleToMoveInOtherConfigurations()
    {
        List<JointConfiguration> configuration = new List<JointConfiguration>();
        configuration.Add(joint1);
        configuration.Add(joint3);
        configuration.Add(joint5);
        foreach (Angles a in possibleAngles)
        {
            TestSingleConfiguration(JointConfiguration.Minus, JointConfiguration.Minus, JointConfiguration.Minus);
            TestSingleConfiguration(JointConfiguration.Plus, JointConfiguration.Minus, JointConfiguration.Minus);
            TestSingleConfiguration(JointConfiguration.Plus, JointConfiguration.Plus, JointConfiguration.Minus);
            TestSingleConfiguration(JointConfiguration.Plus, JointConfiguration.Plus, JointConfiguration.Plus);
            TestSingleConfiguration(JointConfiguration.Minus, JointConfiguration.Plus, JointConfiguration.Plus);
            TestSingleConfiguration(JointConfiguration.Minus, JointConfiguration.Minus, JointConfiguration.Plus);
            TestSingleConfiguration(JointConfiguration.Plus, JointConfiguration.Minus, JointConfiguration.Plus);
            TestSingleConfiguration(JointConfiguration.Minus, JointConfiguration.Plus, JointConfiguration.Minus);
        }
        joint1 = configuration[0];
        joint3 = configuration[1];
        joint5 = configuration[2];
    }

    /// <summary>
    /// Tests single configuration if it is possible to reach specified point
    /// </summary>
    /// <param name="joint1">Configuration of first joint</param>
    /// <param name="joint3">Configuration of third joint</param>
    /// <param name="joint5">Configuration of fifth joint</param>
    private void TestSingleConfiguration(JointConfiguration joint1, JointConfiguration joint3, JointConfiguration joint5)
    {
        this.joint1 = joint1;
        this.joint3 = joint3;
        this.joint5 = joint5;
        GetConfigurationSolutions(false);
        if (newTargetSolutions.Count > 0)
        {
            Debug.Log("Can reach point in configuration: " + joint1 + ", " + joint3 + ", " + joint5);
            return;
        }
    }

    /// <summary>
    /// Checks if target position is possible to reach in current configuration
    /// </summary>
    /// <param name="targetPosition">Target position to test</param>
    /// <returns>Can we reach target with current configuration?</returns>
    public bool CheckIfPossibleToMoveTo(Target targetPosition)
    {
        var startPoint = currentTranslation;
        var endPoint = targetPosition;
        Vector3 position = Vector3.zero;
        Vector3 rotation = Vector3.zero;
        float step = 0.01f;
        for (
             position = startPoint.position, rotation = startPoint.rotation;
            Vector3.Distance(position, endPoint.position) >= 0.01f;
            position = Vector3.Lerp(startPoint.position, endPoint.position, step),
            rotation = Vector3.Lerp(startPoint.rotation, endPoint.rotation, step))
        {
            newTargetSolutions.Clear();
            var testTarget = new Target() { position = position, rotation = rotation };
            CalculateAxes(false, testTarget);
            if (newTargetSolutions.Count <= 0)
            {
                Debug.Log("Point not possible to go to in current configuration");
                return false;
            }
            step += 0.01f;
        }
        return true;
    }

    /// <summary>
    /// Tries to get 6th pivot rotation from rotation of axes
    /// </summary>
    /// <param name="axes">List of rotations to calculate from</param>
    /// <returns>Rotations of 6th pivot in eulera angles</returns>
    public Vector3 GetRotationFromAxes(List<float> axes)
    {
        float ax = Cos(axes[0]) * (
               -Sin(axes[1] + axes[2]) * (
               Cos(axes[3]) * Cos(axes[4]) * Cos(axes[5]) - Sin(axes[3]) * Sin(axes[5])
               ) - Cos(axes[1] + axes[2]) * Sin(axes[4]) * Cos(axes[5])
               ) + Sin(axes[0]) * (
               Sin(axes[3]) * Cos(axes[4]) * Cos(axes[5]) + Cos(axes[3]) * Sin(axes[5])
               );

        float bx = Cos(axes[0]) * (
            -Sin(axes[1] + axes[2]) * (
            -Cos(axes[3]) * Cos(axes[4]) * Sin(axes[5]) - Sin(axes[3]) * Cos(axes[5])
            ) + Cos(axes[1] + axes[2]) * Sin(axes[4]) * Sin(axes[5])
            ) + Sin(axes[0]) * (
            -Sin(axes[3]) * Cos(axes[4]) * Sin(axes[5]) + Cos(axes[3]) * Cos(axes[5])
            );

        float cy = Sin(axes[0]) * (
                -Sin(axes[1] + axes[2]) * Cos(axes[3]) * Sin(axes[4]) + Cos(axes[1] + axes[2]) * Cos(axes[4])
                ) - Cos(axes[0]) * Sin(axes[3]) * Sin(axes[4]);

        //float angleZ = 2 * Atan2(Mathf.Sqrt(ax * ax + bx * bx) - ax, bx);
        //float angleY = Mathf.Acos(ax * SecRad(angleZ));
        //float angleX = Mathf.Asin(cy * SecRad(angleY));
        float angleY = (float)Math.Round(Mathf.Asin(-cx), 2); // angle in radians
        float cos2 = (float)Math.Round(Mathf.Cos(angleY), 2); // cos of angle Y
        float sin1 = 0;
        float sin3 = 0;
        if (cos2 != 0)
        {
            sin1 = (float)Math.Round(cy / cos2, 2); // sin of angle X
        }
        float angleX = (float)Math.Round(Mathf.Asin(sin1), 2); // angle in radians
        if (cos2 != 0)
        {
            sin3 = (float)Math.Round(bx / cos2, 2);
        }
        var angleZ = (float)Math.Round(Mathf.Asin(sin3), 2);

        angleX *= Mathf.Rad2Deg;
        angleY *= Mathf.Rad2Deg - 90;
        angleZ *= Mathf.Rad2Deg;

        //Debug.Log("Point rotation in degrees: " + angleX + ", " + angleY + ", " + angleZ);

        return new Vector3(angleX, angleY, angleZ);
    }

    /// <summary>
    /// Testing method for setting up one-time targets
    /// </summary>
    [ContextMenu("Test")]
    private void DebugTest()
    {
        currentTranslation = startTranslation = currentTarget;
    }

    /// <summary>
    /// Coroutine allowing to move robot to a specified point
    /// </summary>
    /// <param name="speed">Speed of movement</param>
    /// <param name="target">Target to move to</param>
    /// <param name="movementType">Type of movement to make</param>
    /// <returns></returns>
    public IEnumerator GoToPoint(float speed, Point target, InstructionMovementType movementType)
    {
        checkDifferences = false;
        if (movementType == InstructionMovementType.LINEAR)
        {
            testSolutions = true;
            interpolationSpeed = 1f / (Vector3.Distance(currentTarget.position, target.position) * 1000 / speed);
            currentTranslation = startTranslation = currentTarget;
            var tempTarget = currentTarget.position;
            currentTarget = new Target(target.position, target.rotation, target.jointAngles);
            RobotData.Instance.CurrentTarget = currentTarget;
            inLinearMoveType = true;
            interpolate = true;
            yield return new WaitForSeconds(Vector3.Distance(tempTarget, target.position) * 1000 / speed);
            interpolate = false;
        }
        else
        {
            testSolutions = false;
            interpolate = true;
            maxDifference = 0;
            inLinearMoveType = false;
            float[] tempArr = new float[6];
            selectedSolutions[0].angles.CopyTo(tempArr);
            startJointAngles = tempArr.ToList();
            targetJointAngles = new List<float>() { target.position.x, target.position.y, target.position.z, target.rotation.x, target.rotation.y, target.rotation.z }; //target
            progress = 0f;
            interpolationSpeed = speed;
            for (int i = 0; i < startJointAngles.Count; i++)
            {
                if (Mathf.Abs(targetJointAngles[i] - startJointAngles[i]) > maxDifference)
                {
                    maxDifference = Mathf.Abs(targetJointAngles[i] - startJointAngles[i]);
                }
            }
            float traverseTime = maxDifference / (interpolationSpeed * RobotData.Instance.MaxRobotJointSpeed);
            Debug.Log(traverseTime);
            yield return new WaitForSeconds(traverseTime);
            interpolate = false;
        }
    }

    /// <summary>
    /// Method attached to changing coordinate system
    /// </summary>
    public void ChangedCoordinateSystem()
    {
        if (RobotData.Instance.MovementType == MovementType.Joint)
        {
            interpolate = false;
            inLinearMoveType = false;
        }
        else
        {
            inLinearMoveType = true;
            interpolate = false;
        }
    }

    /// <summary>
    /// Helper method for calculating sinus with degrees
    /// </summary>
    /// <param name="x">Angle in degrees</param>
    /// <returns>Sinus of specified angle</returns>
    private static float Sin(float x)
    {
        return Mathf.Sin(x * Mathf.Deg2Rad);
    }

    /// <summary>
    /// Helper method for calculating cosinus with degrees
    /// </summary>
    /// <param name="x">Angle in degrees</param>
    /// <returns>Cosinus of specified angle</returns>
    private static float Cos(float x)
    {
        return Mathf.Cos(x * Mathf.Deg2Rad);
    }

    /// <summary>
    /// Helper method for calculating secans with radians
    /// </summary>
    /// <param name="x">Angle in radians</param>
    /// <returns>Secans of specified angle</returns>
    private static float SecRad(float x)
    {
        return 1 / Mathf.Cos(x);
    }

    /// <summary>
    /// Helper method for calculating Arcus Tangens with degrees
    /// Clamps result to positive part of circle (0-360 degrees)
    /// </summary>
    /// <param name="y">Variable in radians</param>
    /// <param name="x">Variable in radians</param>
    /// <returns>Arcus Tangens of specified variables clamped to 0-360 degrees</returns>
    private static float Atan2(float y, float x)
    {
        var angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        return angle;
    }

    /// <summary>
    /// Helper method for subdividing matrices
    /// </summary>
    /// <param name="matrix1">Matrix to subdivide from</param>
    /// <param name="matrix2">Matrix to subdivide</param>
    /// <returns>Subdivided matrix</returns>
    private Matrix4x4 SubdivideMatrices(Matrix4x4 matrix1, Matrix4x4 matrix2)
    {
        Matrix4x4 returnable = Matrix4x4.zero;
        returnable.m00 = matrix1.m00 - matrix2.m00;
        returnable.m01 = matrix1.m01 - matrix2.m01;
        returnable.m02 = matrix1.m02 - matrix2.m02;
        returnable.m03 = matrix1.m03 - matrix2.m03;
        returnable.m10 = matrix1.m10 - matrix2.m10;
        returnable.m11 = matrix1.m11 - matrix2.m11;
        returnable.m12 = matrix1.m12 - matrix2.m12;
        returnable.m13 = matrix1.m13 - matrix2.m13;
        returnable.m20 = matrix1.m20 - matrix2.m20;
        returnable.m21 = matrix1.m21 - matrix2.m21;
        returnable.m22 = matrix1.m22 - matrix2.m22;
        returnable.m23 = matrix1.m23 - matrix2.m23;
        returnable.m30 = matrix1.m30 - matrix2.m30;
        returnable.m31 = matrix1.m31 - matrix2.m31;
        returnable.m32 = matrix1.m32 - matrix2.m32;
        returnable.m33 = matrix1.m33 - matrix2.m33;
        return returnable;
    }
#pragma warning restore CS0219
}
