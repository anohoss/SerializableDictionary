using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>
{
    Dictionary<TKey, TValue> _dictionary;

    public TValue this[TKey key]
    {
        get => _dictionary[key];
        set => _dictionary[key] = value;
    }

    public IEqualityComparer<TKey> Comparer => _dictionary.Comparer;
    
    public Dictionary<TKey, TValue>.KeyCollection Keys => _dictionary.Keys;

    public Dictionary<TKey, TValue>.ValueCollection Values => _dictionary.Values;


    object IDictionary.this[object key]
    {
        get => (_dictionary as IDictionary)[key];
        set => (_dictionary as IDictionary)[key] = value;
    }

    public int Count => _dictionary.Count;

    bool IDictionary.IsFixedSize => (_dictionary as IDictionary).IsFixedSize;

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => (_dictionary as ICollection<KeyValuePair<TKey, TValue>>).IsReadOnly;

    bool IDictionary.IsReadOnly => (_dictionary as IDictionary).IsReadOnly;

    bool ICollection.IsSynchronized => (_dictionary as IDictionary).IsSynchronized;

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

    ICollection IDictionary.Keys => Keys;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

    object ICollection.SyncRoot => (_dictionary as IDictionary).SyncRoot;

    ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

    ICollection IDictionary.Values => Values;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;



    public void Add(TKey key, TValue value)
    {
        _dictionary.Add(key, value);
    }

    public void Clear()
    {
        _dictionary.Clear();
    }

    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }

    public bool ContainsValue(TValue value)
    {
        return _dictionary.ContainsValue(value);
    }

    public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    public bool Remove(TKey key)
    {
        return _dictionary.Remove(key);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        (_dictionary as ICollection<KeyValuePair<TKey, TValue>>).Add(item);
    }

    void IDictionary.Add(object key, object value)
    {
        (_dictionary as IDictionary).Add(key, value);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
        return (_dictionary as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
    }

    bool IDictionary.Contains(object key)
    {
        return (_dictionary as IDictionary).Contains(key);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        (_dictionary as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        (_dictionary as ICollection).CopyTo(array, index);
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
        return (_dictionary as IEnumerable<KeyValuePair<TKey, TValue>>).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return (_dictionary as IEnumerable).GetEnumerator();
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
        return (_dictionary as IDictionary).GetEnumerator();
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        return (_dictionary as ICollection<KeyValuePair<TKey, TValue>>).Remove(item);
    }

    void IDictionary.Remove(object key)
    {
        (_dictionary as IDictionary).Remove(key);
    }
}
