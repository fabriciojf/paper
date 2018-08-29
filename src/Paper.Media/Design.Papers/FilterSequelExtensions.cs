using System;
using System.Collections.Generic;
using System.Text;
using Toolset.Sequel;

namespace Paper.Media.Design.Papers
{
  public static class FilterSequelExtensions
  {
    public static Sql Set(this Sql sql, IFilter filter)
    {
      sql.Set(new FieldMap(filter));
      return sql;
    }
  }
}
