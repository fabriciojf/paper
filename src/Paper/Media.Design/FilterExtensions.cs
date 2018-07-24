using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Paper.Media.Design.Papers.Rendering;
using Paper.Media.Utils;
using Toolset;
using Toolset.Collections;
using Toolset.Reflection;

namespace Paper.Media.Design
{
  public static class FilterExtensions
  {
    #region AddField<T>

    public static Filter AddField<T>(this Filter filter, Expression<Func<T, object>> keySelector, Action<Field> builder = null)
    {
      return (Filter)EntityActionFieldsExtensions
        .AddField<T>(filter, keySelector, builder);
    }

    public static Filter AddField<T>(this Filter filter, Expression<Func<T, object>> keySelector, string title)
    {
      return (Filter)EntityActionFieldsExtensions
        .AddField<T>(filter, keySelector, title);
    }

    public static Filter AddFieldMulti<T>(this Filter filter, Expression<Func<T, object>> keySelector, Action<Field> builder = null)
    {
      return (Filter)EntityActionFieldsExtensions
        .AddFieldMulti<T>(filter, keySelector, builder);
    }

    public static Filter AddFieldMulti<T>(this Filter filter, Expression<Func<T, object>> keySelector, string title)
    {
      return (Filter)EntityActionFieldsExtensions
        .AddFieldMulti<T>(filter, keySelector, title);
    }

    #endregion

    #region AddFieldsFrom<T>

    public static Filter AddFieldsFrom<T>(this Filter filter)
    {
      return (Filter)EntityActionFieldsExtensions
        .AddFieldsFrom<T>(filter);
    }

    public static Filter AddFieldsMultiFrom<T>(this Filter filter)
    {
      return (Filter)EntityActionFieldsExtensions
        .AddFieldsMultiFrom<T>(filter);
    }

    #endregion

    #region AddField

    public static Filter AddField(
        this Filter filter
      , string name
      , string dataType
      , string title = null
      )
    {
      return (Filter)EntityActionFieldsExtensions
        .AddField(filter, name, dataType, title);
    }

    public static Filter AddField(
        this Filter filter
      , string name
      , DataType dataType
      , string title = null
      )
    {
      return (Filter)EntityActionFieldsExtensions
        .AddField(filter, name, dataType, title);
    }

    public static Filter AddField(
        this Filter filter
      , string name
      , Action<Field> builder
      )
    {
      return (Filter)EntityActionFieldsExtensions
        .AddField(filter, name, builder);
    }

    public static Filter AddFieldMulti(
        this Filter filter
      , string name
      , string dataType
      , string title = null
      )
    {
      return (Filter)EntityActionFieldsExtensions
        .AddFieldMulti(filter, name, dataType, title);
    }

    public static Filter AddFieldMulti(
        this Filter filter
      , string name
      , DataType dataType
      , string title = null
      )
    {
      return (Filter)EntityActionFieldsExtensions
        .AddFieldMulti(filter, name, dataType, title);
    }

    public static Filter AddFieldMulti(
        this Filter filter
      , string name
      , Action<Field> builder
      )
    {
      return (Filter)EntityActionFieldsExtensions
        .AddFieldMulti(filter, name, builder);
    }

    #endregion
  }
}