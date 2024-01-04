using System;
using UnityEngine;

/// <summary>
/// Mark a method with an integer argument with this to display the argument as an enum popup in the UnityEvent
/// drawer. Use: [EnumAction(typeof(SomeEnumType))]
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class EnumActionAttribute : PropertyAttribute
{
    /// <summary>
    /// Type of enum
    /// </summary>
    public Type enumType;

    /// <summary>
    /// Attribute method allowing for casting int
    /// </summary>
    /// <param name="enumType">Type of enum to cast</param>
    public EnumActionAttribute(Type enumType)
    {
        this.enumType = enumType;
    }
}