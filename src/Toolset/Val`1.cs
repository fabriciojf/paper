using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Reflection;

namespace Toolset
{
  public class Val<T> : Val
  {
    public Val(object value)
      : base(MakeCompatibleValue<T>(value))
    {
    }

    public Val(T value)
      : base(value)
    {
    }

    public Val(T min, T max)
      : base(min, max)
    {
    }

    public Val(IEnumerable<T> items)
      : base(items)
    {
    }

    public new T Value => (base.Value is T ? (T)base.Value : default(T));

    public new T Min => (base.Min is T ? (T)base.Min : default(T));

    public new T Max => (base.Max is T ? (T)base.Max : default(T));

    public new IEnumerable<T> Array => base.Array?.OfType<T>();

    private static object MakeCompatibleValue<TTarget>(object value)
    {
      if (value == null)
        return null;

      if (value._Has("Min") || value._Has("Max"))
      {
        var min = value._Get("Min");
        var max = value._Get("Max");

        if (min != null && max != null)
        {
          min = Change.To<TTarget>(min);
          max = Change.To<TTarget>(max);
          return new Range(min, max);
        }

        if (min != null)
        {
          min = Change.To<TTarget>(min);
          return new Range(min, null);
        }

        if (max != null)
        {
          max = Change.To<TTarget>(max);
          return new Range(null, max);
        }

        return null;
      }

      if ((value is IEnumerable) && !(value is string))
      {
        return ((IEnumerable)value).Cast<object>().Select(Change.To<TTarget>);
      }

      return Change.To<TTarget>(value);
    }

    #region Conversões

    public static implicit operator T(Val<T> value)
      => value.Value;

    public static implicit operator Val<T>(T value)
      => new Val<T>(value);

    public static implicit operator T[] (Val<T> value)
      => value.Array?.ToArray();

    public static implicit operator Val<T>(T[] value)
      => new Val<T>(value);

    public static implicit operator Range(Val<T> value)
      => value.IsNull ? null : new Range(value.Min, value.Max);

    public static implicit operator Val<T>(Range value)
      => new Val<T>(value);

    #endregion

  }
}
