using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Paper.Media.Design;
using Toolset.Collections;
using Toolset.Reflection;

namespace Paper.Media.Design.Papers
{
  public static class FilterLinqExtensions
  {
    public static IQueryable<T> FilterBy<T>(this IQueryable<T> items, object filter)
    {
      if (filter == null)
        return items;

      return items;
    }

    public static IEnumerable<T> FilterBy<T>(this IEnumerable<T> items, object filter)
    {
      if (filter == null)
        return items;

      return items;
    }
  }
}