using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Sequel
{
  public static class TraceExtensions
  {
    private static string Dump(Sql defaultSql)
    {
      var sql = defaultSql.Clone();

      if (SequelSettings.QueryTemplateEnabled)
        sql.ApplyTemplate();

      var text = sql.ToString().Beautify();
      var lines = text.Split('\n', '\r').Where(x => !string.IsNullOrWhiteSpace(x));

      var builder = new StringBuilder();
      builder.AppendLine("Sql {");
      builder.AppendLine("  Text {");
      foreach (var line in lines)
      {
        builder.Append("    ");
        builder.AppendLine(line);
      }
      builder.AppendLine("  }");
      builder.AppendLine("  Args {");
      foreach (var parameter in sql.ParameterNames)
      {
        var value = Commander.CreateSqlCompatibleValue(sql[parameter]);
        builder.Append("    ");
        builder.Append(parameter);
        builder.Append(" := ");
        builder.AppendLine(value is DBNull ? "(null)" : $"\"{value}\"");
      }
      builder.AppendLine("  }");
      builder.AppendLine("}");
      return builder.ToString();
    }

    public static Sql Trace(this Sql sql)
    {
      var text = Dump(sql);
      try
      {
        System.Diagnostics.Trace.WriteLine(text);
      }
      catch
      {
        // nada a fazer
      }
      return sql;
    }

    public static Sql Echo(this Sql sql)
    {
      var text = Dump(sql.ApplyTemplate());
      try
      {
        System.Diagnostics.Trace.WriteLine(text);
      }
      catch
      {
        // nada a fazer
      }
      return sql;
    }
  }
}
