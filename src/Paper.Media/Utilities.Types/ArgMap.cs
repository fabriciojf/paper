using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Toolset;
using Toolset.Collections;
using Toolset.Data;
using Toolset.Reflection;

namespace Media.Utilities.Types
{
  public class ArgMap : IEnumerable<KeyValuePair<string, object>>
  {
    private readonly Map<string, object> items;

    public ArgMap()
    {
      this.items = new Map<string, object>();
    }

    public ArgMap(IEnumerable<KeyValuePair<string, object>> items)
    {
      this.items = new Map<string, object>(items);
    }

    public ICollection<string> Keys => items.Keys;

    public object this[string key]
    {
      get => Get(key);
      set => Set(key, value);
    }

    private object Get(string key)
    {
      object value = this;
      foreach (var token in key.Split('.'))
      {
        if (value == null)
          break;

        while (value is IVar)
          value = ((IVar)value).Value;

        if (value is ArgMap)
          value = ((ArgMap)value).items[token];
        else
          value = value?._Get(token);
      }
      return value;
    }

    private void Set(string key, object value)
    {
      ParseArg(this, key, value);
    }

    public T Get<T>(string key)
    {
      var value = Get(key);
      var convertedValue = Change.To<T>(value);
      return convertedValue;
    }

    public bool Contains(string key)
    {
      if (items.ContainsKey(key))
        return true;

      object value = this;
      foreach (var token in key.Split('.'))
      {
        if (value == null)
          break;

        if (value is ArgMap)
          value = ((ArgMap)value).items[token];
        else
          value = value?._Get(token);
      }
      return value != null;
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
      return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return items.GetEnumerator();
    }

    private static void ParseArg(ArgMap map, string key, object value)
    {
      var isList = key.EndsWith("[]");
      if (isList)
      {
        key = key.Substring(0, key.Length - 2);
      }

      var isMin = key.EndsWithIgnoreCase(".min");
      var isMax = key.EndsWithIgnoreCase(".max");
      var isRange = isMin || isMax;
      if (isRange)
      {
        key = key.Substring(0, key.Length - 4);
      }

      var tokens = key.Split('.');
      var parents = tokens.Take(tokens.Length - 1);
      var current = tokens.Last();

      foreach (var parent in parents)
      {
        if (!(map.items[parent] is ArgMap))
        {
          map.items[parent] = new ArgMap();
        }
        map = (ArgMap)map.items[parent];
      }

      if (isRange)
      {
        var range = (Range)map.items[current];
        var min = isMin ? value : range.Min;
        var max = isMax ? value : range.Max;
        map.items[current] = new Range(min, max);
      }
      else if (isList)
      {
        var list = map.items[current] as List<object>;
        if (list == null)
        {
          map.items[current] = (list = new List<object>());
        }
        if (value != null)
        {
          list.Add(value);
        }
      }
      else
      {
        map.items[current] = value;
      }
    }
  }
}