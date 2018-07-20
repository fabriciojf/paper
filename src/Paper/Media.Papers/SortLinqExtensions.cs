using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Paper.Media.Papers.Rendering;
using Toolset.Collections;
using Toolset.Reflection;

namespace Paper.Media.Papers
{
  public static class SortLinqExtensions
  {
    public static ISortableQueryable<T> SortBy<T, TKey>(
        this IQueryable<T> items
      , Sort sort
      , Expression<Func<T, TKey>> keySelector)
    {
      var expression = keySelector.Body as MemberExpression;
      if (expression == null)
        throw new Exception("A expressão não é válida. Apenas um seletor de propriedade de objeto é suportado.");

      var name = expression.Member.Name;
      var field = sort[name];
      
      if (field?.Order == SortOrder.Ascending)
      {
        var source = (items as ISortableQueryable<T>)?.Source ?? items;
        items =
          (source is IOrderedQueryable<T>)
            ? ((IOrderedQueryable<T>)source).ThenBy(keySelector)
            : source.OrderBy(keySelector);
      }

      if (field?.Order == SortOrder.Descending)
      {
        var source = (items as ISortableQueryable<T>)?.Source ?? items;
        items =
          (source is IOrderedQueryable<T>)
            ? ((IOrderedQueryable<T>)source).ThenByDescending(keySelector)
            : source.OrderByDescending(keySelector);
      }

      return new SortableQueryable<T>(items);
    }

    public static ISortableEnumerable<T> SortBy<T, TKey>(
        this IEnumerable<T> items
      , Sort sort
      , Expression<Func<T, TKey>> keySelector)
    {
      var expression = keySelector.Body as MemberExpression;
      if (expression == null)
        throw new Exception("A expressão não é válida. Apenas um seletor de propriedade de objeto é suportado.");

      var name = expression.Member.Name;
      var field = sort[name];

      if (field?.Order == SortOrder.Ascending)
      {
        var source = (items as ISortableEnumerable<T>)?.Source ?? items;
        items =
          (source is IOrderedEnumerable<T>)
            ? ((IOrderedEnumerable<T>)source).ThenBy(keySelector.Compile())
            : source.OrderBy(keySelector.Compile());
      }

      if (field?.Order == SortOrder.Descending)
      {
        var source = (items as ISortableEnumerable<T>)?.Source ?? items;
        items =
          (source is IOrderedEnumerable<T>)
            ? ((IOrderedEnumerable<T>)source).ThenByDescending(keySelector.Compile())
            : source.OrderByDescending(keySelector.Compile());
      }

      return new SortableEnumerable<T>(items);
    }
  }
}