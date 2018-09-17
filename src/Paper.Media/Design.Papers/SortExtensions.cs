using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Media.Utilities.Types;
using Paper.Media.Utilities;
using Toolset.Collections;
using Toolset.Reflection;

namespace Paper.Media.Design.Papers
{
  public static class SortExtensions
  {
    public static Sort AddField(this Sort sort, string fieldName)
    {
      sort.Add(fieldName);
      return sort;
    }

    public static Sort AddField<T>(this Sort sort, Expression<Func<T, object>> keySelector)
    {
      var expression = ExpressionUtils.FindMemberExpression(keySelector);
      if (expression == null)
        throw new Exception("A expressão não é válida. Apenas um seletor de propriedade de objeto é suportado.");

      var fieldName = expression.Member.Name;
      sort.Add(fieldName);
      return sort;
    }

    public static Sort AddFields(this Sort sort, IEnumerable<string> fieldNames)
    {
      sort.AddRange(fieldNames);
      return sort;
    }

    public static Sort AddFieldsFrom(this Sort sort, object data)
    {
      if (data is IEnumerable && !(data is IDictionary))
      {
        var wrapper = DataWrapperEnumerable.Create(data);
        sort.AddRange(wrapper.EnumerateKeys());
      }
      else
      {
        var wrapper = DataWrapper.Create(data);
        sort.AddRange(wrapper.EnumerateKeys());
      }
      return sort;
    }

    public static Sort AddFieldsFrom<T>(this Sort sort)
    {
      var keys = typeof(T)._GetPropertyNames();
      sort.AddRange(keys);
      return sort;
    }
  }
}