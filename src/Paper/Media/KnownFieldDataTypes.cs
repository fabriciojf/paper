using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paper.Media
{
  /// <summary>
  /// Tipos de dados conhencidos.
  /// </summary>
  public static class KnownFieldDataTypes
  {
    /// <summary>
    /// Tipo para campo texto.
    /// Nomes alternativos:
    /// - string
    /// </summary>
    public const string Text = "text";

    /// <summary>
    /// Tipo para campo booliano.
    /// Nomes alternativos:
    /// - boolean
    /// </summary>
    public const string Bit = "bit";

    /// <summary>
    /// Tipo para campo númerico inteiro, sem dígito.
    /// Nomes alternativos:
    /// - integer
    /// - int
    /// - long
    /// </summary>
    public const string Number = "number";

    /// <summary>
    /// Tipo para campo numérico fracionário, com dígito.
    /// Nomes alternativos:
    /// - double
    /// - float
    /// </summary>
    public const string Decimal = "decimal";

    /// <summary>
    /// Tipo para campo data somente, sem hora.
    /// </summary>
    public const string Date = "date";

    /// <summary>
    /// Tipo para campo hora somente, sem data.
    /// </summary>
    public const string Time = "time";

    /// <summary>
    /// Tipo para campo data/hora.
    /// </summary>
    public const string Datetime = "datetime";

    public static string GetDataTypeName(object typeOrInstance)
    {
      if (typeOrInstance == null)
        return null;

      string typeName = null;

      var type = (typeOrInstance is Type) ? (Type)typeOrInstance : typeOrInstance.GetType();
      type = Nullable.GetUnderlyingType(type) ?? type;

      var isList = false;

      if (type.IsArray)
      {
        isList = true;
        type = type.GetElementType();
      }
      else if (typeof(IList<>).IsAssignableFrom(type))
      {
        isList = true;
        type = type.GetGenericArguments().Single();
      }

      if (type == typeof(DateTime) || type == typeof(TimeSpan))
      {
        typeName = type.Name.ToLower();
      }
      else
      {
        var compiler = new CSharpCodeProvider();
        var codeType = new CodeTypeReference(type);
        typeName = compiler.GetTypeOutput(codeType);
      }

      if (isList)
        typeName += "[]";

      if (typeName.Contains("AnonymousType"))
        typeName = "AnonymousType";

      return typeName;
    }

    public static bool IsList(object typeOrInstance)
    {
      if (typeOrInstance == null)
        return false;

      var type = (typeOrInstance is Type) ? (Type)typeOrInstance : typeOrInstance.GetType();
      return type.IsArray || typeof(IList<>).IsAssignableFrom(type);
    }
  }
}