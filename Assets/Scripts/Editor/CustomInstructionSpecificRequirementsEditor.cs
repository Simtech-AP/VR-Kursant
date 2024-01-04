using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[UnityEditor.CustomEditor(typeof(InstructionSpecificRequirements), true)]
[CanEditMultipleObjects()]
public class CustomInstructionSpecificRequirementsEditor : Editor
{

    private string[] _hiddenProperties;

    protected virtual void OnEnable()
    {
        var tp = this.target.GetType();
        var arr = tp.GetCustomAttributes(typeof(HidePropertiesInInspectorAttribute), true) as HidePropertiesInInspectorAttribute[];
        if (arr != null && arr.Length > 0)
        {
            var set = new HashSet<string>();
            foreach (var a in arr)
            {
                foreach (var p in a.HiddenProperties)
                {
                    set.Add(p);
                }
            }
            _hiddenProperties = new string[set.Count];
            set.CopyTo(_hiddenProperties);
        }
    }

    public override void OnInspectorGUI()
    {
        this.DrawDefaultInspector();
    }

    public new bool DrawDefaultInspector()
    {
        //draw properties
        this.serializedObject.Update();
        var result = DrawDefaultInspectorExcept(this.serializedObject, _hiddenProperties);
        this.serializedObject.ApplyModifiedProperties();

        return result;
    }

    #region Static Interface

    public static bool DrawDefaultInspectorExcept(SerializedObject serializedObject, params string[] propsNotToDraw)
    {
        if (serializedObject == null) throw new System.ArgumentNullException("serializedObject");

        EditorGUI.BeginChangeCheck();
        var iterator = serializedObject.GetIterator();
        for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
        {
            if (propsNotToDraw == null || !propsNotToDraw.Contains(iterator.name))
            {
                EditorGUILayout.PropertyField(iterator, true);
            }
        }
        return EditorGUI.EndChangeCheck();
    }

    #endregion

}