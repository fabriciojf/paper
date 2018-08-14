using System;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Data
{
  public struct RangeEx<T>
    where T : class
  {
    public static readonly RangeEx<T> Empty = default(RangeEx<T>);

    public RangeEx(T min, T max)
    {
      this.Min = min;
      this.Max = max;
    }

    public T Min { get; }
    public T Max { get; }

    public static RangeEx<T> Between(T min, T max)
    {
      return new RangeEx<T>(min, max);
    }

    public static RangeEx<T> Above(T min)
    {
      return new RangeEx<T>(min, null);
    }

    public static RangeEx<T> Below(T max)
    {
      return new RangeEx<T>(null, max);
    }

    public static implicit operator Range(RangeEx<T> range)
    {
      return new Range(range.Min, range.Max);
    }

    public static implicit operator RangeEx<T>(Range range)
    {
      return new RangeEx<T>(Change.To<T>(range.Min), Change.To<T>(range.Max));
    }

    public override string ToString() => $"{{{$"'{Min}'" ?? "*"}, {$"'{Max}'" ?? "*"}}}";
  }
}