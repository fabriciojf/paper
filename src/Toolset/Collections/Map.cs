using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Collections
{
  public class Map<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
  {
    private readonly Dictionary<TKey, TValue> map;

    public Map()
    {
      map = new Dictionary<TKey, TValue>();
    }

    public Map(int capacity)
    {
      map = new Dictionary<TKey, TValue>(capacity);
    }

    public Map(IDictionary<TKey, TValue> entries)
    {
      map = new Dictionary<TKey, TValue>(entries);
    }

    public Map(IEnumerable<KeyValuePair<TKey, TValue>> entries)
    {
      map = new Dictionary<TKey, TValue>();
      foreach (var entry in entries)
      {
        map[entry.Key] = entry.Value;
      }
    }

    public TValue this[TKey key]
    {
      get => map.ContainsKey(key) ? map[key] : default(TValue);
      set => map[key] = value;
    }

    public int Count => map.Count;

    public ICollection<TKey> Keys => map.Keys;

    public ICollection<TValue> Values => map.Values;

    public void Add(TKey key, TValue value)
    {
      map[key] = value;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
      map[item.Key] = item.Value;
    }

    public void AddMany(IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
      items.ForEach(item => map[item.Key] = item.Value);
    }

    public void Clear()
    {
      map.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      return ((IDictionary<TKey, TValue>)map).Contains(item);
    }

    public bool ContainsKey(TKey key)
    {
      return map.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      ((IDictionary<TKey, TValue>)map).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      return ((IDictionary<TKey, TValue>)map).GetEnumerator();
    }

    public bool Remove(TKey key)
    {
      return map.Remove(key);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      return ((IDictionary<TKey, TValue>)map).Remove(item);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      return map.TryGetValue(key, out value);
    }

    public bool IsReadOnly => ((IDictionary)map).IsReadOnly;

    #region Implementação explícita

    object IDictionary.this[object key]
    {
      get => ((IDictionary)this)[key];
      set => ((IDictionary)this)[key] = value;
    }

    bool IDictionary.IsFixedSize => ((IDictionary)map).IsFixedSize;

    ICollection IDictionary.Keys => ((IDictionary)map).Keys;

    ICollection IDictionary.Values => ((IDictionary)map).Values;

    bool ICollection.IsSynchronized => ((IDictionary)map).IsSynchronized;

    object ICollection.SyncRoot => ((IDictionary)map).SyncRoot;

    void IDictionary.Add(object key, object value) => ((IDictionary)map).Add(key, value);

    void IDictionary.Clear() => ((IDictionary)map).Clear();

    bool IDictionary.Contains(object key) => ((IDictionary)map).Contains(key);

    void ICollection.CopyTo(Array array, int index) => ((IDictionary)map).CopyTo(array, index);

    IEnumerator IEnumerable.GetEnumerator() => ((IDictionary)map).GetEnumerator();

    IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary)map).GetEnumerator();

    void IDictionary.Remove(object key) => ((IDictionary)map).Remove(key);

    #endregion
  }
}