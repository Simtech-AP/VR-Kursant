using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
/// <summary>
/// Helper class for resetting certain module data
/// </summary>
[CustomEditor(typeof(CellStateReseter))]
public class ScenarioResetData : Editor
{
    /// <summary>
    /// Reference to entrance of a cell
    /// </summary>
    private SerializedProperty cellEntrance;
    /// <summary>
    /// Reference to bumper cap on scene
    /// </summary>
    private SerializedProperty bumperCap;
    /// <summary>
    /// Reference to padlock on scene
    /// </summary>
    private SerializedProperty padLock;
    /// <summary>
    /// Reference to cjurrently used robot
    /// </summary>
    private SerializedProperty robot;

    private SerializedProperty estops;
    /// <summary>
    /// Style of GUI elements
    /// </summary>
    private GUIStyle gUIStyle;

    /// <summary>
    /// Gui elemnt builder
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        gUIStyle = new GUIStyle();
        gUIStyle.fontSize = 12;
        gUIStyle.richText = true;

        cellEntrance = serializedObject.FindProperty("cellEntranceReset");
        bumperCap = serializedObject.FindProperty("bumperCapReset");
        padLock = serializedObject.FindProperty("padLockReset");
        robot = serializedObject.FindProperty("robotReset");
        estops = serializedObject.FindProperty("eStopsState");

        serializedObject.Update();

        EditorGUILayout.Space();
        CellToggle(cellEntrance);
        EditorGUILayout.Space();
        BumperCapToggle(bumperCap);
        EditorGUILayout.Space();
        PadLockToggle(padLock);
        EditorGUILayout.Space();
        RobotToggle(robot);
        EditorGUILayout.Space();
        EStopsToggle();

        serializedObject.ApplyModifiedProperties();
    }

    private void EStopsToggle()
    {
        EditorGUILayout.LabelField("<b>EStops State: </b>", gUIStyle);
        EditorGUILayout.PropertyField(estops.FindPropertyRelative("OnCasette"));
        EditorGUILayout.PropertyField(estops.FindPropertyRelative("OnLocker"));
    }

    /// <summary>
    /// Toggles visual robot state
    /// </summary>
    /// <param name="robot">Referenced robot object</param>
    private void RobotToggle(SerializedProperty robot)
    {
        EditorGUILayout.LabelField("<b>Robot State: </b>", gUIStyle);
        EditorGUILayout.PropertyField(robot.FindPropertyRelative("Stopped"));
        EditorGUILayout.PropertyField(robot.FindPropertyRelative("HomePosition"));
    }

    /// <summary>
    /// Toggles visual padlock state
    /// </summary>
    /// <param name="padLockProperty">Referenced padlock object</param>
    private void PadLockToggle(SerializedProperty padLockProperty)
    {
        EditorGUILayout.LabelField("<b>Pad Lock State: </b>", gUIStyle);
        EditorGUILayout.PropertyField(padLockProperty.FindPropertyRelative("OnDoor"));
        if (padLockProperty.FindPropertyRelative("OnDoor").boolValue)
        {
            padLockProperty.FindPropertyRelative("InLockBox").boolValue = !padLockProperty.FindPropertyRelative("OnDoor").boolValue;
            padLockProperty.FindPropertyRelative("State").intValue = 2;
        }
        EditorGUILayout.PropertyField(padLockProperty.FindPropertyRelative("InLockBox"));
        if (padLockProperty.FindPropertyRelative("InLockBox").boolValue)
        {
            padLockProperty.FindPropertyRelative("OnDoor").boolValue = !padLockProperty.FindPropertyRelative("InLockBox").boolValue;
            padLockProperty.FindPropertyRelative("State").intValue = 0;
        }
    }

    /// <summary>
    /// Toggles visual bumpercap state
    /// </summary>
    /// <param name="padLockProperty">Referenced bumpercap object</param>
    private void BumperCapToggle(SerializedProperty bumperCapProperty)
    {
        EditorGUILayout.LabelField("<b>Bumper Cap State: </b>", gUIStyle);
        EditorGUILayout.PropertyField(bumperCapProperty.FindPropertyRelative("OnDoor"));
        if (bumperCapProperty.FindPropertyRelative("OnDoor").boolValue)
        {
            bumperCapProperty.FindPropertyRelative("OnHead").boolValue = !bumperCapProperty.FindPropertyRelative("OnDoor").boolValue;
            bumperCapProperty.FindPropertyRelative("State").intValue = 0;
        }
        EditorGUILayout.PropertyField(bumperCapProperty.FindPropertyRelative("OnHead"));
        if (bumperCapProperty.FindPropertyRelative("OnHead").boolValue)
        {
            bumperCapProperty.FindPropertyRelative("OnDoor").boolValue = !bumperCapProperty.FindPropertyRelative("OnHead").boolValue;
            bumperCapProperty.FindPropertyRelative("State").intValue = 2;
        }
    }

    /// <summary>
    /// Toggles visual cell state
    /// </summary>
    /// <param name="padLockProperty">Referenced cell object</param>
    private void CellToggle(SerializedProperty cellStateReseter)
    {
        EditorGUILayout.LabelField("<b>Cell Entrance State: </b>", gUIStyle);
        EditorGUILayout.PropertyField(cellStateReseter.FindPropertyRelative("Opened"));
        if (cellStateReseter.FindPropertyRelative("Opened").boolValue)
        {
            cellStateReseter.FindPropertyRelative("Closed").boolValue = !cellStateReseter.FindPropertyRelative("Opened").boolValue;
            cellStateReseter.FindPropertyRelative("State").intValue = 2;
        }
        EditorGUILayout.PropertyField(cellStateReseter.FindPropertyRelative("Closed"));
        if (cellStateReseter.FindPropertyRelative("Closed").boolValue)
        {
            cellStateReseter.FindPropertyRelative("Opened").boolValue = !cellStateReseter.FindPropertyRelative("Closed").boolValue;
            cellStateReseter.FindPropertyRelative("State").intValue = 0;
        }
    }
}
#endif