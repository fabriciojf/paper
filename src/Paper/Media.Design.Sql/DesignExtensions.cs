using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset;

namespace Paper.Media.Design.Sql
{
  public static class DesignExtensions
  {
    public static string ToSqlComponent(this Sort sort)
    {
      if (sort == null)
        return "";

      return string.Join(", ",
        sort.SortedFields.Select(x =>
          $"{x.FieldName} {(x.Order == Sort.Order.Ascending ? "" : " desc")}")
      );
    }
  }
}
