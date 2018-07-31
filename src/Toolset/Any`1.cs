using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Reflection;

namespace Toolset
{
  public class Any<T> : Any
  {
    private Range<T> _range;

    public Any()
      : base(default(T), x => Change.To<T>(x))
    {
    }

    public Any(object value)
      : base(value, x => Change.To<T>(x))
    {
    }

    public Any(T value)
      : base(value, x => Change.To<T>(x))
    {
    }

    public Any(T min, T max)
      : base(new { min, max }, x => Change.To<T>(x))
    {
    }

    public Any(IEnumerable<T> items)
      : base(items, x => Change.To<T>(x))
    {
    }

    public new IEnumerable<T> List => base.List?.Cast<T>();

    public new T Raw => base.Raw is T ? (T)base.Raw : default(T);

    public new Range<T> Range
    {
      get
      {
        if (IsRange && _range == null)
        {
          _range = new Range<T>((T)base.Range.Min, (T)base.Range.Max);
        }
        return _range;
      }
    }

    public static implicit operator T(Any<T> any)
    {
      return any.IsRaw ? any.Raw : default(T);
    }

    public static implicit operator Any<T>(T value)
    {
      return new Any<T>(value);
    }

    public static implicit operator Any<T>(T[] value)
    {
      return new Any<T>(value);
    }

    public static implicit operator Any<T>(List<T> value)
    {
      return new Any<T>(value);
    }

    public static implicit operator Any<T>(Range<T> value)
    {
      return new Any<T>(value);
    }
  }
}