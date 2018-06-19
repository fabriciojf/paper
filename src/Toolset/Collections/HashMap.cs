using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Collections
{
  public class HashMap : IDictionary<string, object>
  {
    private readonly Dictionary<string, object> items = new Dictionary<string, object>();

    public HashMap()
    {
      items = new Dictionary<string, object>();
    }

    public HashMap(int capacity)
    {
      items = new Dictionary<string, object>(capacity);
    }

    public object this[string key]
    {
      get => items.ContainsKey(key) ? items[key] : null;
      set => items[key] = value;
    }

    public ICollection<string> Keys => ((IDictionary<string, object>)items).Keys;

    public ICollection<object> Values => ((IDictionary<string, object>)items).Values;

    public int Count => items.Count;

    public bool IsReadOnly => ((IDictionary<string, object>)items).IsReadOnly;

    public void Add(string key, object value)
    {
      items.Add(key, value);
    }

    public void Add(KeyValuePair<string, object> item)
    {
      ((IDictionary<string, object>)items).Add(item);
    }

    public void Clear()
    {
      items.Clear();
    }

    public bool Contains(KeyValuePair<string, object> item)
    {
      return ((IDictionary<string, object>)items).Contains(item);
    }

    public bool ContainsKey(string key)
    {
      return items.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
      ((IDictionary<string, object>)items).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
      return ((IDictionary<string, object>)items).GetEnumerator();
    }

    public bool Remove(string key)
    {
      return items.Remove(key);
    }

    public bool Remove(KeyValuePair<string, object> item)
    {
      return ((IDictionary<string, object>)items).Remove(item);
    }

    public bool TryGetValue(string key, out object value)
    {
      return items.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IDictionary<string, object>)items).GetEnumerator();
    }
  }
}