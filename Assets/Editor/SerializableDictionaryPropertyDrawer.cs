using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
public class SerializableDictionaryPropertyDrawer : PropertyDrawer
{
    private class SerializableDictionaryProperty
    {
        private static class Constraints
        {
            public const string PAIRS_FIELD_NAME = "_pairs";

            public const string KEY_FIELD_NAME = "Key";

            public const string VALUE_FIELD_NAME = "Value";
        }



        private readonly bool _isSerializable;

        private readonly SerializedProperty _list;

        private readonly SerializedProperty _property;

        public int Count
        {
            get
            {
                return _list.arraySize;
            }
        }

        public bool IsSerializable => _isSerializable;

        public SerializedProperty List => _list;

        public SerializedProperty Property => _property;

        public SerializedObject SerializedObject => _property?.serializedObject;



        public SerializableDictionaryProperty(SerializedProperty property)
        {
            if(property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            _property = property;
            _list = property.FindPropertyRelative(Constraints.PAIRS_FIELD_NAME);

            var targetObj = property.GetTargetObject();
            for (var type = targetObj.GetType(); ;type = type.BaseType)
            {
                if(type.BaseType == typeof(object)) // type is SerializableDictionary' 2
                {
                    _isSerializable = type.GenericTypeArguments[0].IsUnitySerializable() && type.GenericTypeArguments[1].IsUnitySerializable();
                    break;
                }
            }
        }



        public bool ContainsKey(SerializedProperty key)
        {
            dynamic obj = _property.GetTargetObject();   // type is SerializableDictionary` 2 or its derivatives
            dynamic keyObj = key.GetTargetObject();
            return obj.ContainsKey(keyObj);
        }



        /// <summary>
        /// Get index-th key from<see cref="SerializableDictionary{TKey, TValue}._pairs"/>
        /// </summary>
        public SerializedProperty GetKey(int index)
        {
            var element = _list.GetArrayElementAtIndex(index);
            return element?.FindPropertyRelative(Constraints.KEY_FIELD_NAME);
        }



        /// <summary>
        /// Get index-th value from <see cref="SerializableDictionary{TKey, TValue}._pairs"/>
        /// </summary>
        public SerializedProperty GetValue(int index)
        {
            var element = _list.GetArrayElementAtIndex(index);
            return element?.FindPropertyRelative(Constraints.VALUE_FIELD_NAME);
        }



        public int IndexOfKey(SerializedProperty property)
        {
            dynamic targetObj = _property.GetTargetObject();
            // propertyの値と同値のキーを検索する
            for (int i = 0; i < _list.arraySize; i++)
            {
                SerializedProperty key = GetKey(i);
                if (targetObj.Comparer.Equals((dynamic)key.GetTargetObject(), (dynamic)property.GetTargetObject()))
                {
                    return i;
                }
            }

            return -1;
        }



        public bool IsUniqueKey(int index)
        {
            int firstIndex = IndexOfKey(GetKey(index));
            return firstIndex >= index; // List<KeyValuePair>の中で同値のKeyがある場合、最初に出現するKeyのみをユニークキーとみなす。
        } 
    }



    private class Styles
    {
        public readonly GUIStyle DuplicatedKeyFoldoutStyle;

        public readonly GUIStyle DuplicatedKeyStyle;

        public readonly GUIContent DuplicatedKeyContent;

        public readonly GUIContent KeyContent;

        public readonly GUIContent ValueContent;

        public Styles()
        {
            DuplicatedKeyStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                normal = { textColor = Color.red },
            };

            DuplicatedKeyFoldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                normal = { textColor = Color.red },
                onNormal = { textColor = Color.red },
                fontStyle = FontStyle.Bold,
            };

            KeyContent = EditorGUIUtility.TrTextContent("Key");
            DuplicatedKeyContent = EditorGUIUtility.TrTextContent("Key (Duplicated)");
            ValueContent = EditorGUIUtility.TrTextContent("Value");
        }
    }



    SerializableDictionaryProperty _property;

    Styles _styles;

    ReorderableList _list;



    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Init(property);

        if (!_property.IsSerializable)
        {
            return 0;
        }

        float height = 0f;
        // Foldout
        height += EditorGUIUtility.singleLineHeight;
        // spacing + Dictionary
        height += property.isExpanded ? EditorGUIUtility.standardVerticalSpacing + _list.GetHeight() : 0f;
        return height;
    }



    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Init(property);

        if (!_property.IsSerializable)
        {
            return;
        }

        position.height = EditorGUIUtility.singleLineHeight;
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
        position.y += position.height;

        if (property.isExpanded)
        {
            position.y += EditorGUIUtility.standardVerticalSpacing;     // spacing betweeen foldout and dictionary
            _list.DoList(position);
        }
    }



    public void Init(SerializedProperty property)
    {
        _styles ??= new Styles();
        _property ??= new SerializableDictionaryProperty(property);
        _list ??= _list = new ReorderableList(_property.SerializedObject, _property.List, true, false, true, true)
        {
            elementHeightCallback = CallOnElementHeight,
            drawElementCallback = CallOnDrawElement,
        };
    }



    private void CallOnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        var labelWidth = GetElementLabelWidth(rect);
        var key = _property.GetKey(index);
        var isUniqueKey = _property.IsUniqueKey(index);
        var keyContent = isUniqueKey ? _styles.KeyContent : _styles.DuplicatedKeyContent;

        // Draw Key Property
        if (key.hasVisibleChildren)
        {
            key.isExpanded = EditorGUI.Foldout(new Rect(rect.x + 8f, rect.y, rect.width - 8f, EditorGUIUtility.singleLineHeight), key.isExpanded, keyContent, isUniqueKey ? EditorStyles.foldout : _styles.DuplicatedKeyFoldoutStyle);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (key.isExpanded)
            {
                var prop = key.Copy();
                EditorGUI.indentLevel += 1;

                while (prop.NextVisible(true) && prop.depth == key.depth + 1)
                {
                    var height = EditorGUI.GetPropertyHeight(prop);
                    EditorGUI.PropertyField(new Rect(rect.x + 8f, rect.y, rect.width - 8f, height), prop, true);
                    rect.y += height + EditorGUIUtility.standardVerticalSpacing;
                }

                prop.Dispose();
                EditorGUI.indentLevel -= 1;
            }
        }
        else
        {
            var height = EditorGUI.GetPropertyHeight(key);

            EditorGUI.LabelField(new Rect(rect.x + 8f, rect.y, labelWidth, height), keyContent, isUniqueKey ? EditorStyles.label : _styles.DuplicatedKeyStyle);
            EditorGUI.PropertyField(new Rect(rect.x + labelWidth + 3f, rect.y, rect.width - labelWidth - 3f, height), key, GUIContent.none);

            rect.y += height + EditorGUIUtility.standardVerticalSpacing;
        }

        var value = _property.GetValue(index);
        var valueHeight = EditorGUI.GetPropertyHeight(value);

        // Draw Value Property
        if (value.hasVisibleChildren)
        {
            EditorGUI.PropertyField(new Rect(rect.x + 8f, rect.y, rect.width - 8f, valueHeight), value, true);
        }
        else
        {
            EditorGUI.LabelField(new Rect(rect.x + 8f, rect.y, labelWidth, valueHeight), _styles.ValueContent);
            EditorGUI.PropertyField(new Rect(rect.x + labelWidth + 3f, rect.y, rect.width - labelWidth - 3f, valueHeight), value, GUIContent.none);
        }
    }






    private float CallOnElementHeight(int index)
    {
        SerializedProperty key = _property.GetKey(index);
        SerializedProperty value = _property.GetValue(index);

        return EditorGUI.GetPropertyHeight(key) + EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(value);
    }



    private float GetElementLabelWidth(Rect rect)
    {
        return rect.width * 0.45f - 35;
    }
}
