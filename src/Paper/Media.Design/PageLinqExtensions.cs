using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Paper.Media.Design;
using Paper.Media.Design.Papers.Rendering;
using Paper.Media.Utils;
using Toolset.Collections;
using Toolset.Reflection;

namespace Paper.Media.Design
{
  public static class PageLinqExtensions
  {
    public static IQueryable<T> PaginateBy<T>(this IQueryable<T> items, Page page)
    {
      if (page == null)
        return items;

      return items.Skip(page.Offset).Take(page.Size);
    }

    public static IEnumerable<T> PaginateBy<T>(this IEnumerable<T> items, Page page)
    {
      if (page == null)
        return items;

      return items.Skip(page.Offset).Take(page.Size);
    }
  }
}