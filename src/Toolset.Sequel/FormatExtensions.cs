using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Sequel
{
  /// <summary>
  /// Extensões de formatação e indentação do texto da SQL.
  /// </summary>
  public static class FormatExtensions
  {
    /// <summary>
    /// Formata e indenta uma SQL.
    /// A SQL deve ser escrita na conveção do Sequel para um
    /// resultado otimizado:
    /// 
    ///   @"select *
    ///       from tabela
    ///      where campo = valor"
    ///     .AsSql()
    ///     .Beautify()
    ///     ...
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <returns>A SQL indentada.</returns>
    public static Sql Beautify(this Sql sql)
    {
      var text = Beautify(sql.ToString());
      sql.Text = text;
      return sql;
    }

    /// <summary>
    /// Formata e indenta uma string.
    /// A string deve ser escrita na conveção do Sequel para um
    /// resultado otimizado:
    /// 
    ///   var sql =
    ///     @"select *
    ///         from tabela
    ///        where campo = valor"
    ///       .Beautify();
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <returns>A SQL indentada.</returns>
    public static string Beautify(this string sql)
    {
      var lines = sql.Split('\n');
      var indentSize = (
        from line in lines.Skip(1)
        where !string.IsNullOrWhiteSpace(line)
        let spaces = (line.Length - line.TrimStart().Length)
        select spaces
      ).DefaultIfEmpty().Min();

      var indentedLines =
        new[] { lines.First() }.Concat(
          from line in lines.Skip(1)
          select (line.Length > indentSize)
            ? line.Substring(indentSize)
            : line
        );

      var text = string.Join("\n", indentedLines).Trim();
      return text;
    }
  }
}
