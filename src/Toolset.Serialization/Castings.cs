using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Toolset;

namespace Toolset.Serialization
{
  /// <summary>
  /// Métodos de extensão para conversão entre tipos de dados.
  /// </summary>
  internal static class Castings
  {
    /// <summary>
    /// Diz se um valor pode ser considerado nulo.
    /// O método considera o tipo DBNull como um valor nulo.
    /// </summary>
    /// <param name="value">O valor a ser verificado.</param>
    /// <returns>Verdadeiro se o valor pode ser considerado nulo.</returns>
    public static bool IsNull(object value)
    {
      return (value == null) || (value == DBNull.Value);
    }

    /// <summary>
    /// Substitui o método nativo do DotNet "Convert.ChangeType()" por um método mais apropriado
    /// para os tipos de conversão do sistema de serialização.
    /// 
    /// O resultado esperado é parecido com "Convert.ChangeType()", porém, com mais possibilidades
    /// de conversão de tipos.
    /// </summary>
    /// <typeparam name="T">O tipo esperado como retorno.</typeparam>
    /// <param name="value">O valor a ser convertido.</param>
    /// <returns>O valor convertido para o tipo esperado.</returns>
    public static T Cast<T>(object value)
    {
      var castValue = (T)Cast(value, typeof(T));
      return castValue;
    }

    /// <summary>
    /// Substitui o método nativo do DotNet "Convert.ChangeType()" por um método mais apropriado
    /// para os tipos de conversão do sistema de serialização.
    /// 
    /// O resultado esperado é parecido com "Convert.ChangeType()", porém, com mais possibilidades
    /// de conversão de tipos.
    /// </summary>
    /// <param name="value">O valor a ser convertido.</param>
    /// <param name="targetType">O tipo esperado como retorno.</param>
    /// <returns>O valor convertido para o tipo esperado.</returns>
    public static object Cast(object value, Type targetType)
    {
      if (IsNull(value))
        return null;

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
      else if (type == typeof(Boolean))
      {
        convertedValue = ToBoolean(value);
      }
      else if (type.IsSubclassOf(typeof(Enum)))
      {
        var enumValue = System.Convert.ChangeType(value, typeof(int));
        convertedValue = Enum.ToObject(type, enumValue);
      }
      else if (type == typeof(TimeSpan))
      {
        convertedValue =
          (value is long)
            ? TimeSpan.FromTicks((long)value)
            : TimeSpan.Parse(value.ToString());
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

    private static bool ToBoolean(object value)
    {
      if (value == null)
        return false;

      if (value is bool)
        return (bool)value;

      if (value is string)
      {
        var text = (string)value;

        if (text.EqualsIgnoreCase("true")) return true;
        if (text.EqualsIgnoreCase("false")) return false;

        if (text.EqualsIgnoreCase("1")) return true;
        if (text.EqualsIgnoreCase("0")) return false;

        if (text.EqualsIgnoreCase("on")) return true;
        if (text.EqualsIgnoreCase("off")) return false;

        if (text.EqualsIgnoreCase("y")) return true;
        if (text.EqualsIgnoreCase("n")) return false;

        if (text.EqualsIgnoreCase("yes")) return true;
        if (text.EqualsIgnoreCase("no")) return false;

        if (text.EqualsIgnoreCase("sim")) return true;
        if (text.EqualsIgnoreCase("nao")) return false;
        if (text.EqualsIgnoreCase("não")) return false;

        return XmlConvert.ToBoolean((string)value);
      }

      if (value.GetType().IsValueType)
      {
        var defaultValue = Activator.CreateInstance(value.GetType());
        return !value.Equals(defaultValue);
      }

      // qualquer objeto de tipo nao primario (IsValueType) e não nulo será considerado uma verdade.
      return true;
    }
  }
}