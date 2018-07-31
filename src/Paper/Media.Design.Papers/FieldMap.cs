using System;
using System.Collections.Generic;
using System.Text;
using Toolset;
using Toolset.Collections;
using Toolset.Reflection;

namespace Paper.Media.Design.Papers
{
  internal class FieldMap : HashMap
  {
    private readonly IFilter filter;

    public FieldMap(IFilter filter)
    {
      this.filter = filter;

      foreach (var entry in EnumerateEntries(filter))
      {
        this.Add(entry);
      }
    }

    private static IEnumerable<KeyValuePair<string, object>> EnumerateEntries(IFilter filter)
    {
      if (filter is EntityAction)
      {
        var action = (EntityAction)filter;
        if (action.Fields == null)
          yield break;

        foreach (var field in action.Fields)
        {
          var resolvedValue = ResolveValue(field.Value);
          yield return KeyValuePair.Create(field.Name, resolvedValue);
        }
      }
      else
      {
        var names = filter._GetPropertyNames();
        foreach (var name in names)
        {
          var value = filter._Get(name);
          var resolvedValue = ResolveValue(value);
          yield return KeyValuePair.Create(name, resolvedValue);
        }
      }
    }

    private static object ResolveValue(object value)
    {
      while (value is Any)
      {
        value = ((Any)value).Value;
      }
      return value;
    }
  }
}
