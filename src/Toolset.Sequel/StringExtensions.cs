using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Sequel
{
  /// <summary>
  /// Extensões de conversão de instruções SQL em objetos Sql do Sequel.
  /// </summary>
  public static class StringExtensions
  {
    /// <summary>
    /// Converte uma instrução SQL em um objeto Sql do Sequel.
    /// O objeto Sql é requerido para aplicação dos métodos de
    /// extensão do Sequel.
    /// 
    /// Exemplo:
    /// 
    ///     using (var scope = new SequelScope("conexao"))
    ///     {
    ///       var sql = "delete from usuario".AsSql();
    ///       sql.Execute();
    ///     }
    /// </summary>
    /// <param name="text">A SQL a ser processada.</param>
    /// <returns>
    /// O objeto Sql do Sequel para aplicação dos métodos de extensão do Sequel.
    /// </returns>
    public static Sql AsSql(this string text, SequelScope scope = null)
    {
      return new Sql(scope) { Text = text };
    }

    /// <summary>
    /// Converte linhas de uma instrução SQL em um objeto Sql do Sequel.
    /// O objeto Sql é requerido para aplicação dos métodos de
    /// extensão do Sequel.
    /// 
    /// Exemplo:
    /// 
    ///     using (var scope = new SequelScope("conexao"))
    ///     {
    ///       var sql = new[] { "delete from usuario", "where ativo = 0" }.AsSql();
    ///       sql.Execute();
    ///     }
    /// </summary>
    /// <param name="text">A SQL a ser processada.</param>
    /// <returns>
    /// O objeto Sql do Sequel para aplicação dos métodos de extensão do Sequel.
    /// </returns>
    public static Sql AsSql(this string[] sqlParts, SequelScope scope = null)
    {
      var text = string.Join("\n", sqlParts);
      return new Sql(scope) { Text = text };
    }
  }
}
