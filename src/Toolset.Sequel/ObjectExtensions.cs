using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Toolset;
using Toolset.Collections;
using Toolset.Data;
using Toolset.Reflection;
using Toolset.Xml;

namespace Toolset.Sequel
{
  /// <summary>
  /// Extensões de validação e conversão de tipo.
  /// </summary>
  internal static class ObjectExtensions
  {
    /// <summary>
    /// Converte o valor para o tipo indicado.
    /// Nulo é convertido para o valor padrão do tipo.
    /// </summary>
    /// <param name="value">O valor a ser convertido.</param>
    /// <param name="targetType">O tipo desejado.</param>
    /// <returns>O valor convertido.</returns>
    public static object ConvertTo(this object value, Type targetType)
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
    public static object ConvertTo(this object value, Type targetType, object defaultValue)
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
    public static T ConvertTo<T>(this object value)
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
    public static T ConvertToOrDefault<T>(this object value)
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
    public static T ConvertToOrDefault<T>(this object value, T defaultValue)
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

    /// <summary>
    /// Desmonta um objeto em um mapa de propriedades.
    /// </summary>
    /// <param name="graph">O grafo a ser desmontado.</param>
    /// <returns>O mapa de propriedades do grafo.</returns>
    public static IDictionary<string, object> UnwrapGraph(this object graph)
    {
      var map = new HashMap();
      UnwrapGraph(graph, map);
      return map;
    }

    /// <summary>
    /// Desmonta um objeto em um mapa de propriedades.
    /// </summary>
    /// <param name="graph">O grafo a ser desmontado.</param>
    /// <param name="map">Mapa para escrita das propriedades encontradas.</param>
    /// <returns>O mapa de propriedades do grafo.</returns>
    public static void UnwrapGraph(this object graph, IDictionary<string, object> map)
    {
      foreach (var name in graph._GetPropertyNames())
      {
        var value = graph._Get(name);
        map[name] = value.IsSimpleValue() ? value : (value as Any ?? new Any(value));
      }
    }

    /// <summary>
    /// Determina se o objeto é considerado um grafo.
    /// Um grafo é um objeto qualquer exceto:
    /// -   Valores do tipo Any.
    /// -   Valores considerados simples (IsSimpleValue).
    /// -   Valores considerados enumerado (IsEnumerable).
    /// -   Valores considerados intervalo (IsRange).
    /// </summary>
    /// <returns>
    /// Verdadeiro se o tipo for considerado um grafo; Falso caso contrário.
    /// </returns>
    public static bool IsGraph(this object value)
    {
      if (value == null) return false;
      return !(value is Any) && !IsSimpleValue(value) && !IsEnumerable(value) && !IsRange(value);
    }

    /// <summary>
    /// Determina se o objeto é considerado um intervalo (Range).
    /// Um intervalo é um objeto qualquer com propriedades Min ou Max.
    /// </summary>
    /// <returns>
    /// Verdadeiro se o tipo for considerado um intervalo (Range); Falso caso contrário.
    /// </returns>
    public static bool IsRange(this object value)
    {
      if (value == null) return false;
      return value._Has("min") || value._Has("max");
    }

    /// <summary>
    /// Determina se o valor é considerado um valor simples.
    /// Um valor simples é um tipo primitivo qualquer ou uma string.
    /// </summary>
    /// <returns>
    /// Verdadeiro se o tipo for considerado simples; Falso caso contrário.
    /// </returns>
    public static bool IsSimpleValue(this object value)
    {
      if (value == null) return false;
      return (value is string) || (value?.GetType().IsValueType == true);
    }

    /// <summary>
    /// Determina se o valor é considerado um enumerado.
    /// Um enumerado é um tipo qualquer que implementa IEnumerable exceto strings.
    /// </summary>
    /// <returns>
    /// Verdadeiro se o tipo for considerado enumerado; Falso caso contrário.
    /// </returns>
    public static bool IsEnumerable(this object value)
    {
      if (value == null) return false;
      return !(value is string) && (value is IEnumerable);
    }
  }
}
