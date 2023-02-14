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
            public const string PAIRS = "_pairs";

            public const string KEY = "Key";

            public const string VALUE = "Value";
        }



        private readonly SerializedProperty _property;

        public SerializedObject SerializedObject => _property?.serializedObject;



        public SerializableDictionaryProperty(SerializedProperty property)
        {
            _property = property;
        }



        public SerializedProperty GetPairs()
        {
            return _property.FindPropertyRelative(Constraints.PAIRS);
        }



        /// <summary>
        /// Get index-th key from<see cref="SerializableDictionary{TKey, TValue}._pairs"/>
        /// </summary>
        public SerializedProperty GetKey(int index)
        {
            var pairs = GetPairs();
            var element = pairs.GetArrayElementAtIndex(index);
            return element.FindPropertyRelative(Constraints.KEY);
        }



        /// <summary>
        /// Get index-th value from <see cref="SerializableDictionary{TKey, TValue}._pairs"/>
        /// </summary>
        public SerializedProperty GetValue(int index)
        {
            var pairs = GetPairs();
            var element = pairs.GetArrayElementAtIndex(index);
            return element.FindPropertyRelative(Constraints.VALUE);
        }
    }



    private class Styles
    {
        public readonly GUIStyle DuplicatedKeyStyle;

        public readonly GUIContent DuplicatedKeyContent;

        public readonly GUIContent KeyContent;

        public readonly GUIContent ValueContent;

        public Styles()
        {
            DuplicatedKeyStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                normal = { textColor = Color.red }
            };

            DuplicatedKeyContent = EditorGUIUtility.TrTextContent("Key (Duplicated)");
            KeyContent = EditorGUIUtility.TrTextContent("Key");
            ValueContent = EditorGUIUtility.TrTextContent("Value");
        }
    }



    SerializableDictionaryProperty _property;

    Styles _styles;

    ReorderableList _list;



    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Init(property);

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

        position.height = EditorGUIUtility.singleLineHeight;
        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(position, property.isExpanded, label);
        position.y += position.height;

        if (property.isExpanded)
        {
            position.y += EditorGUIUtility.standardVerticalSpacing;     // spacing betweeen foldout and dictionary
            _list.DoList(position);
        }

        EditorGUI.EndFoldoutHeaderGroup();
    }



    public void Init(SerializedProperty property)
    {
        _styles ??= new Styles();
        _property ??= new SerializableDictionaryProperty(property);
        _list ??= _list = new ReorderableList(_property.SerializedObject, _property.GetPairs(), true, false, true, true)
        {
            elementHeightCallback = CallOnElementHeight,
            drawElementCallback = CallOnDrawElement,
        };
    }



    private void CallOnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        var labelWidth = GetElementLabelWidth(rect);

        var key = _property.GetKey(index);
        rect.height = EditorGUI.GetPropertyHeight(key);


        // Draw Key Property
        EditorGUI.LabelField(new Rect(rect.x + 8f, rect.y, labelWidth, rect.height), _styles.KeyContent);
        EditorGUI.PropertyField(new Rect(rect.x + labelWidth + 3f, rect.y, rect.width - labelWidth - 3f, rect.height), key, GUIContent.none);
        
        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

        var value = _property.GetValue(index);
        rect.height = EditorGUI.GetPropertyHeight(value);

        // Draw Value Property
        EditorGUI.LabelField(new Rect(rect.x + 8f, rect.y, labelWidth, rect.height), _styles.ValueContent);
        EditorGUI.PropertyField(new Rect(rect.x + labelWidth + 3f, rect.y, rect.width - labelWidth - 3f, rect.height), value, GUIContent.none);
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
