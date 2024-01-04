using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Allows checking of robot movement for tasks
/// </summary>
public class RobotMovementChecker : StepEnabler
{
    [Header("Robot Movement Checker")]

    /// <summary>
    /// Which axes (from requiredAxisID) should be clamped to reach Enabled state
    /// </summary>
    [SerializeField] private axisLimitRequirement conditionType;
    /// <summary>
    /// Which axes should be checked in this step? (if list is empty then every axis will be checked)
    /// </summary>
    [SerializeField] private List<int> requiredAxisID;

    /// <summary>
    /// Array of axes' clamped state (same index as originalAxisMaterialList)
    /// </summary>
    private bool[] limitArray = default;
    /// <summary>
    /// List of transforms and original materials of each axis (same index as limitArray)
    /// </summary>
    public List<Tuple<Transform, Material>> originalAxisMaterialList = default;

    /// <summary>
    /// Enum of logical combinator for axis clamped state
    /// </summary>
    private enum axisLimitRequirement
    {
        ALL,
        ANY,
        NONE
    }

    /// <summary>
    /// Initialize methode in which original axes state is saved, starting limit reached (clapmed) state are checked and if any is already reached, then it is painted
    /// </summary>
    protected override void initialize()
    {
        originalAxisMaterialList = new List<Tuple<Transform, Material>>();

        bool[] initialLimitsArray;
        Transform axisTransform;

        initialLimitsArray = ControllersManager.Instance.GetController<RobotController>().CurrentRobot.gameObject.GetComponent<JointRobotAngles>().GetPivotsLimitArray();
        limitArray = new bool[initialLimitsArray.Length];

        for (int i = 0; i < initialLimitsArray.Length; i++)
        {
            axisTransform = ControllersManager.Instance.GetController<RobotController>().CurrentRobot.gameObject.GetComponent<ClampRobotLimitAngle>().GetAxisTransform(i);
            originalAxisMaterialList.Add(new Tuple<Transform, Material>(axisTransform, new Material(axisTransform.GetComponent<MeshRenderer>().materials[0])));
            limitArray[i] = initialLimitsArray[i];

            if (initialLimitsArray[i] == true)
            {
                paintLimitAxis(i);
            }
        }

        ClampRobotLimitAngle.newLimitReached += limitReached;
        ClampRobotLimitAngle.newLimitReset += limitReset;

        checkStepComplete();
    }

    /// <summary>
    /// Action that is invoked when axis limit is reached
    /// </summary>
    /// <param name="axisID">Index of clamped axis (same index is used for both limitArray and originalAxisMaterialList)</param>
    private void limitReached(int axisID)
    {
        if (limitArray[axisID] != true)
        {
            limitArray[axisID] = true;
            paintLimitAxis(axisID);
        }
        checkStepComplete();
    }

    /// <summary>
    /// Action that is invoked when axis limit is reset
    /// </summary>
    /// <param name="axisID">Index of freed axis (same index is used for both limitArray and originalAxisMaterialList)</param>
    private void limitReset(int axisID)
    {
        if (limitArray[axisID] != false)
        {
            limitArray[axisID] = false;
            cleanLimitAxis(axisID);
        }
        checkStepComplete();
    }

    /// <summary>
    /// Paints particular joint on red (limit reached color)
    /// </summary>
    /// <param name="axisID">Index of axis to paint (same index is used for both limitArray and originalAxisMaterialList)</param>
    private void paintLimitAxis(int axisID)
    {
        originalAxisMaterialList[axisID].Item1.GetComponent<MeshRenderer>().materials[0].color = Color.red;
    }

    /// <summary>
    /// Paints particular joint on original color
    /// </summary>
    /// <param name="axisID">Index of axis to clean (same index is used for both limitArray and originalAxisMaterialList)</param>
    private void cleanLimitAxis(int axisID)
    {
        originalAxisMaterialList[axisID].Item1.GetComponent<MeshRenderer>().materials[0].color = originalAxisMaterialList[axisID].Item2.color;
    }

    /// <summary>
    /// Clears all painted joints 
    /// </summary>
    public void CleanMaterials()
    {
        foreach (var l in originalAxisMaterialList)
        {
            l.Item1.GetComponent<MeshRenderer>().materials[0].color = l.Item2.color;
        }
    }

    /// <summary>
    /// Runs a test to check if step conditions are met
    /// </summary>
    private void checkStepComplete()
    {
        bool pass = true;
        switch (conditionType)
        {
            case axisLimitRequirement.ALL:
                if (requiredAxisID.Count > 0)
                {
                    foreach (int ID in requiredAxisID)
                    {
                        if (limitArray[ID] == false)
                            pass = false;
                    }
                }
                else
                {
                    if (limitArray.Any((x) => x == false))
                        pass = false;
                }
                break;

            case axisLimitRequirement.ANY:
                if (requiredAxisID.Count > 0)
                {
                    pass = false;
                    foreach (int ID in requiredAxisID)
                    {
                        if (limitArray[ID] == true)
                            pass = true;
                    }
                }
                else
                {
                    pass = false;
                    if (limitArray.Any((x) => x == true))
                        pass = true;
                }
                break;

            case axisLimitRequirement.NONE:
                if (requiredAxisID.Count > 0)
                {
                    foreach (int ID in requiredAxisID)
                    {
                        if (limitArray[ID] == true)
                            pass = false;
                    }
                }
                else
                {
                    if (limitArray.Any((x) => x == true))
                        pass = false;
                }
                break;

            default:
                Debug.Log("Unkown LogicOperationType: " + conditionType);
                pass = false;
                break;
        }
        if (pass == true && Enabled != true)
            Enabled = true;
        else if (pass == false && Enabled != false)
            Enabled = false;
    }

    /// <summary>
    /// CleanUp methode, returns joints their original colors
    /// </summary>
    protected override void cleanUp()
    {
        ClampRobotLimitAngle.newLimitReached -= limitReached;
        ClampRobotLimitAngle.newLimitReset -= limitReset;
        CleanMaterials();
    }
}


