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
  public class Range<T> : Range
  {
    public new T Min => Change.To<T>(base.Min);
    public new T Max => Change.To<T>(base.Max);

    public Range(object min, object max)
      : base(min, max)
    {
    }

    public Range(T min, T max)
      : base(min, max)
    {
    }
  }
}