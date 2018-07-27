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
    public new T Min => (T)base.Min;
    public new T Max => (T)base.Max;

    public Range(T min, T max)
      : base(min, max)
    {
    }
  }
}