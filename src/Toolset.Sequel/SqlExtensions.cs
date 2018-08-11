using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;

namespace Toolset.Sequel
{
  /// <summary>
  /// Coleção dos utilitários da API do Sequel para execução e consulta
  /// de SQLs.
  /// 
  /// Os métodos desta extensão devem ser executados obrigatoriamente
  /// dentro de escopos do Sequel.
  /// 
  /// Exemplo:
  /// 
  ///     using (var scope = new SequelScope("conexao"))
  ///     {
  ///       var reader =
  ///         @"select *
  ///             from usuarios"
  ///           .AsSql()
  ///           .Select()
  ///           
  ///       using (reader)
  ///       {
  ///         ...
  ///       }
  ///     }
  /// </summary>
  public static class SqlExtensions
  {
    #region Select As RecordResult

    /// <summary>
    /// Determina se a consulta produz  resultados.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// Verdadeiro se a consulta retorna pelo menos um resultado
    /// ou falso caso contrário.
    /// </returns>
    public static bool TryExists(this Sql sql)
    {
      try { return Exists(sql); }
      catch { return false; }
    }

    /// <summary>
    /// Determina se a consulta produz  resultados.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// Verdadeiro se a consulta retorna pelo menos um resultado
    /// ou falso caso contrário.
    /// </returns>
    public static bool Exists(this Sql sql)
    {
      var cn = sql.Connection;
      var command = Commander.CreateCommand(cn, sql);
      var result = command.ExecuteScalar();
      return result != null;
    }

    #endregion

    #region Select As RecordResult

    /// <summary>
    /// Obtém um IResult com os registros obtidos.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult com os registros obtidos.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static RecordResult TrySelect(this Sql sql)
    {
      try { return Select(sql); }
      catch { return null; }
    }

    /// <summary>
    /// Obtém um IResult com os registros obtidos.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult com os registros obtidos.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static RecordResult Select(this Sql sql)
    {
      var cn = sql.Connection;
      var result = new RecordResult(() => Commander.CreateCommand(cn, sql));
      return result;
    }

    #endregion

    #region Select As Result

    /// <summary>
    /// Obtém um IResult contendo apenas a primeira coluna obtida pela consulta
    /// convertida para o tipo desejado.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T">Tipo do resultado desejado.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo apenas a primeira coluna obtida pela consulta
    /// convertida para o tipo desejado.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static IResult<T> TrySelect<T>(this Sql sql)
    {
      try { return Select<T>(sql); }
      catch { return Result<T>.Empty; }
    }

    /// <summary>
    /// Obtém um IResult contendo apenas a primeira coluna obtida pela consulta
    /// convertida para o tipo desejado.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <typeparam name="T">Tipo do resultado desejado.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo apenas a primeira coluna obtida pela consulta
    /// convertida para o tipo desejado.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static IResult<T> Select<T>(this Sql sql)
    {
      var cn = sql.Connection;
      var result = new Result<T>(
        () => Commander.CreateCommand(cn, sql),
        r => r.GetValue(0).ConvertToOrDefault<T>()
      );
      return result;
    }

    #endregion

    #region SelectOne

    /// <summary>
    /// Obtém o valor da primeira coluna do primeiro resultado obtido.
    /// Se nenhum resultado for obtido nulo será retornado.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer o valor padrão será retornado.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O valor da primeira coluna do primeiro resultado obtido.
    /// Se nenhum resultado for obtido nulo será retornado.
    /// </returns>
    public static object TrySelectOne(this Sql sql)
    {
      try { return SelectOne(sql); }
      catch { return null; }
    }

    /// <summary>
    /// Obtém o valor da primeira coluna do primeiro resultado obtido.
    /// Se nenhum resultado for obtido nulo será retornado.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O valor da primeira coluna do primeiro resultado obtido.
    /// Se nenhum resultado for obtido nulo será retornado.
    /// </returns>
    public static object SelectOne(this Sql sql)
    {
      var cn = sql.Connection;
      var command = Commander.CreateCommand(cn, sql);
      var result = command.ExecuteScalar();
      return result;
    }

    /// <summary>
    /// Obtém o valor da primeira coluna do primeiro resultado obtido.
    /// Se nenhum resultado for obtido o valor padrão indicado será retornado.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer o valor padrão será retornado.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O valor da primeira coluna do primeiro resultado obtido.
    /// Se nenhum resultado for obtido o valor padrão indicado será retornado.
    /// </returns>
    public static object TrySelectOne(this Sql sql, object defaultValue)
    {
      try { return SelectOne(sql, defaultValue); }
      catch { return defaultValue; }
    }

    /// <summary>
    /// Obtém o valor da primeira coluna do primeiro resultado obtido.
    /// Se nenhum resultado for obtido o valor padrão indicado será retornado.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O valor da primeira coluna do primeiro resultado obtido.
    /// Se nenhum resultado for obtido o valor padrão indicado será retornado.
    /// </returns>
    public static object SelectOne(this Sql sql, object defaultValue)
    {
      var result = SelectOne(sql);
      return result ?? defaultValue;
    }

    /// <summary>
    /// Obtém o valor da primeira coluna do primeiro resultado convertido
    /// para o tipo indicado.
    /// Se nenhum resultado for obtido o valor padrão do tipo será retornado.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer o valor padrão será retornado.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// Obtém o valor da primeira coluna do primeiro resultado convertido
    /// para o tipo indicado.
    /// Se nenhum resultado for obtido o valor padrão do tipo será retornado.
    /// </returns>
    public static T TrySelectOne<T>(this Sql sql)
    {
      try { return SelectOne<T>(sql); }
      catch { return default(T); }
    }

    /// <summary>
    /// Obtém o valor da primeira coluna do primeiro resultado convertido
    /// para o tipo indicado.
    /// Se nenhum resultado for obtido o valor padrão do tipo será retornado.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// Obtém o valor da primeira coluna do primeiro resultado convertido
    /// para o tipo indicado.
    /// Se nenhum resultado for obtido o valor padrão do tipo será retornado.
    /// </returns>
    public static T SelectOne<T>(this Sql sql)
    {
      var result = SelectOne(sql);
      var convertedValue = result.ConvertToOrDefault<T>();
      return convertedValue;
    }

    /// <summary>
    /// Obtém o valor da primeira coluna do primeiro resultado convertido
    /// para o tipo indicado.
    /// Se nenhum resultado for obtido o valor padrão indicado será retornado.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer o valor padrão será retornado.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// Obtém o valor da primeira coluna do primeiro resultado convertido
    /// para o tipo indicado.
    /// Se nenhum resultado for obtido o valor padrão indicado será retornado.
    /// </returns>
    public static T TrySelectOne<T>(this Sql sql, T defaultValue)
    {
      try { return SelectOne<T>(sql, defaultValue); }
      catch { return defaultValue; }
    }

    /// <summary>
    /// Obtém o valor da primeira coluna do primeiro resultado convertido
    /// para o tipo indicado.
    /// Se nenhum resultado for obtido o valor padrão indicado será retornado.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// Obtém o valor da primeira coluna do primeiro resultado convertido
    /// para o tipo indicado.
    /// Se nenhum resultado for obtido o valor padrão indicado será retornado.
    /// </returns>
    public static T SelectOne<T>(this Sql sql, T defaultValue)
    {
      var result = SelectOne(sql);
      var convertedValue = result.ConvertToOrDefault<T>(defaultValue);
      return convertedValue;
    }

    #endregion

    #region Select As Graph

    /// <summary>
    /// Obtém um IResult contendo objetos construídos pela correspondência de nomes
    /// entre os campos do registro e as propriedades declaradas no objeto.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T">Tipo do resultado desejado.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// IResult contendo objetos construídos pela correspondência de nomes
    /// entre os campos do registro e as propriedades declaradas no objeto.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static IResult<T> TrySelectGraph<T>(this Sql sql)
      where T : class
    {
      try { return SelectGraph<T>(sql); }
      catch { return Result<T>.Empty; }
    }

    /// <summary>
    /// Obtém um IResult contendo objetos construídos pela correspondência de nomes
    /// entre os campos do registro e as propriedades declaradas no objeto.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <typeparam name="T">Tipo do resultado desejado.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo objetos construídos pela correspondência de nomes
    /// entre os campos do registro e as propriedades declaradas no objeto.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static IResult<T> SelectGraph<T>(this Sql sql)
      where T : class
    {
      var cn = sql.Connection;
      var result = new Result<T>(
        () => Commander.CreateCommand(cn, sql),
        Graph.CreateGraph<T>
      );
      return result;
    }

    /// <summary>
    /// Obtém um IResult contendo objetos construídos pela correspondência de nomes
    /// entre os campos do registro e as propriedades declaradas no objeto.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <param name="type">Tipo do resultado desejado.</param>
    /// <returns>
    /// IResult contendo objetos construídos pela correspondência de nomes
    /// entre os campos do registro e as propriedades declaradas no objeto.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static IResult<object> TrySelectGraph(this Sql sql, Type type)
    {
      try { return SelectGraph(sql, type); }
      catch { return Result<object>.Empty; }
    }

    /// <summary>
    /// Obtém um IResult contendo objetos construídos pela correspondência de nomes
    /// entre os campos do registro e as propriedades declaradas no objeto.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <param name="type">Tipo do resultado desejado.</param>
    /// <returns>
    /// O IResult contendo objetos construídos pela correspondência de nomes
    /// entre os campos do registro e as propriedades declaradas no objeto.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static IResult<object> SelectGraph(this Sql sql, Type type)
    {
      var cn = sql.Connection;
      var result = new Result<object>(
        () => Commander.CreateCommand(cn, sql),
        reader => Graph.CreateGraph(reader, type)
      );
      return result;
    }

    #endregion

    #region Select As GraphArray

    /// <summary>
    /// Obt├⌐m um Array contendo objetos constru├¡dos pela correspond├¬ncia de nomes
    /// entre os campos do registro e as propriedades declaradas no objeto.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    ///
    /// Como todo m├⌐todo Try* este m├⌐todo n├úo lan├ºa exce├º├úo.
    /// Se uma falha ocorrer uma cole├º├úo vazia ser├í retornada.
    /// </summary>
    /// <typeparam name="T">Tipo do resultado desejado.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// Array contendo objetos constru├¡dos pela correspond├¬ncia de nomes
    /// entre os campos do registro e as propriedades declaradas no objeto.
    /// </returns>
    public static T[] TrySelectGraphArray<T>(this Sql sql)
      where T : class
    {
      try { return SelectGraphArray<T>(sql); }
      catch { return new T[0]; }
    }

    /// <summary>
    /// Obt├⌐m um Array contendo objetos constru├¡dos pela correspond├¬ncia de nomes
    /// entre os campos do registro e as propriedades declaradas no objeto.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <typeparam name="T">Tipo do resultado desejado.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// Array contendo objetos constru├¡dos pela correspond├¬ncia de nomes
    /// entre os campos do registro e as propriedades declaradas no objeto.
    /// </returns>
    public static T[] SelectGraphArray<T>(this Sql sql)
      where T : class
    {
      var cn = SequelConnectionScope.GetScopedConnection();
      var result = new Result<T>(
        () => Commander.CreateCommand(cn, sql),
        Graph.CreateGraph<T>
      );
      using (result)
      {
        return result.ToArray();
      }
    }

    /// <summary>
    /// Obt├⌐m um Array contendo objetos constru├¡dos pela correspond├¬ncia de nomes
    /// entre os campos do registro e as propriedades declaradas no objeto.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    ///
    /// Como todo m├⌐todo Try* este m├⌐todo n├úo lan├ºa exce├º├úo.
    /// Se uma falha ocorrer uma cole├º├úo vazia ser├í retornada.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <param name="type">Tipo do resultado desejado.</param>
    /// <returns>
    /// Array contendo objetos constru├¡dos pela correspond├¬ncia de nomes
    /// entre os campos do registro e as propriedades declaradas no objeto.
    /// </returns>
    public static object[] TrySelectGraphArray(this Sql sql, Type type)
    {
      try { return SelectGraphArray(sql, type); }
      catch { return new object[0]; }
    }

    /// <summary>
    /// Obt├⌐m um Array contendo objetos constru├¡dos pela correspond├¬ncia de nomes
    /// entre os campos do registro e as propriedades declaradas no objeto.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <param name="type">Tipo do resultado desejado.</param>
    /// <returns>
    /// Array contendo objetos constru├¡dos pela correspond├¬ncia de nomes
    /// entre os campos do registro e as propriedades declaradas no objeto.
    /// </returns>
    public static object[] SelectGraphArray(this Sql sql, Type type)
    {
      var cn = SequelConnectionScope.GetScopedConnection();
      var result = new Result<object>(
        () => Commander.CreateCommand(cn, sql),
        reader => Graph.CreateGraph(reader, type)
      );
      using (result)
      {
        return result.ToArray();
      }
    }

    #endregion

    #region SelectOne As Graph

    /// <summary>
    /// Obtém um IResult objeto construído pela correspondência de nomes entre os campos
    /// do primeiro registro e as propriedadees do objeto.
    /// Se não houverem registros nulo será retornado.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer nulo será retornada.
    /// </summary>
    /// <typeparam name="T">Tipo do resultado desejado.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O objeto construído pela correspondência de nomes entre os campos
    /// do primeiro registro e as propriedadees do objeto.
    /// Se não houverem registros nulo será retornado.
    /// </returns>
    public static T TrySelectOneGraph<T>(this Sql sql)
      where T : class
    {
      try { return (T)SelectOneGraph(sql, typeof(T)); }
      catch { return default(T); }
    }

    /// <summary>
    /// Obtém um IResult objeto construído pela correspondência de nomes entre os campos
    /// do primeiro registro e as propriedadees do objeto.
    /// Se não houverem registros nulo será retornado.
    /// </summary>
    /// <typeparam name="T">Tipo do resultado desejado.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O objeto construído pela correspondência de nomes entre os campos
    /// do primeiro registro e as propriedadees do objeto.
    /// Se não houverem registros nulo será retornado.
    /// </returns>
    public static T SelectOneGraph<T>(this Sql sql)
      where T : class
    {
      return (T)SelectOneGraph(sql, typeof(T));
    }

    /// <summary>
    /// Obtém um IResult objeto construído pela correspondência de nomes entre os campos
    /// do primeiro registro e as propriedadees do objeto.
    /// Se não houverem registros nulo será retornado.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <param name="type">Tipo do resultado desejado.</param>
    /// <returns>
    /// O objeto construído pela correspondência de nomes entre os campos
    /// do primeiro registro e as propriedadees do objeto.
    /// Se não houverem registros nulo será retornado.
    /// </returns>
    public static object TrySelectOneGraph(this Sql sql, Type type)
    {
      try { return SelectOneGraph(sql, type); }
      catch { return null; }
    }

    /// <summary>
    /// Obtém um IResult objeto construído pela correspondência de nomes entre os campos
    /// do primeiro registro e as propriedadees do objeto.
    /// Se não houverem registros nulo será retornado.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <param name="type">Tipo do resultado desejado.</param>
    /// <returns>
    /// O objeto construído pela correspondência de nomes entre os campos
    /// do primeiro registro e as propriedadees do objeto.
    /// Se não houverem registros nulo será retornado.
    /// </returns>
    public static object SelectOneGraph(this Sql sql, Type type)
    {
      var cn = sql.Connection;
      using (var cm = Commander.CreateCommand(cn, sql))
      using (var reader = cm.ExecuteReader())
      {
        var ok = reader.Read();
        if (!ok)
          return null;

        var graph = Graph.CreateGraph(reader, type);
        return graph;
      }
    }

    #endregion

    #region Select As TupleResult

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      TupleResult<T1, T2>
      TrySelect<T1, T2>(this Sql sql)
    {
      try { return Select<T1, T2>(sql); }
      catch { return TupleResult<T1, T2>.Empty; }
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      TupleResult<T1, T2>
      Select<T1, T2>(this Sql sql)
    {
      var cn = sql.Connection;
      var result = new TupleResult<T1, T2>(() => Commander.CreateCommand(cn, sql));
      return result;
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      TupleResult<T1, T2, T3>
      TrySelect<T1, T2, T3>(this Sql sql)
    {
      try { return Select<T1, T2, T3>(sql); }
      catch { return TupleResult<T1, T2, T3>.Empty; }
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      TupleResult<T1, T2, T3>
      Select<T1, T2, T3>(this Sql sql)
    {
      var cn = sql.Connection;
      var result = new TupleResult<T1, T2, T3>(() => Commander.CreateCommand(cn, sql));
      return result;
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      TupleResult<T1, T2, T3, T4>
      TrySelect<T1, T2, T3, T4>(this Sql sql)
    {
      try { return Select<T1, T2, T3, T4>(sql); }
      catch { return TupleResult<T1, T2, T3, T4>.Empty; }
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      TupleResult<T1, T2, T3, T4>
      Select<T1, T2, T3, T4>(this Sql sql)
    {
      var cn = sql.Connection;
      var result = new TupleResult<T1, T2, T3, T4>(() => Commander.CreateCommand(cn, sql));
      return result;
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      TupleResult<T1, T2, T3, T4, T5>
      TrySelect<T1, T2, T3, T4, T5>(this Sql sql)
    {
      try { return Select<T1, T2, T3, T4, T5>(sql); }
      catch { return TupleResult<T1, T2, T3, T4, T5>.Empty; }
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      TupleResult<T1, T2, T3, T4, T5>
      Select<T1, T2, T3, T4, T5>(this Sql sql)
    {
      var cn = sql.Connection;
      var result = new TupleResult<T1, T2, T3, T4, T5>(() => Commander.CreateCommand(cn, sql));
      return result;
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      TupleResult<T1, T2, T3, T4, T5, T6>
      TrySelect<T1, T2, T3, T4, T5, T6>(this Sql sql)
    {
      try { return Select<T1, T2, T3, T4, T5, T6>(sql); }
      catch { return TupleResult<T1, T2, T3, T4, T5, T6>.Empty; }
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      TupleResult<T1, T2, T3, T4, T5, T6>
      Select<T1, T2, T3, T4, T5, T6>(this Sql sql)
    {
      var cn = sql.Connection;
      var result = new TupleResult<T1, T2, T3, T4, T5, T6>(() => Commander.CreateCommand(cn, sql));
      return result;
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <typeparam name="T7">Tipo da sétima coluna, retorna na tupla como Item7.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      TupleResult<T1, T2, T3, T4, T5, T6, T7>
      TrySelect<T1, T2, T3, T4, T5, T6, T7>(this Sql sql)
    {
      try { return Select<T1, T2, T3, T4, T5, T6, T7>(sql); }
      catch { return TupleResult<T1, T2, T3, T4, T5, T6, T7>.Empty; }
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <typeparam name="T7">Tipo da sétima coluna, retorna na tupla como Item7.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      TupleResult<T1, T2, T3, T4, T5, T6, T7>
      Select<T1, T2, T3, T4, T5, T6, T7>(this Sql sql)
    {
      var cn = sql.Connection;
      var result = new TupleResult<T1, T2, T3, T4, T5, T6, T7>(() => Commander.CreateCommand(cn, sql));
      return result;
    }

    #endregion

    #region Select As Array de Tuple

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      Tuple<T1, T2>[]
      TrySelectArray<T1, T2>(this Sql sql)
    {
      try { return SelectArray<T1, T2>(sql); }
      catch { return new Tuple<T1, T2>[0]; }
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      Tuple<T1, T2>[]
      SelectArray<T1, T2>(this Sql sql)
    {
      var cn = sql.Connection;
      var result = new TupleResult<T1, T2>(() => Commander.CreateCommand(cn, sql));
      using (result)
      {
        return result.ToArray();
      }
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      Tuple<T1, T2, T3>[]
      TrySelectArray<T1, T2, T3>(this Sql sql)
    {
      try { return SelectArray<T1, T2, T3>(sql); }
      catch { return new Tuple<T1, T2, T3>[0]; }
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      Tuple<T1, T2, T3>[]
      SelectArray<T1, T2, T3>(this Sql sql)
    {
      var cn = sql.Connection;
      var result = new TupleResult<T1, T2, T3>(() => Commander.CreateCommand(cn, sql));
      using (result)
      {
        return result.ToArray();
      }
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      Tuple<T1, T2, T3, T4>[]
      TrySelectArray<T1, T2, T3, T4>(this Sql sql)
    {
      try { return SelectArray<T1, T2, T3, T4>(sql); }
      catch { return new Tuple<T1, T2, T3, T4>[0]; }
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      Tuple<T1, T2, T3, T4>[]
      SelectArray<T1, T2, T3, T4>(this Sql sql)
    {
      var cn = sql.Connection;
      var result = new TupleResult<T1, T2, T3, T4>(() => Commander.CreateCommand(cn, sql));
      using (result)
      {
        return result.ToArray();
      }
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      Tuple<T1, T2, T3, T4, T5>[]
      TrySelectArray<T1, T2, T3, T4, T5>(this Sql sql)
    {
      try { return SelectArray<T1, T2, T3, T4, T5>(sql); }
      catch { return new Tuple<T1, T2, T3, T4, T5>[0]; }
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      Tuple<T1, T2, T3, T4, T5>[]
      SelectArray<T1, T2, T3, T4, T5>(this Sql sql)
    {
      var cn = sql.Connection;
      var result = new TupleResult<T1, T2, T3, T4, T5>(() => Commander.CreateCommand(cn, sql));
      using (result)
      {
        return result.ToArray();
      }
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      Tuple<T1, T2, T3, T4, T5, T6>[]
      TrySelectArray<T1, T2, T3, T4, T5, T6>(this Sql sql)
    {
      try { return SelectArray<T1, T2, T3, T4, T5, T6>(sql); }
      catch { return new Tuple<T1, T2, T3, T4, T5, T6>[0]; }
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      Tuple<T1, T2, T3, T4, T5, T6>[]
      SelectArray<T1, T2, T3, T4, T5, T6>(this Sql sql)
    {
      var cn = sql.Connection;
      var result = new TupleResult<T1, T2, T3, T4, T5, T6>(() => Commander.CreateCommand(cn, sql));
      using (result)
      {
        return result.ToArray();
      }
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <typeparam name="T7">Tipo da sétima coluna, retorna na tupla como Item7.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      Tuple<T1, T2, T3, T4, T5, T6, T7>[]
      TrySelectArray<T1, T2, T3, T4, T5, T6, T7>(this Sql sql)
    {
      try { return SelectArray<T1, T2, T3, T4, T5, T6, T7>(sql); }
      catch { return new Tuple<T1, T2, T3, T4, T5, T6, T7>[0]; }
    }

    /// <summary>
    /// Obtém um IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <typeparam name="T7">Tipo da sétima coluna, retorna na tupla como Item7.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O IResult contendo em cada registro uma tupla que mapeia
    /// os campos para os tipos indicados.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static
      Tuple<T1, T2, T3, T4, T5, T6, T7>[]
      SelectArray<T1, T2, T3, T4, T5, T6, T7>(this Sql sql)
    {
      var cn = sql.Connection;
      var result = new TupleResult<T1, T2, T3, T4, T5, T6, T7>(() => Commander.CreateCommand(cn, sql));
      using (result)
      {
        return result.ToArray();
      }
    }

    #endregion

    #region SelectOne As TupleResult

    /// <summary>
    /// Obtém uma tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <typeparam name="T7">Tipo da sétima coluna, retorna na tupla como Item7.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// A tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </returns>
    public static
      Tuple<T1, T2>
      TrySelectOne<T1, T2>(this Sql sql)
    {
      try { return MakeTuple(sql, TupleResult<T1, T2>.CreateTuple); }
      catch { return null; }
    }

    /// <summary>
    /// Obtém uma tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <typeparam name="T7">Tipo da sétima coluna, retorna na tupla como Item7.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// A tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </returns>
    public static
      Tuple<T1, T2>
      SelectOne<T1, T2>(this Sql sql)
    {
      return MakeTuple(sql, TupleResult<T1, T2>.CreateTuple);
    }

    /// <summary>
    /// Obtém uma tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <typeparam name="T7">Tipo da sétima coluna, retorna na tupla como Item7.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// A tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </returns>
    public static
      Tuple<T1, T2, T3>
      TrySelectOne<T1, T2, T3>(this Sql sql)
    {
      try { return MakeTuple(sql, TupleResult<T1, T2, T3>.CreateTuple); }
      catch { return null; }
    }

    /// <summary>
    /// Obtém uma tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <typeparam name="T7">Tipo da sétima coluna, retorna na tupla como Item7.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// A tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </returns>
    public static
      Tuple<T1, T2, T3>
      SelectOne<T1, T2, T3>(this Sql sql)
    {
      return MakeTuple(sql, TupleResult<T1, T2, T3>.CreateTuple);
    }

    /// <summary>
    /// Obtém uma tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <typeparam name="T7">Tipo da sétima coluna, retorna na tupla como Item7.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// A tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </returns>
    public static
      Tuple<T1, T2, T3, T4>
      TrySelectOne<T1, T2, T3, T4>(this Sql sql)
    {
      try { return MakeTuple(sql, TupleResult<T1, T2, T3, T4>.CreateTuple); }
      catch { return null; }
    }

    /// <summary>
    /// Obtém uma tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <typeparam name="T7">Tipo da sétima coluna, retorna na tupla como Item7.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// A tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </returns>
    public static
      Tuple<T1, T2, T3, T4>
      SelectOne<T1, T2, T3, T4>(this Sql sql)
    {
      return MakeTuple(sql, TupleResult<T1, T2, T3, T4>.CreateTuple);
    }

    /// <summary>
    /// Obtém uma tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <typeparam name="T7">Tipo da sétima coluna, retorna na tupla como Item7.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// A tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </returns>
    public static
      Tuple<T1, T2, T3, T4, T5>
      TrySelectOne<T1, T2, T3, T4, T5>(this Sql sql)
    {
      try { return MakeTuple(sql, TupleResult<T1, T2, T3, T4, T5>.CreateTuple); }
      catch { return null; }
    }

    /// <summary>
    /// Obtém uma tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <typeparam name="T7">Tipo da sétima coluna, retorna na tupla como Item7.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// A tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </returns>
    public static
      Tuple<T1, T2, T3, T4, T5>
      SelectOne<T1, T2, T3, T4, T5>(this Sql sql)
    {
      return MakeTuple(sql, TupleResult<T1, T2, T3, T4, T5>.CreateTuple);
    }

    /// <summary>
    /// Obtém uma tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <typeparam name="T7">Tipo da sétima coluna, retorna na tupla como Item7.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// A tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </returns>
    public static
      Tuple<T1, T2, T3, T4, T5, T6>
      TrySelectOne<T1, T2, T3, T4, T5, T6>(this Sql sql)
    {
      try { return MakeTuple(sql, TupleResult<T1, T2, T3, T4, T5, T6>.CreateTuple); }
      catch { return null; }
    }

    /// <summary>
    /// Obtém uma tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <typeparam name="T7">Tipo da sétima coluna, retorna na tupla como Item7.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// A tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </returns>
    public static
      Tuple<T1, T2, T3, T4, T5, T6>
      SelectOne<T1, T2, T3, T4, T5, T6>(this Sql sql)
    {
      return MakeTuple(sql, TupleResult<T1, T2, T3, T4, T5, T6>.CreateTuple);
    }

    /// <summary>
    /// Obtém uma tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <typeparam name="T7">Tipo da sétima coluna, retorna na tupla como Item7.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// A tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </returns>
    public static
      Tuple<T1, T2, T3, T4, T5, T6, T7>
      TrySelectOne<T1, T2, T3, T4, T5, T6, T7>(this Sql sql)
    {
      try { return MakeTuple(sql, TupleResult<T1, T2, T3, T4, T5, T6, T7>.CreateTuple); }
      catch { return null; }
    }

    /// <summary>
    /// Obtém uma tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </summary>
    /// <typeparam name="T1">Tipo da primeira coluna, retorna na tupla como Item1.</typeparam>
    /// <typeparam name="T2">Tipo da segunda coluna, retorna na tupla como Item2.</typeparam>
    /// <typeparam name="T3">Tipo da terceira coluna, retorna na tupla como Item3.</typeparam>
    /// <typeparam name="T4">Tipo da quarta coluna, retorna na tupla como Item4.</typeparam>
    /// <typeparam name="T5">Tipo da quinta coluna, retorna na tupla como Item5.</typeparam>
    /// <typeparam name="T6">Tipo da sexta coluna, retorna na tupla como Item6.</typeparam>
    /// <typeparam name="T7">Tipo da sétima coluna, retorna na tupla como Item7.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// A tupla que mapeia os campos do primeiro registro obtido
    /// para os tipos indicados.
    /// Se não houverem resultados nulo será retornado.
    /// </returns>
    public static
      Tuple<T1, T2, T3, T4, T5, T6, T7>
      SelectOne<T1, T2, T3, T4, T5, T6, T7>(this Sql sql)
    {
      return MakeTuple(sql, TupleResult<T1, T2, T3, T4, T5, T6, T7>.CreateTuple);
    }

    /// <summary>
    /// Utilitário para selecionar um registro e converter para uma
    /// tupla personalizada.
    /// </summary>
    /// <typeparam name="T">O tipo da tupla.</typeparam>
    /// <param name="sql">A consulta SQL.</param>
    /// <param name="factory">
    /// A fábrica de tupla, para conversão de um regitro em uma tupla.
    /// </param>
    /// <returns>A tupla produzida para um registro ou nulo.</returns>
    private static T MakeTuple<T>(this Sql sql, Func<DbDataReader, T> factory)
    {
      var cn = sql.Connection;
      using (var cm = Commander.CreateCommand(cn, sql))
      using (var reader = cm.ExecuteReader())
      {
        var ok = reader.Read();
        if (!ok)
          return default(T);

        var tuple = factory.Invoke(reader);
        return tuple;
      }
    }

    #endregion
    
    #region SelectArray

    /// <summary>
    /// Obtém um array contendo os valores da primeira coluna do resultado obtido.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O array contendo os valores da primeira coluna do resultado obtido.
    /// </returns>
    public static object[] TrySelectArray(this Sql sql)
    {
      try { return SelectArray(sql); }
      catch { return null; }
    }

    /// <summary>
    /// Obtém um array contendo os valores da primeira coluna do resultado obtido.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O array contendo os valores da primeira coluna do resultado obtido.
    /// </returns>
    public static object[] SelectArray(this Sql sql)
    {
      var cn = sql.Connection;
      var command = Commander.CreateCommand(cn, sql);
      using (var reader = command.ExecuteReader())
      {
        var values = EnumerateReader<object>(reader, 0);
        var result = values.ToArray();
        return result;
      }
    }

    /// <summary>
    /// Obtém um array contendo os valores da primeira coluna do resultado obtido
    /// convertidos para o tipo indicado.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <typeparam name="T">Tipo do valor esperado.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O array contendo os valores da primeira coluna do resultado obtido
    /// convertidos para o tipo indicado.
    /// </returns>
    public static T[] TrySelectArray<T>(this Sql sql)
    {
      try { return SelectArray<T>(sql); }
      catch { return new T[0]; }
    }

    /// <summary>
    /// Obtém um array contendo os valores da primeira coluna do resultado obtido
    /// convertidos para o tipo indicado.
    /// </summary>
    /// <typeparam name="T">Tipo do valor esperado.</typeparam>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// O array contendo os valores da primeira coluna do resultado obtido
    /// convertidos para o tipo indicado.
    /// </returns>
    public static T[] SelectArray<T>(this Sql sql)
    {
      var cn = sql.Connection;
      var command = Commander.CreateCommand(cn, sql);
      using (var reader = command.ExecuteReader())
      {
        var values = EnumerateReader<T>(reader, 0);
        var result = values.ToArray();
        return result;
      }
    }

    /// <summary>
    /// Função para enumeração de todos os registros de uma determinada coluna.
    /// </summary>
    /// <typeparam name="T">O tipo do array esperado.</typeparam>
    /// <param name="reader">O leitor de registros.</param>
    /// <param name="field">O índice do campo desejado.</param>
    /// <returns>O array obtido.</returns>
    private static IEnumerable<T> EnumerateReader<T>(DbDataReader reader, int field)
    {
      do
      {
        while (reader.Read())
        {
          var value = reader.GetValue(field);
          var convertedValue = value.ConvertToOrDefault<T>();
          yield return convertedValue;
        }
      } while (reader.NextResult());
    }

    #endregion

    #region Select Anonymous

    public static IResult<TItem> TrySelect<T1, TItem>(this Sql sql,
      Func<T1, TItem> builder)
    {
      try { return Select(sql, builder); }
      catch { return Result<TItem>.Empty; }
    }

    public static IResult<TItem> Select<T1, TItem>(this Sql sql,
      Func<T1, TItem> builder)
    {
      var reader = sql.Select<T1>();
      return new ResultWrapper<
        IResult<T1>
      , T1
      , TItem>
      (
        reader,
        x => builder.Invoke(x)
      );
    }

    public static IResult<TItem> TrySelect<T1, T2, TItem>(this Sql sql,
      Func<T1, T2, TItem> builder)
    {
      try { return Select(sql, builder); }
      catch { return Result<TItem>.Empty; }
    }

    public static IResult<TItem> Select<T1, T2, TItem>(this Sql sql,
      Func<T1, T2, TItem> builder)
    {
      var reader = sql.Select<T1, T2>();
      return new ResultWrapper<
        TupleResult<T1, T2>
      , Tuple<T1, T2>
      , TItem>
      (
        reader,
        x => builder.Invoke(x.Item1, x.Item2)
      );
    }

    public static IResult<TItem> TrySelect<T1, T2, T3, TItem>(this Sql sql,
      Func<T1, T2, T3, TItem> builder)
    {
      try { return Select(sql, builder); }
      catch { return Result<TItem>.Empty; }
    }

    public static IResult<TItem> Select<T1, T2, T3, TItem>(this Sql sql,
      Func<T1, T2, T3, TItem> builder)
    {
      var reader = sql.Select<T1, T2, T3>();
      return new ResultWrapper<
        TupleResult<T1, T2, T3>
      , Tuple<T1, T2, T3>
      , TItem>
      (
        reader,
        x => builder.Invoke(x.Item1, x.Item2, x.Item3)
      );
    }

    public static IResult<TItem> TrySelect<T1, T2, T3, T4, TItem>(this Sql sql,
      Func<T1, T2, T3, T4, TItem> builder)
    {
      try { return Select(sql, builder); }
      catch { return Result<TItem>.Empty; }
    }

    public static IResult<TItem> Select<T1, T2, T3, T4, TItem>(this Sql sql,
      Func<T1, T2, T3, T4, TItem> builder)
    {
      var reader = sql.Select<T1, T2, T3, T4>();
      return new ResultWrapper<
        TupleResult<T1, T2, T3, T4>
      , Tuple<T1, T2, T3, T4>
      , TItem>
      (
        reader,
        x => builder.Invoke(x.Item1, x.Item2, x.Item3, x.Item4)
      );
    }

    public static IResult<TItem> TrySelect<T1, T2, T3, T4, T5, TItem>(this Sql sql,
      Func<T1, T2, T3, T4, T5, TItem> builder)
    {
      try { return Select(sql, builder); }
      catch { return Result<TItem>.Empty; }
    }

    public static IResult<TItem> Select<T1, T2, T3, T4, T5, TItem>(this Sql sql,
      Func<T1, T2, T3, T4, T5, TItem> builder)
    {
      var reader = sql.Select<T1, T2, T3, T4, T5>();
      return new ResultWrapper<
        TupleResult<T1, T2, T3, T4, T5>
      , Tuple<T1, T2, T3, T4, T5>
      , TItem>
      (
        reader,
        x => builder.Invoke(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5)
      );
    }

    public static IResult<TItem> TrySelect<T1, T2, T3, T4, T5, T6, TItem>(this Sql sql,
      Func<T1, T2, T3, T4, T5, T6, TItem> builder)
    {
      try { return Select(sql, builder); }
      catch { return Result<TItem>.Empty; }
    }

    public static IResult<TItem> Select<T1, T2, T3, T4, T5, T6, TItem>(this Sql sql,
      Func<T1, T2, T3, T4, T5, T6, TItem> builder)
    {
      var reader = sql.Select<T1, T2, T3, T4, T5, T6>();
      return new ResultWrapper<
        TupleResult<T1, T2, T3, T4, T5, T6>
      , Tuple<T1, T2, T3, T4, T5, T6>
      , TItem>
      (
        reader,
        x => builder.Invoke(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6)
      );
    }

    public static IResult<TItem> TrySelect<T1, T2, T3, T4, T5, T6, T7, TItem>(this Sql sql,
      Func<T1, T2, T3, T4, T5, T6, T7, TItem> builder)
    {
      try { return Select(sql, builder); }
      catch { return Result<TItem>.Empty; }
    }

    public static IResult<TItem> Select<T1, T2, T3, T4, T5, T6, T7, TItem>(this Sql sql,
      Func<T1, T2, T3, T4, T5, T6, T7, TItem> builder)
    {
      var reader = sql.Select<T1, T2, T3, T4, T5, T6, T7>();
      return new ResultWrapper<
        TupleResult<T1, T2, T3, T4, T5, T6, T7>
      , Tuple<T1, T2, T3, T4, T5, T6, T7>
      , TItem>
      (
        reader,
        x => builder.Invoke(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7)
      );
    }

    #endregion

    #region SelectArray Anonymous

    public static TItem[] TrySelectArray<T1, TItem>(this Sql sql,
      Func<T1, TItem> builder)
    {
      try { return SelectArray(sql, builder); }
      catch { return new TItem[0]; }
    }

    public static TItem[] SelectArray<T1, TItem>(this Sql sql,
      Func<T1, TItem> builder)
    {
      var reader = sql.Select<T1>();
      using (reader)
      {
        var array = reader
          .Select(x => builder.Invoke(x))
          .ToArray();
        return array;
      }
    }

    public static TItem[] TrySelectArray<T1, T2, TItem>(this Sql sql,
      Func<T1, T2, TItem> builder)
    {
      try { return SelectArray(sql, builder); }
      catch { return new TItem[0]; }
    }

    public static TItem[] SelectArray<T1, T2, TItem>(this Sql sql,
      Func<T1, T2, TItem> builder)
    {
      var reader = sql.Select<T1, T2>();
      using (reader)
      {
        var array = reader
          .Select(x => builder.Invoke(x.Item1, x.Item2))
          .ToArray();
        return array;
      }
    }

    public static TItem[] TrySelectArray<T1, T2, T3, TItem>(this Sql sql,
      Func<T1, T2, T3, TItem> builder)
    {
      try { return SelectArray(sql, builder); }
      catch { return new TItem[0]; }
    }

    public static TItem[] SelectArray<T1, T2, T3, TItem>(this Sql sql,
      Func<T1, T2, T3, TItem> builder)
    {
      var reader = sql.Select<T1, T2, T3>();
      using (reader)
      {
        var array = reader
          .Select(x => builder.Invoke(x.Item1, x.Item2, x.Item3))
          .ToArray();
        return array;
      }
    }

    public static TItem[] TrySelectArray<T1, T2, T3, T4, TItem>(this Sql sql,
      Func<T1, T2, T3, T4, TItem> builder)
    {
      try { return SelectArray(sql, builder); }
      catch { return new TItem[0]; }
    }

    public static TItem[] SelectArray<T1, T2, T3, T4, TItem>(this Sql sql,
      Func<T1, T2, T3, T4, TItem> builder)
    {
      var reader = sql.Select<T1, T2, T3, T4>();
      using (reader)
      {
        var array = reader
          .Select(x => builder.Invoke(x.Item1, x.Item2, x.Item3, x.Item4))
          .ToArray();
        return array;
      }
    }

    public static TItem[] TrySelectArray<T1, T2, T3, T4, T5, TItem>(this Sql sql,
      Func<T1, T2, T3, T4, T5, TItem> builder)
    {
      try { return SelectArray(sql, builder); }
      catch { return new TItem[0]; }
    }

    public static TItem[] SelectArray<T1, T2, T3, T4, T5, TItem>(this Sql sql,
      Func<T1, T2, T3, T4, T5, TItem> builder)
    {
      var reader = sql.Select<T1, T2, T3, T4, T5>();
      using (reader)
      {
        var array = reader
          .Select(x => builder.Invoke(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5))
          .ToArray();
        return array;
      }
    }

    public static TItem[] TrySelectArray<T1, T2, T3, T4, T5, T6, TItem>(this Sql sql,
      Func<T1, T2, T3, T4, T5, T6, TItem> builder)
    {
      try { return SelectArray(sql, builder); }
      catch { return new TItem[0]; }
    }

    public static TItem[] SelectArray<T1, T2, T3, T4, T5, T6, TItem>(this Sql sql,
      Func<T1, T2, T3, T4, T5, T6, TItem> builder)
    {
      var reader = sql.Select<T1, T2, T3, T4, T5, T6>();
      using (reader)
      {
        var array = reader
          .Select(x => builder.Invoke(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6))
          .ToArray();
        return array;
      }
    }

    public static TItem[] TrySelectArray<T1, T2, T3, T4, T5, T6, T7, TItem>(this Sql sql,
      Func<T1, T2, T3, T4, T5, T6, T7, TItem> builder)
    {
      try { return SelectArray(sql, builder); }
      catch { return new TItem[0]; }
    }

    public static TItem[] SelectArray<T1, T2, T3, T4, T5, T6, T7, TItem>(this Sql sql,
      Func<T1, T2, T3, T4, T5, T6, T7, TItem> builder)
    {
      var reader = sql.Select<T1, T2, T3, T4, T5, T6, T7>();
      using (reader)
      {
        var array = reader
          .Select(x => builder.Invoke(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7))
          .ToArray();
        return array;
      }
    }

    #endregion

    #region SelectOne Anonymous

    public static TResult TrySelectOne<T1, TResult>(this Sql sql,
      Func<T1, TResult> builder)
    {
      try { return SelectOne(sql, builder); }
      catch { return default(TResult); }
    }

    public static TResult SelectOne<T1, TResult>(this Sql sql,
      Func<T1, TResult> builder)
    {
      var x = sql.SelectOne<T1>();
      return builder.Invoke(x);
    }

    public static TResult TrySelectOne<T1, T2, TResult>(this Sql sql,
      Func<T1, T2, TResult> builder)
    {
      try { return SelectOne(sql, builder); }
      catch { return default(TResult); }
    }

    public static TResult SelectOne<T1, T2, TResult>(this Sql sql,
      Func<T1, T2, TResult> builder)
    {
      var x = sql.SelectOne<T1, T2>();
      return builder.Invoke(x.Item1, x.Item2);
    }

    public static TResult TrySelectOne<T1, T2, T3, TResult>(this Sql sql,
      Func<T1, T2, T3, TResult> builder)
    {
      try { return SelectOne(sql, builder); }
      catch { return default(TResult); }
    }

    public static TResult SelectOne<T1, T2, T3, TResult>(this Sql sql,
      Func<T1, T2, T3, TResult> builder)
    {
      var x = sql.SelectOne<T1, T2, T3>();
      return builder.Invoke(x.Item1, x.Item2, x.Item3);
    }

    public static TResult TrySelectOne<T1, T2, T3, T4, TResult>(this Sql sql,
      Func<T1, T2, T3, T4, TResult> builder)
    {
      try { return SelectOne(sql, builder); }
      catch { return default(TResult); }
    }

    public static TResult SelectOne<T1, T2, T3, T4, TResult>(this Sql sql,
      Func<T1, T2, T3, T4, TResult> builder)
    {
      var x = sql.SelectOne<T1, T2, T3, T4>();
      return builder.Invoke(x.Item1, x.Item2, x.Item3, x.Item4);
    }

    public static TResult TrySelectOne<T1, T2, T3, T4, T5, TResult>(this Sql sql,
      Func<T1, T2, T3, T4, T5, TResult> builder)
    {
      try { return SelectOne(sql, builder); }
      catch { return default(TResult); }
    }

    public static TResult SelectOne<T1, T2, T3, T4, T5, TResult>(this Sql sql,
      Func<T1, T2, T3, T4, T5, TResult> builder)
    {
      var x = sql.SelectOne<T1, T2, T3, T4, T5>();
      return builder.Invoke(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5);
    }

    public static TResult TrySelectOne<T1, T2, T3, T4, T5, T6, TResult>(this Sql sql,
      Func<T1, T2, T3, T4, T5, T6, TResult> builder)
    {
      try { return SelectOne(sql, builder); }
      catch { return default(TResult); }
    }

    public static TResult SelectOne<T1, T2, T3, T4, T5, T6, TResult>(this Sql sql,
      Func<T1, T2, T3, T4, T5, T6, TResult> builder)
    {
      var x = sql.SelectOne<T1, T2, T3, T4, T5, T6>();
      return builder.Invoke(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6);
    }

    public static TResult TrySelectOne<T1, T2, T3, T4, T5, T6, T7, TResult>(this Sql sql,
      Func<T1, T2, T3, T4, T5, T6, T7, TResult> builder)
    {
      try { return SelectOne(sql, builder); }
      catch { return default(TResult); }
    }

    public static TResult SelectOne<T1, T2, T3, T4, T5, T6, T7, TResult>(this Sql sql,
      Func<T1, T2, T3, T4, T5, T6, T7, TResult> builder)
    {
      var x = sql.SelectOne<T1, T2, T3, T4, T5, T6, T7>();
      return builder.Invoke(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7);
    }

    #endregion

    #region Select With Custom Transform

    /// <summary>
    /// Obtém um IResult com os registros convertidos para o tipo desejado por
    /// uma função personalizada.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <param name="transform">
    /// função personalizada para conversão do registro Record no tipo desejado.
    /// </param>
    /// <returns>
    /// O IResult com os registros obtidos.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static Result<T> TrySelect<T>(this Sql sql, Func<Record, T> transform)
    {
      try { return Select(sql, transform); }
      catch { return null; }
    }

    /// <summary>
    /// Obtém um IResult com os registros convertidos para o tipo desejado por
    /// uma função personalizada.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <param name="transform">
    /// função personalizada para conversão do registro Record no tipo desejado.
    /// </param>
    /// <returns>
    /// O IResult com os registros obtidos.
    /// Todo objeto IResult deve ser fechado depois do uso.
    /// Prefira encapsular o uso do IResult em um bloco using.
    /// </returns>
    public static Result<T> Select<T>(this Sql sql, Func<Record, T> transform)
    {
      var cn = sql.Connection;
      var result = new Result<T>(
        () => Commander.CreateCommand(cn, sql),
        reader => transform.Invoke(new Record(reader))
      );
      return result;
    }

    #endregion

    #region SelectOne With Custom Transform

    /// <summary>
    /// Obtém um único registro convertido para o tipo desejado por
    /// uma função personalizada.
    /// Se nenhum resultado for obtido o valor padrão do tipo é retornado.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <param name="transform">
    /// função personalizada para conversão do registro Record no tipo desejado.
    /// </param>
    /// <returns>
    /// O registro convertido para o tipo desejado por
    /// uma função personalizada.
    /// Se nenhum resultado for obtido o valor padrão do tipo é retornado.
    /// </returns>
    public static T TrySelectOne<T>(this Sql sql, Func<Record, T> transform)
    {
      try { return SelectOne(sql, transform); }
      catch { return default(T); }
    }

    /// <summary>
    /// Obtém um único registro convertido para o tipo desejado por
    /// uma função personalizada.
    /// Se nenhum resultado for obtido o valor padrão do tipo é retornado.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <param name="transform">
    /// função personalizada para conversão do registro Record no tipo desejado.
    /// </param>
    /// <returns>
    /// O registro convertido para o tipo desejado por
    /// uma função personalizada.
    /// Se nenhum resultado for obtido o valor padrão do tipo é retornado.
    /// </returns>
    public static T SelectOne<T>(this Sql sql, Func<Record, T> transform)
    {
      var cn = sql.Connection;
      using (var cm = Commander.CreateCommand(cn, sql))
      using (var reader = cm.ExecuteReader())
      {
        var ok = reader.Read();
        if (!ok)
          return default(T);
        var result = (T)transform.Invoke(new Record(reader));
        return result;
      }
    }

    #endregion

    #region SelectOne With Custom Transform

    /// <summary>
    /// Obtém o resultado da consulta em uma instância de DataTable.
    /// Caso a consulta falhe nulo é retornado.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// A instância de DataTable contendo o resultado da consulta.
    /// Se a consulta falhar nulo será retornado.
    /// </returns>
    public static DataTable TrySelectDataTable(this Sql sql)
    {
      try { return SelectDataTable(sql); }
      catch { return null; }
    }

    /// <summary>
    /// Obtém o resultado da consulta em uma instância de DataTable.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    /// <returns>
    /// A instância de DataTable contendo o resultado da consulta.
    /// </returns>
    public static DataTable SelectDataTable(this Sql sql)
    {
      var cn = sql.Connection;
      using (var cm = Commander.CreateCommand(cn, sql))
      using (var adapter = cm.CreateDataAdapter())
      {
        var dataTable = new DataTable();
        adapter.Fill(dataTable);
        return dataTable;
      }
    }

    #endregion

    #region Execute

    /// <summary>
    /// Executa uma SQL sem produzir resultados.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    public static void TryExecute(this Sql sql)
    {
      try { Execute(sql); }
      catch { /* nada a fazer */ }
    }

    /// <summary>
    /// Executa uma SQL sem produzir resultados.
    /// </summary>
    /// <param name="sql">A SQL a ser executada.</param>
    public static void Execute(this Sql sql)
    {
      var cn = sql.Connection;
      var command = Commander.CreateCommand(cn, sql);
      command.ExecuteNonQuery();
    }

    #endregion

    #region CreateCommand

    /// <summary>
    /// Prepara um comando para execução.
    /// Um comando preparado é otimizado para execuções repetidas.
    /// </summary>
    /// <param name="sql">A SQL a ser peparada.</param>
    /// <returns>O comando preparado para execuções repetidas.</returns>
    public static DbCommand CreateCommand(this Sql sql)
    {
      var cn = sql.Connection;
      var command = Commander.CreateCommand(cn, sql);
      return command;
    }

    #endregion
  }
}
