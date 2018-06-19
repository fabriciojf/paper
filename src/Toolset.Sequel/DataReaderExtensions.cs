using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Toolset.Sequel
{
  /// <summary>
  /// Extensões da API do Sequel para transformação de registros obtidos da base.
  /// </summary>
  public static class DataReaderExtensions
  {

    #region Extensoes para DbDataReader

    /// <summary>
    /// Diz se o campo indicado existe no registro corrente.
    /// </summary>
    /// <param name="reader">O registro corrente em inspeção.</param>
    /// <param name="field">O nome do campo procurado.</param>
    /// <returns>Verdadeiro se o campo existe no registro.</returns>
    public static bool ContainsField(this DbDataReader reader, string field)
    {
      for (int i = 0; i < reader.FieldCount; i++)
      {
        var name = reader.GetName(i);
        if (name.Equals(field))
          return true;
      }
      return false;
    }

    /// <summary>
    /// Obtém o valor do campo convertido para o tipo desejado.
    /// Em caso de nulo o valor padrão do tipo é retornado.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="reader">O registro corrente em inspeção.</param>
    /// <param name="fieldIndex">O índice do campo procurado.</param>
    /// <returns>O valor convertido para o tipo desejado.</returns>
    public static T Get<T>(this DbDataReader reader, int fieldIndex)
    {
      if (fieldIndex < reader.FieldCount)
      {
        var value = reader[fieldIndex];
        if (!value.IsNull())
        {
          var convertedValue = (T)System.Convert.ChangeType(value, typeof(T));
          return convertedValue;
        }
      }
      return default(T);
    }

    /// <summary>
    /// Obtém o valor do campo convertido para o tipo desejado.
    /// Em caso de nulo o valor padrão indicado é retornado.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="reader">O registro corrente em inspeção.</param>
    /// <param name="fieldIndex">O índice do campo procurado.</param>
    /// <returns>O valor convertido para o tipo desejado.</returns>
    public static T Get<T>(this DbDataReader reader, int fieldIndex, T defaultValue)
    {
      if (fieldIndex < reader.FieldCount)
      {
        var value = reader[fieldIndex];
        if (!value.IsNull())
        {
          var convertedValue = (T)System.Convert.ChangeType(value, typeof(T));
          return convertedValue;
        }
      }
      return defaultValue;
    }

    /// <summary>
    /// Obtém o valor do campo convertido para o tipo desejado.
    /// Em caso de nulo o valor padrão do tipo é retornado.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="reader">O registro corrente em inspeção.</param>
    /// <param name="fieldIndex">O nome do campo procurado.</param>
    /// <returns>O valor convertido para o tipo desejado.</returns>
    public static T Get<T>(this DbDataReader reader, string fieldName)
    {
      var value = reader[fieldName];
      if (!value.IsNull())
      {
        var convertedValue = (T)System.Convert.ChangeType(value, typeof(T));
        return convertedValue;
      }
      return default(T);
    }

    /// <summary>
    /// Obtém o valor do campo convertido para o tipo desejado.
    /// Em caso de nulo o valor padrão indicado é retornado.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="reader">O registro corrente em inspeção.</param>
    /// <param name="fieldIndex">O nome do campo procurado.</param>
    /// <returns>O valor convertido para o tipo desejado.</returns>
    public static T Get<T>(this DbDataReader reader, string fieldName, T defaultValue)
    {
      var value = reader[fieldName];
      if (!value.IsNull())
      {
        var convertedValue = (T)System.Convert.ChangeType(value, typeof(T));
        return convertedValue;
      }
      return defaultValue;
    }

    #endregion

    #region Extensoes para Record

    /// <summary>
    /// Diz se o campo indicado existe no registro corrente.
    /// </summary>
    /// <param name="reader">O registro corrente em inspeção.</param>
    /// <param name="field">O nome do campo procurado.</param>
    /// <returns>Verdadeiro se o campo existe no registro.</returns>
    public static bool ContainsField(this Record record, string field)
    {
      for (int i = 0; i < record.FieldCount; i++)
      {
        var name = record.GetName(i);
        if (name.Equals(field))
          return true;
      }
      return false;
    }

    /// <summary>
    /// Obtém o valor do campo convertido para o tipo desejado.
    /// Em caso de nulo o valor padrão do tipo é retornado.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="reader">O registro corrente em inspeção.</param>
    /// <param name="fieldIndex">O índice do campo procurado.</param>
    /// <returns>O valor convertido para o tipo desejado.</returns>
    public static T Get<T>(this Record record, int fieldIndex)
    {
      if (fieldIndex < record.FieldCount)
      {
        var value = record[fieldIndex];
        if (!value.IsNull())
        {
          var convertedValue = (T)System.Convert.ChangeType(value, typeof(T));
          return convertedValue;
        }
      }
      return default(T);
    }

    /// <summary>
    /// Obtém o valor do campo convertido para o tipo desejado.
    /// Em caso de nulo o valor padrão indicado é retornado.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="reader">O registro corrente em inspeção.</param>
    /// <param name="fieldIndex">O índice do campo procurado.</param>
    /// <returns>O valor convertido para o tipo desejado.</returns>
    public static T Get<T>(this Record record, int fieldIndex, T defaultValue)
    {
      if (fieldIndex < record.FieldCount)
      {
        var value = record[fieldIndex];
        if (!value.IsNull())
        {
          var convertedValue = (T)System.Convert.ChangeType(value, typeof(T));
          return convertedValue;
        }
      }
      return defaultValue;
    }

    /// <summary>
    /// Obtém o valor do campo convertido para o tipo desejado.
    /// Em caso de nulo o valor padrão do tipo é retornado.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="reader">O registro corrente em inspeção.</param>
    /// <param name="fieldIndex">O nome do campo procurado.</param>
    /// <returns>O valor convertido para o tipo desejado.</returns>
    public static T Get<T>(this Record record, string fieldName)
    {
      var value = record[fieldName];
      if (!value.IsNull())
      {
        var convertedValue = (T)System.Convert.ChangeType(value, typeof(T));
        return convertedValue;
      }
      return default(T);
    }

    /// <summary>
    /// Obtém o valor do campo convertido para o tipo desejado.
    /// Em caso de nulo o valor padrão indicado é retornado.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="reader">O registro corrente em inspeção.</param>
    /// <param name="fieldIndex">O nome do campo procurado.</param>
    /// <returns>O valor convertido para o tipo desejado.</returns>
    public static T Get<T>(this Record record, string fieldName, T defaultValue)
    {
      var value = record[fieldName];
      if (!value.IsNull())
      {
        var convertedValue = (T)System.Convert.ChangeType(value, typeof(T));
        return convertedValue;
      }
      return defaultValue;
    }

    #endregion

    #region As mesmas extensoes mas para RecordResults

    /// <summary>
    /// Diz se o campo indicado existe no registro corrente.
    /// </summary>
    /// <param name="reader">O registro corrente em inspeção.</param>
    /// <param name="field">O nome do campo procurado.</param>
    /// <returns>Verdadeiro se o campo existe no registro.</returns>
    public static bool ContainsField(this RecordResult result, string field)
    {
      for (int i = 0; i < result.Current.FieldCount; i++)
      {
        var name = result.Current.GetName(i);
        if (name.Equals(field))
          return true;
      }
      return false;
    }

    /// <summary>
    /// Obtém o valor do campo convertido para o tipo desejado.
    /// Em caso de nulo o valor padrão do tipo é retornado.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="reader">O registro corrente em inspeção.</param>
    /// <param name="fieldIndex">O índice do campo procurado.</param>
    /// <returns>O valor convertido para o tipo desejado.</returns>
    public static T Get<T>(this RecordResult result, int fieldIndex)
    {
      if (fieldIndex < result.Current.FieldCount)
      {
        var value = result.Current[fieldIndex];
        if (!value.IsNull())
        {
          var convertedValue = (T)System.Convert.ChangeType(value, typeof(T));
          return convertedValue;
        }
      }
      return default(T);
    }

    /// <summary>
    /// Obtém o valor do campo convertido para o tipo desejado.
    /// Em caso de nulo o valor padrão indicado é retornado.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="reader">O registro corrente em inspeção.</param>
    /// <param name="fieldIndex">O índice do campo procurado.</param>
    /// <returns>O valor convertido para o tipo desejado.</returns>
    public static T Get<T>(this RecordResult result, int fieldIndex, T defaultValue)
    {
      if (fieldIndex < result.Current.FieldCount)
      {
        var value = result.Current[fieldIndex];
        if (!value.IsNull())
        {
          var convertedValue = (T)System.Convert.ChangeType(value, typeof(T));
          return convertedValue;
        }
      }
      return defaultValue;
    }

    /// <summary>
    /// Obtém o valor do campo convertido para o tipo desejado.
    /// Em caso de nulo o valor padrão do tipo é retornado.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="reader">O registro corrente em inspeção.</param>
    /// <param name="fieldIndex">O nome do campo procurado.</param>
    /// <returns>O valor convertido para o tipo desejado.</returns>
    public static T Get<T>(this RecordResult result, string fieldName)
    {
      var value = result.Current[fieldName];
      if (!value.IsNull())
      {
        var convertedValue = (T)System.Convert.ChangeType(value, typeof(T));
        return convertedValue;
      }
      return default(T);
    }

    /// <summary>
    /// Obtém o valor do campo convertido para o tipo desejado.
    /// Em caso de nulo o valor padrão indicado é retornado.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="reader">O registro corrente em inspeção.</param>
    /// <param name="fieldIndex">O nome do campo procurado.</param>
    /// <returns>O valor convertido para o tipo desejado.</returns>
    public static T Get<T>(this RecordResult result, string fieldName, T defaultValue)
    {
      var value = result.Current[fieldName];
      if (!value.IsNull())
      {
        var convertedValue = (T)System.Convert.ChangeType(value, typeof(T));
        return convertedValue;
      }
      return defaultValue;
    }

    #endregion

  }
}
