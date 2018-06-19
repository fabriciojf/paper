using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Collections
{
  public class Map<TKey, TValue> : IDictionary<TKey, TValue>
    where TValue : class
  {
    private readonly Dictionary<TKey, TValue> items = new Dictionary<TKey, TValue>();

    public TValue this[TKey key]
    {
      get => items.ContainsKey(key) ? items[key] : null;
      set => items[key] = value;
    }

    public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)items).Keys;

    public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)items).Values;

    public int Count => items.Count;

    public bool IsReadOnly => ((IDictionary<TKey, TValue>)items).IsReadOnly;

    public void Add(TKey key, TValue value)
    {
      items.Add(key, value);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
      ((IDictionary<TKey, TValue>)items).Add(item);
    }

    public void Clear()
    {
      items.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      return ((IDictionary<TKey, TValue>)items).Contains(item);
    }

    public bool ContainsKey(TKey key)
    {
      return items.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      ((IDictionary<TKey, TValue>)items).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      return ((IDictionary<TKey, TValue>)items).GetEnumerator();
    }

    public bool Remove(TKey key)
    {
      return items.Remove(key);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      return ((IDictionary<TKey, TValue>)items).Remove(item);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      return items.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IDictionary<TKey, TValue>)items).GetEnumerator();
    }
  }
}