using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Toolset;
using Toolset.Xml;

namespace Toolset.Sequel
{
  /// <summary>
  /// Extensões de validação e conversão de tipo.
  /// </summary>
  public static class ObjectExtensions
  {
    /// <summary>
    /// Diz se um valor pode ser considerado nulo.
    /// O método considera o tipo DBNull como um valor nulo.
    /// </summary>
    /// <param name="value">O valor a ser verificado.</param>
    /// <returns>Verdadeiro se o valor pode ser considerado nulo.</returns>
    public static bool IsNull(this object value)
    {
      return (value == null) || (value == DBNull.Value);
    }

    /// <summary>
    /// Converte o valor para o tipo indicado.
    /// Nulo é convertido para o valor padrão do tipo.
    /// </summary>
    /// <param name="value">O valor a ser convertido.</param>
    /// <param name="targetType">O tipo desejado.</param>
    /// <returns>O valor convertido.</returns>
    internal static object ConvertTo(this object value, Type targetType)
    {
      return ConvertTo(value, targetType, null);
    }

    /// <summary>
    /// Converte o valor para o tipo indicado.
    /// Nulo é convertido para o valor padrão indicado.
    /// </summary>
    /// <param name="value">O valor a ser convertido.</param>
    /// <param name="targetType">O tipo desejado.</param>
    /// <param name="defaultValue">O valor padrão em caso de nulo.</param>
    /// <returns>O valor convertido.</returns>
    internal static object ConvertTo(this object value, Type targetType, object defaultValue)
    {
      if (value.IsNull())
        return defaultValue;

      var isNullableType =
        (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>));

      var type =
        isNullableType
          ? Nullable.GetUnderlyingType(targetType)
          : targetType;
      
      object convertedValue = null;

      // se o valor é do tipo esperado nao precisamos de conversao
      if (type.IsInstanceOfType(value))
      {
        convertedValue = value;
      }
      else if (type == typeof(XDocument))
      {
        convertedValue = value.ToXDocument();
      }
      else if (type == typeof(XElement) || type == typeof(XContainer))
      {
        convertedValue = value.ToXElement();
      }
      else if (type.IsSubclassOf(typeof(Enum)))
      {
        var enumValue = System.Convert.ChangeType(value, typeof(int));
        convertedValue = Enum.ToObject(type, enumValue);
      }
      else
      {
        convertedValue = System.Convert.ChangeType(value, type);
      }

      if (isNullableType)
      {
        convertedValue = Activator.CreateInstance(targetType, convertedValue);
      }

      return convertedValue;
    }

    /// <summary>
    /// Converte o valor para o tipo indicado.
    /// Nulo é convertido para o valor padrão do tipo.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="value">O valor a ser convertido.</param>
    /// <returns>O valor convertido.</returns>
    internal static T ConvertTo<T>(this object value)
    {
      var convertedValue = (T)ConvertTo(value, typeof(T), default(T));
      return convertedValue;
    }

    /// <summary>
    /// Converte o valor para o tipo indicado.
    /// Nulo é convertido para o valor padrão do tipo.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="value">O valor a ser convertido.</param>
    /// <returns>O valor convertido.</returns>
    internal static T ConvertToOrDefault<T>(this object value)
    {
      try
      {
        var convertedValue = (T)ConvertTo(value, typeof(T), default(T));
        return convertedValue;
      }
      catch { /* nada a fazer */ }
      return default(T);
    }

    /// <summary>
    /// Converte o valor para o tipo indicado.
    /// Nulo é convertido para o valor padrão indicado.
    /// </summary>
    /// <typeparam name="T">O tipo desejado.</typeparam>
    /// <param name="value">O valor a ser convertido.</param>
    /// <param name="defaultValue">O valor padrão em caso de nulo.</param>
    /// <returns>O valor convertido.</returns>
    internal static T ConvertToOrDefault<T>(this object value, T defaultValue)
    {
      try
      {
        if (!value.IsNull())
        {
          var convertedValue = (T)System.Convert.ChangeType(value, typeof(T));
          return convertedValue;
        }
      }
      catch { /* nada a fazer */ }
      return defaultValue;
    }

  }
}
