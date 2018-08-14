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
  public class Range2
  {
    public object Min { get; }
    public object Max { get; }

    public Range2(object min, object max)
    {
      this.Min = min;
      this.Max = max;
    }

    public Range2<T> As<T>()
    {
      return new Range2<T>(Change.To<T>(Min), Change.To<T>(Max));
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

    public static Range2<T> Between<T>(object min, object max)
      => new Range2<T>(min, max);

    public static Range2<T> Between<T>(T min, T max)
      => new Range2<T>(min, max);

    public static Range2<T> Below<T>(object max)
      => new Range2<T>(null, max);

    public static Range2<T> Below<T>(T max)
      => new Range2<T>(null, max);

    public static Range2<T> Above<T>(object min)
      => new Range2<T>(min, null);

    public static Range2<T> Above<T>(T min)
      => new Range2<T>(min, null);
  }
}