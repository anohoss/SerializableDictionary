using System;
using System.Collections;
using System.Reflection;
using UnityEditor;

internal static class PropertyDrawerExtension
{
    public static object GetTargetObject(this SerializedProperty property)
    {
        if (property is null)
        {
            return null;
        }

        // If the property passed is an array element or its child properties, the path to the property is as follows.
        //      {name of array property}.Array.data[0].{name of property}
        // 
        // Therefore, convert this path as follows.
        //      {name of array property}.[0].{name of property}
        string[] fieldNames = property.propertyPath.Replace("Array.data", "").Split('.');

        object targetObj = property.serializedObject.targetObject;
        for (int i = 0; i < fieldNames.Length; i++)
        {
            var fieldName = fieldNames[i];
            if (fieldName[0] is '[')    // element of array
            {
                var index = Convert.ToInt32(fieldName.Substring(1, fieldName.Length - 2));
                if (targetObj is IEnumerable enumerable)
                {
                    var enumerator = enumerable.GetEnumerator();
                    while (index-- >= 0 && enumerator.MoveNext()) { }

                    targetObj = enumerator.Current;
                    continue;
                }
            }
            else
            {
                for (var type = targetObj?.GetType(); type != null; type = type.BaseType)
                {
                    var fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fieldInfo != null)
                    {
                        targetObj = fieldInfo.GetValue(targetObj);
                        break;
                    }

                    var propInfo = type.GetProperty(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (propInfo != null)
                    {
                        targetObj = propInfo.GetValue(targetObj);
                        break;
                    }
                }
            }
        }

        return targetObj;
    }
}
