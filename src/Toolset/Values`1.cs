using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Reflection;

namespace Toolset
{
  public class Values<T> : Values
  {
    public Values(object value)
      : base(MakeCompatibleValue<T>(value))
    {
    }

    public Values(T value)
      : base(value)
    {
    }

    public Values(T min, T max)
      : base(min, max)
    {
    }

    public Values(IEnumerable<T> items)
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

      if (value.Has("Min") || value.Has("Max"))
      {
        var min = value.Get("Min");
        var max = value.Get("Max");

        if (min != null && max != null)
        {
          min = Cast.To<TTarget>(min);
          max = Cast.To<TTarget>(max);
          return new Range(min, max);
        }

        if (min != null)
        {
          min = Cast.To<TTarget>(min);
          return new Range(min, null);
        }

        if (max != null)
        {
          max = Cast.To<TTarget>(max);
          return new Range(null, max);
        }

        return null;
      }

      if ((value is IEnumerable) && !(value is string))
      {
        return ((IEnumerable)value).Cast<object>().Select(Cast.To<TTarget>);
      }

      return Cast.To<TTarget>(value);
    }

    #region Conversões

    public static implicit operator T(Values<T> value)
      => value.Value;

    public static implicit operator Values<T>(T value)
      => new Values<T>(value);

    public static implicit operator T[] (Values<T> value)
      => value.Array?.ToArray();

    public static implicit operator Values<T>(T[] value)
      => new Values<T>(value);

    public static implicit operator Range(Values<T> value)
      => value.IsNull ? null : new Range(value.Min, value.Max);

    public static implicit operator Values<T>(Range value)
      => new Values<T>(value);

    #endregion

  }
}
