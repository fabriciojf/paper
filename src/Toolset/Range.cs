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

    public Range<T> Cast<T>()
    {
      return new Range<T>(Change.To<T>(Min), Change.To<T>(Max));
    }

    public override string ToString()
    {
      return $"{{{Min ?? "*"}, {Max ?? "*"}}}";
    }
  }
}