using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

public class ReadOnlyAttribute : PropertyAttribute
{

}

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        EditorGUI.indentLevel = 0;
        var indent = EditorGUI.indentLevel;

        var valueRect = new Rect(position.x, position.y, position.width, position.height);

        EditorGUI.LabelField(valueRect, EditorHelper.GetPropertyTargetObject(property).ToString());

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}

#endif
