using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Toolset.Sequel
{
  /// <summary>
  /// Extensões de processamento de parâmetros de SQL.
  /// </summary>
  public static class ParameterExtensions
  {
    /// <summary>
    /// Atribui de uma só vez uma série de parâmetros definidos no mapa
    /// de chave/valor indicado.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="parameters">Os parâmetros a ser atribuídos.</param>
    /// <returns>
    /// A mesma instância da SQL obtida como parâmetro para encadeamento
    /// de operações.
    /// </returns>
    public static Sql Set(this Sql sql, NameValueCollection parameters)
    {
      foreach (var key in parameters.AllKeys)
      {
        sql[key] = parameters[key];
      }
      return sql;
    }

    /// <summary>
    /// Atribui de uma só vez uma série de parâmetros definidos no mapa
    /// de chave/valor indicado.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="parameters">Os parâmetros a ser atribuídos.</param>
    /// <returns>
    /// A mesma instância da SQL obtida como parâmetro para encadeamento
    /// de operações.
    /// </returns>
    public static Sql Set(this Sql sql, IDictionary<string, object> parameters)
    {
      foreach (var key in parameters.Keys)
      {
        sql[key] = parameters[key];
      }
      return sql;
    }

    /// <summary>
    /// Atribui de uma só vez uma série de parâmetros definidos no vetor indicado.
    /// O vetor é interpretado em pares de chave valor.
    /// Iniciando em zero, os termos pares são considerados como chaves e os termos
    /// ímpares são considerados como valores destas chaves.
    /// Exemplo:
    ///     var texto = "select * from usuario where nome = @nome, ativo = @ativo";
    ///     var sql = texto.AsSql();
    ///     sql.Set("nome", "Fulano", "ativo", 1);
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="parameters">Os parâmetros a ser atribuídos.</param>
    /// <returns>
    /// A mesma instância da SQL obtida como parâmetro para encadeamento
    /// de operações.
    /// </returns>
    public static Sql Set(this Sql sql, params object[] parameters)
    {
      var enumerator = parameters.GetEnumerator();
      while (enumerator.MoveNext())
      {
        var token = enumerator.Current as string;
        if (token == null)
        {
          throw new SequelException(
            "Era esperado um nome de parâmetro ou uma atribuição (\"parametro=valor\") mas "
          + "foi encontrado: " + enumerator.Current
          );
        }

        if (token.Contains("="))
        {
          var statements = 
            from statement in token.Split(',')
            let tokens = statement.Split('=')
            select new {
              key = tokens.First(),
              value = string.Join("=", tokens.Skip(1))
            };

          foreach (var statement in statements)
          {
            var key = statement.key.Trim();
            sql[key] = statement.value.Trim();
          }
        }
        else
        {
          var key = token.Trim();

          if (!enumerator.MoveNext())
            throw new SequelException("Faltou definir o valor de: " + key);

          sql[key] = enumerator.Current;
        }
      }

      return sql;
    }

    /// <summary>
    /// Aplica uma formatação posicional usando String.Format do DotNet.
    /// Todas as capacidades do String.Format são suportadas.
    /// Exemplo:
    ///     var texto = "select * from {0} where {1} = {2}";
    ///     var sql = texto.AsSql();
    ///     sql.Format("usuario", "nome", "Fulano");
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="parameters">
    /// A coleção de parâmetros posicionais para substituição.
    /// </param>
    /// <returns>
    /// A mesma instância da SQL obtida como parâmetro para encadeamento
    /// de operações.
    /// </returns>
    public static Sql Format(this Sql sql, params object[] parameters)
    {
      var text = sql.ToString();

      // Escapando ocorrencias de { e }
      // - Caracteres { e } sao duplicados
      // - Em seguida o padrao {{numero}} é retornado para {numero}
      text =
        Regex.Replace(
          text.Replace("{", "{{").Replace("}", "}}"),
          "[{]([{][\\d]+[}])[}]",
          "$1"
        );

      sql.Text = string.Format(text, parameters);
      return sql;
    }
  }
}
