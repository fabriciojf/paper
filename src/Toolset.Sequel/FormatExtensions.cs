using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Collections;

namespace Toolset.Sequel
{
  /// <summary>
  /// Extensões de formatação e indentação do texto da SQL.
  /// </summary>
  public static class FormatExtensions
  {
    /// <summary>
    /// Remove a formatação e a identação da SQL.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <returns>A SQL indentada.</returns>
    public static Sql Uglify(this Sql sql)
    {
      var lines = sql.Text.Split('\n').NonWhitespace().Select(x => x.Trim());
      sql.Text = string.Join(" ", lines).Trim();
      return sql;
    }
    
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
      var lines = sql.Text.Split('\n');

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

      sql.Text = string.Join("\n", indentedLines).Trim();
      return sql;
    }
  }
}
