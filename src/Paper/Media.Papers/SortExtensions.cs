using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Paper.Media.Papers.Rendering;
using Toolset.Collections;
using Toolset.Reflection;

namespace Paper.Media.Papers
{
  public static class SortExtensions
  {
    public static Sort AddField(this Sort sort, string fieldName, SortOrder order = SortOrder.None)
    {
      var field = sort[fieldName] ?? (sort[fieldName] = new SortField());
      field.Order = order;
      return sort;
    }

    public static Sort AddFields(this Sort sort, IEnumerable<string> fieldNames)
    {
      foreach (var fieldName in fieldNames.Except(sort.FieldNames))
      {
        sort[fieldName] = new SortField();
      }
      return sort;
    }

    public static Sort AddFieldsFrom(this Sort sort, object data)
    {
      if (data is IEnumerable && !(data is IDictionary))
      {
        var wrapper = DataWrapperEnumerable.Create(data);
        AddFields(sort, wrapper.EnumerateKeys());
      }
      else
      {
        var wrapper = DataWrapper.Create(data);
        AddFields(sort, wrapper.EnumerateKeys());
      }
      return sort;
    }

    public static Sort AddFieldsFrom<T>(this Sort sort)
    {
      var keys = typeof(T)._GetPropertyNames();
      AddFields(sort, keys);
      return sort;
    }
  }
}