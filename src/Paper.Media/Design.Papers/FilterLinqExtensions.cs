using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Paper.Media.Design;
using Paper.Media.Utilities;
using Toolset;
using Toolset.Collections;
using Toolset.Data;
using Toolset.Reflection;

namespace Paper.Media.Design.Papers
{
  public static class FilterLinqExtensions
  {
    #region IQueryable com seletor de propriedade dinamico

    public static IQueryable<T> FilterBy<T>(this IQueryable<T> items, IFilter filter)
    {
      if (filter == null)
        return items;

      var type = typeof(T);
      var fields = new FieldMap(filter);
      foreach (var field in fields)
      {
        var property = type._GetPropertyInfo(field.Key);
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

        items = DoFilterBy(items, fields, keySelector);
      }

      return items;
    }

    #endregion

    #region IEnumerable com seletor de propriedade dinamico

    public static IEnumerable<T> FilterBy<T>(this IEnumerable<T> items, IFilter filter)
    {
      if (filter == null)
        return items;

      var type = typeof(T);
      var fields = new FieldMap(filter);
      foreach (var field in fields)
      {
        var property = type._GetPropertyInfo(field.Key);
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

        items = DoFilterBy(items, fields, keySelector);
      }

      return items;
    }

    #endregion

    #region IQueryable com seletor de propriedade

    public static IQueryable<T> FilterBy<T, TKey>(
        this IQueryable<T> items
      , Filter filter
      , Expression<Func<T, TKey>> keySelector)
    {
      var fields = new FieldMap(filter);
      return DoFilterBy(items, fields, keySelector);
    }

    public static IQueryable<T> ThenBy<T, TKey>(
        this IQueryable<T> items
      , Filter filter
      , Expression<Func<T, TKey>> keySelector)
    {
      var fields = new FieldMap(filter);
      return DoFilterBy(items, fields, keySelector);
    }

    private static IQueryable<T> DoFilterBy<T, TKey>(
        this IQueryable<T> items
      , FieldMap fields
      , Expression<Func<T, TKey>> keySelector)
    {
      if (fields == null || fields.Count == 0)
        return items;

      var expression = ExpressionUtils.FindMemberExpression(keySelector);
      if (expression == null)
        throw new Exception("A expressão não é válida. Apenas um seletor de propriedade de objeto é suportado.");

      var name = expression.Member.Name;
      var field = fields.FirstOrDefault(x => x.Key.EqualsIgnoreCase(name));
      if (field.Key != null)
      {
        var comparison = CreateComparisonExpression<T>(field.Key, field.Value, forSql: true);
        items = items.Where(comparison);
      }

      return items;
    }

    #endregion

    #region IEnumerable com seletor de propriedade

    public static IEnumerable<T> FilterBy<T, TKey>(
        this IEnumerable<T> items
      , Filter filter
      , Expression<Func<T, TKey>> keySelector)
    {
      var fields = new FieldMap(filter);
      return DoFilterBy(items, fields, keySelector);
    }

    public static IEnumerable<T> ThenBy<T, TKey>(
        this IEnumerable<T> items
      , Filter filter
      , Expression<Func<T, TKey>> keySelector)
    {
      var fields = new FieldMap(filter);
      return DoFilterBy(items, fields, keySelector);
    }

    private static IEnumerable<T> DoFilterBy<T, TKey>(
        this IEnumerable<T> items
      , FieldMap fields
      , Expression<Func<T, TKey>> keySelector)
    {
      if (fields == null || fields.Count == 0)
        return items;

      var expression = ExpressionUtils.FindMemberExpression(keySelector);
      if (expression == null)
        throw new Exception("A expressão não é válida. Apenas um seletor de propriedade de objeto é suportado.");

      var name = expression.Member.Name;
      var field = fields.FirstOrDefault(x => x.Key.EqualsIgnoreCase(name));
      if (field.Key != null)
      {
        var comparison = CreateComparisonExpression<T>(field.Key, field.Value, forSql: false);
        items = items.Where(comparison.Compile());
      }

      return items;
    }

    #endregion

    #region Funções lambda dinâmicas comparativas

    private static Expression<Func<T, bool>> CreateComparisonExpression<T>(string field, object value, bool forSql)
    {
      if (value == null)
        return MakeConstantExpression<T>(true);

      var isAny = value is IVar;
      if (isAny)
      {
        var any = (IVar)value;

        // Raramente um valor de um tipo Any pode ser outro tipo Any.
        // Neste caso vamos seguir com o valor do tipo Any mais profundo encontrado.
        while (any.Value is IVar)
        {
          value = any.Value;
        }

        value = any.Value;
      }

      var isList = (value is IEnumerable) && !(value is string);
      if (isList)
      {
        return MakeInExpression<T>(field, (IEnumerable)value);
      }

      var isRange = value._Has("min") || value._Has("max");
      if (isRange)
      {
        var min = value._Get("min");
        var max = value._Get("max");

        if (min != null && max != null)
          return MakeBetweenExpression<T>(field, min, max);

        if (min != null)
          return MakeGreaterThanExpression<T>(field, min);

        if (max != null)
          return MakeLesserThanExpression<T>(field, max);

        return MakeConstantExpression<T>(true);
      }

      if (value is string text)
      {
        if (text.Contains("%") || text.Contains("?"))
        {
          return forSql
            ? MakeLikeExpression<T>(field, text)
            : MakeRegexExpression<T>(field, text);
        }
      }

      return MakeEqualsToExpression<T>(field, value);
    }

    private static Expression<Func<T, bool>> MakeConstantExpression<T>(bool constantValue)
    {
      return x => constantValue;
    }

    private static Expression<Func<T, bool>> MakeEqualsToExpression<T>(string field, object value)
    {
      // Construindo uma expressao como esta:
      //   param => param.PROP == Change.To<T>(value)
      // Sendo
      //   PROP: O nome do campo pesquisado informado pelo parametro 'field'
      var param = Expression.Parameter(typeof(T));
      var lambda =
        Expression.Lambda<Func<T, bool>>(
          Expression.Equal(
            Expression.PropertyOrField(param, field),
            Expression.Call(
              typeof(Change),
              "To",
              new[] { typeof(T)._GetPropertyType(field) },
              Expression.Constant(value, typeof(object))
            )
          ),
          param
        );
      return lambda;
    }

    private static Expression<Func<T, bool>> MakeGreaterThanExpression<T>(string field, object min)
    {
      // Construindo uma expressao como esta:
      //   param => param.PROP >= Change.To<T>(min)
      // Sendo
      //   PROP: O nome do campo pesquisado informado pelo parametro 'field'
      var param = Expression.Parameter(typeof(T));
      var lambda =
        Expression.Lambda<Func<T, bool>>(
          Expression.GreaterThanOrEqual(
            Expression.PropertyOrField(param, field),
            Expression.Call(
              typeof(Change),
              "To",
              new[] { typeof(T)._GetPropertyType(field) },
              Expression.Constant(min, typeof(object))
            )
          ),
          param
        );
      return lambda;
    }

    private static Expression<Func<T, bool>> MakeLesserThanExpression<T>(string field, object max)
    {
      // Construindo uma expressao como esta:
      //   param => param.PROP <= Change.To<T>(max)
      // Sendo
      //   PROP: O nome do campo pesquisado informado pelo parametro 'field'
      var param = Expression.Parameter(typeof(T));
      var lambda =
        Expression.Lambda<Func<T, bool>>(
          Expression.LessThanOrEqual(
            Expression.PropertyOrField(param, field),
            Expression.Call(
              typeof(Change),
              "To",
              new[] { typeof(T)._GetPropertyType(field) },
              Expression.Constant(max, typeof(object))
            )
          ),
          param
        );
      return lambda;
    }

    private static Expression<Func<T, bool>> MakeBetweenExpression<T>(string field, object min, object max)
    {
      // Construindo uma expressao como esta:
      //   param => param.PROP >= Change.To<T>(min) && param.PROP <= Change.To<T>(max)
      // Sendo
      //   PROP: O nome do campo pesquisado informado pelo parametro 'field'
      var param = Expression.Parameter(typeof(T));
      var lambda =
        Expression.Lambda<Func<T, bool>>(
          Expression.And(
            Expression.GreaterThanOrEqual(
              Expression.PropertyOrField(param, field),
              Expression.Call(
                typeof(Change),
                "To",
                new[] { typeof(T)._GetPropertyType(field) },
                Expression.Constant(min, typeof(object))
              )
            ),
            Expression.LessThanOrEqual(
              Expression.PropertyOrField(param, field),
              Expression.Call(
                typeof(Change),
                "To",
                new[] { typeof(T)._GetPropertyType(field) },
                Expression.Constant(max, typeof(object))
              )
            )
          ),
          param
        );
      return lambda;
    }

    private static Expression<Func<T, bool>> MakeInExpression<T>(string field, IEnumerable list)
    {
      // Construindo uma expressao como esta:
      //   param => list.Contains(x => x.PROP)
      // Sendo
      //   PROP: O nome do campo pesquisado informado pelo parametro 'field'
      var targetType = typeof(T)._GetPropertyType(field);
      var terms = list.Cast<object>().Select(x => Change.To(x, targetType));

      var param = Expression.Parameter(typeof(T));
      var lambda =
        Expression.Lambda<Func<T, bool>>(
          Expression.Call(
            typeof(Enumerable),
            "Contains",
            new[] { typeof(object) },
            Expression.Constant(terms),
            Expression.Convert(
              Expression.PropertyOrField(param, field),
              typeof(object)
            )
          ),
          param
        );
      return lambda;
    }

    private static Expression<Func<T, bool>> MakeLikeExpression<T>(string field, string pattern)
    {
      // Construindo uma expressao como esta:
      //   param => EF.Functions.Like(param.PROP, value)
      // Sendo
      //   PROP: O nome do campo pesquisado informado pelo parametro 'field'
      var targetType = typeof(T)._GetPropertyType(field);
      
      var param = Expression.Parameter(typeof(T));
      var lambda =
        Expression.Lambda<Func<T, bool>>(
          Expression.Call(
            Expression.Constant(EF.Functions),
            "Like",
            (Type[])null,
            Expression.PropertyOrField(param, field),
            Expression.Constant(pattern)
          ),
          param
        );
      return lambda;
    }

    private static Expression<Func<T, bool>> MakeRegexExpression<T>(string field, string pattern)
    {
      // Construindo uma expressao como esta:
      //   param => EF.Functions.Like(param.PROP, value)
      // Sendo
      //   PROP: O nome do campo pesquisado informado pelo parametro 'field'
      var targetType = typeof(T)._GetPropertyType(field);
      var regexPattern = Var.CreateTextPattern(pattern);

      var param = Expression.Parameter(typeof(T));
      var lambda =
        Expression.Lambda<Func<T, bool>>(
          Expression.Call(
            typeof(Regex),
            "IsMatch",
            (Type[])null,
            Expression.PropertyOrField(param, field),
            Expression.Constant(regexPattern)
          ),
          param
        );
      return lambda;
    }

    #endregion

  }
}