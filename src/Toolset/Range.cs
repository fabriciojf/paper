using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Toolset.Collections;
using Toolset.Data;
using Toolset.Reflection;

namespace Toolset
{
  public class Range
  {
    public object Min { get; }
    public object Max { get; }

    public Range(object min, object max)
    {
      this.Min = min;
      this.Max = max;
    }

    public Range<T> As<T>()
    {
      return new Range<T>(Change.To<T>(Min), Change.To<T>(Max));
    }

    public override string ToString()
    {
      if (Min != null && Max != null)
        return $"{{ {Min} <= x <= {Max} }}";

      if (Min != null)
        return $"{{ >= {Min} }}";

      if (Max != null)
        return $"{{ <= {Min} }}";

      return "{}";
    }

    public static Range<T> Between<T>(object min, object max)
      => new Range<T>(min, max);

    public static Range<T> Between<T>(T min, T max)
      => new Range<T>(min, max);

    public static Range<T> EqualsOrLesser<T>(object max)
      => new Range<T>(null, max);

    public static Range<T> EqualsOrLesser<T>(T max)
      => new Range<T>(null, max);

    public static Range<T> EqualsOrGreater<T>(object min)
      => new Range<T>(min, null);

    public static Range<T> EqualsOrGreater<T>(T min)
      => new Range<T>(min, null);
  }
}