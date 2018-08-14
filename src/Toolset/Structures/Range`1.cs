using System;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Structures
{
  public struct Range<T>
    where T : struct
  {
    public static readonly Range<T> Empty = default(Range<T>);

    public Range(T? min, T? max)
    {
      this.Min = min;
      this.Max = max;
    }

    public T? Min { get; }
    public T? Max { get; }

    public static implicit operator Range(Range<T> range)
    {
      return new Range(range.Min, range.Max);
    }

    public static implicit operator Range<T>(Range range)
    {
      return new Range<T>(Change.To<T>(range.Min), Change.To<T>(range.Max));
    }

    public override string ToString() => $"{{{Min?.ToString() ?? "*"}, {Max?.ToString() ?? "*"}}}";
  }
}
