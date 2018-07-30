using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Paper.Media.Design;
using Paper.Media.Utilities;
using Toolset.Collections;
using Toolset.Reflection;

namespace Paper.Media.Design.Papers
{
  public static class SortLinqExtensions
  {
    #region IQueryable com seletor de propriedade dinamico

    public static ISortableQueryable<T> SortBy<T>(this IQueryable<T> items, Sort sort)
    {
      if (sort == null)
        return new SortableQueryable<T>(items);

      var type = typeof(T);
      foreach (var field in sort.SortedFields)
      {
        var property = type._GetPropertyInfo(field.Name);
        if (property == null)
          continue;

        var param = Expression.Parameter(type);
        var keySelector =
          Expression.Lambda<Func<T, object>>(
            Expression.Convert(
              Expression.Property(
                param,
                property.Name
              ),
              typeof(object)
            ),
            param
          );

        var reset = !(items is ISortableQueryable<T>);
        items = DoSortBy(items, sort, keySelector, reset);
      }

      return items as SortableQueryable<T> ?? new SortableQueryable<T>(items);
    }

    #endregion

    #region IEnumerable com seletor de propriedade dinamico

    public static ISortableEnumerable<T> SortBy<T>(this IEnumerable<T> items, Sort sort)
    {
      if (sort == null)
        return new SortableEnumerable<T>(items);

      var type = typeof(T);
      foreach (var field in sort.SortedFields)
      {
        var property = type._GetPropertyInfo(field.Name);
        if (property == null)
          continue;

        var param = Expression.Parameter(type);
        var keySelector =
          Expression.Lambda<Func<T, object>>(
            Expression.Convert(
              Expression.Property(
                param,
                property.Name
              ),
              typeof(object)
            ),
            param
          );

        var reset = !(items is ISortableEnumerable<T>);
        items = DoSortBy(items, sort, keySelector, reset);
      }

      return items as ISortableEnumerable<T> ?? new SortableEnumerable<T>(items);
    }

    #endregion

    #region IQueryable com seletor de propriedade

    public static ISortableQueryable<T> SortBy<T, TKey>(
        this IQueryable<T> items
      , Sort sort
      , Expression<Func<T, TKey>> keySelector)
    {
      return DoSortBy(items, sort, keySelector, reset: true);
    }

    public static ISortableQueryable<T> ThenBy<T, TKey>(
        this ISortableQueryable<T> items
      , Sort sort
      , Expression<Func<T, TKey>> keySelector)
    {
      return DoSortBy(items, sort, keySelector, reset: false);
    }

    private static ISortableQueryable<T> DoSortBy<T, TKey>(
        this IQueryable<T> items
      , Sort sort
      , Expression<Func<T, TKey>> keySelector
      , bool reset)
    {
      if (sort == null)
        return new SortableQueryable<T>(items);

      var expression = Expressions.FindMemberExpression(keySelector);
      if (expression == null)
        throw new Exception("A expressão não é válida. Apenas um seletor de propriedade de objeto é suportado.");

      var name = expression.Member.Name;
      var field = sort.GetSortedField(name);

      if (field?.Order == SortOrder.Ascending)
      {
        var source = (items as ISortableQueryable<T>)?.Source ?? items;
        if (reset)
        {
          items = source.OrderBy(keySelector);
        }
        else
        {
          items =
            (source is IOrderedQueryable<T>)
              ? ((IOrderedQueryable<T>)source).ThenBy(keySelector)
              : source.OrderBy(keySelector);
        }
      }

      if (field?.Order == SortOrder.Descending)
      {
        var source = (items as ISortableQueryable<T>)?.Source ?? items;
        if (reset)
        {
          items = source.OrderByDescending(keySelector);
        }
        else
        {
          items =
            (source is IOrderedQueryable<T>)
              ? ((IOrderedQueryable<T>)source).ThenByDescending(keySelector)
              : source.OrderByDescending(keySelector);
        }
      }

      return new SortableQueryable<T>(items);
    }

    #endregion

    #region IEnumerable com seletor de propriedade

    public static ISortableEnumerable<T> SortBy<T, TKey>(
        this IEnumerable<T> items
      , Sort sort
      , Expression<Func<T, TKey>> keySelector)
    {
      if (sort == null)
        return new SortableEnumerable<T>(items);

      return DoSortBy(items, sort, keySelector, reset: true);
    }

    public static ISortableEnumerable<T> ThenBy<T, TKey>(
        this ISortableEnumerable<T> items
      , Sort sort
      , Expression<Func<T, TKey>> keySelector)
    {
      return DoSortBy(items, sort, keySelector, reset: false);
    }

    private static ISortableEnumerable<T> DoSortBy<T, TKey>(
        this IEnumerable<T> items
      , Sort sort
      , Expression<Func<T, TKey>> keySelector
      , bool reset)
    {
      if (sort == null)
        return new SortableEnumerable<T>(items);

      var expression = Expressions.FindMemberExpression(keySelector);
      if (expression == null)
        throw new Exception("A expressão não é válida. Apenas um seletor de propriedade de objeto é suportado.");

      var name = expression.Member.Name;
      var field = sort.GetSortedField(name);

      if (field?.Order == SortOrder.Ascending)
      {
        var source = (items as ISortableEnumerable<T>)?.Source ?? items;
        if (reset)
        {
          items = source.OrderBy(keySelector.Compile());
        }
        else
        {
          items =
            (source is IOrderedEnumerable<T>)
              ? ((IOrderedEnumerable<T>)source).ThenBy(keySelector.Compile())
              : source.OrderBy(keySelector.Compile());
        }
      }

      if (field?.Order == SortOrder.Descending)
      {
        var source = (items as ISortableEnumerable<T>)?.Source ?? items;
        if (reset)
        {
          items = source.OrderByDescending(keySelector.Compile());
        }
        else
        {
          items =
            (source is IOrderedEnumerable<T>)
              ? ((IOrderedEnumerable<T>)source).ThenByDescending(keySelector.Compile())
              : source.OrderByDescending(keySelector.Compile());
        }
      }

      return new SortableEnumerable<T>(items);
    }

    #endregion
  }
}