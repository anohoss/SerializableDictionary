using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

internal static class UnitySerializableExtension
{
    public static bool IsUnitySerializable(this Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (type.IsSubclassOf(typeof(UnityObject)))
        {
            return true;
        }

        if (type.IsSerializable)
        {
            return type != typeof(decimal);
        }

        return type == typeof(AnimationCurve)
            || type == typeof(Color)
            || type == typeof(Bounds)
            || type == typeof(BoundsInt)
            || type == typeof(LayerMask)
            || type == typeof(Quaternion)
            || type == typeof(Rect)
            || type == typeof(RectInt)
            || type == typeof(Vector2)
            || type == typeof(Vector2Int)
            || type == typeof(Vector3)
            || type == typeof(Vector3Int)
            || type == typeof(Vector4);
    }
}
