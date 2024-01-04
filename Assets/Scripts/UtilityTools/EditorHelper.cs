using System.Reflection;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class EditorHelper
{

    public static object GetPropertyTargetObject(SerializedProperty property)
    {

        if (property == null)
        {
            return null;
        }

        var path = property.propertyPath;
        var targetObject = property.serializedObject.targetObject;

        var value = GetPropertyValue(targetObject, path);

        return value;
    }

    private static object GetPropertyValue(object obj, string path)
    {
        if (obj == null)
        {
            return null;
        }

        var type = obj.GetType();

        while (type != null)
        {
            var field = type.GetField(path, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (field != null)
            {
                return field.GetValue(obj);
            }

            var prop = type.GetProperty(path, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop != null)
            {
                return prop.GetValue(obj, null);
            }

            type = type.BaseType;
        }

        return null;
    }
}

#endif