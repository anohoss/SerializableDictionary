using System;
using System.Collections.Generic;
using UnityEngine;

public partial class SerializableDictionary<TKey, TValue>: ISerializationCallbackReceiver
{
    // Serializable TKey/TValue pair
    [Serializable]
    private class KeyValuePair
    {
        [SerializeField]
        public TKey Key;

        [SerializeField]
        public TValue Value;

        public KeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public KeyValuePair(KeyValuePair pair) : this(pair.Key, pair.Value) { }
    }


    
    // Serialized Key/Value pair.
    // 
    // This stores the same data inside the Dictionary.
    // If you want to change the data in this field, Call following functions instead of changing it directly.
    // - this[TKey]
    // - Add(TKey, TValue)
    // - Remove(TKey)
    // - Clear
    [SerializeField]
    private List<KeyValuePair> _pairs;



    private void InitSerializedProperty()
    {
        _pairs = new List<KeyValuePair>();

        foreach (var pair in _dictionary)
        {
            _pairs.Add(new KeyValuePair(pair.Key, pair.Value));
        }
    }



    private void UpdateSerializedProperty()
    {
        _pairs.Clear();

        foreach(var pair in _dictionary)
        {
            _pairs.Add(new KeyValuePair(pair.Key, pair.Value));
        }
    }



    public void OnBeforeSerialize() { }



    public void OnAfterDeserialize()
    {
        _dictionary.Clear();

        for (int i = 0; i < _pairs.Count; i++)
        {
            if (_dictionary.ContainsKey(_pairs[i].Key))
            {
                continue;
            }

            _dictionary.Add(_pairs[i].Key, _pairs[i].Value);
        }
    }
}
