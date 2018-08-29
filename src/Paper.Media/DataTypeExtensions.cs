using System;
using System.Collections.Generic;
using System.Text;
using Toolset;

namespace Paper.Media
{
  public static class DataTypeExtensions
  {
    /// <summary>
    /// Obtém o nome padronizado do tipo de dado.
    /// O nome obtido equivale àquele declarado nas constantes de
    /// DataTypeNames.
    /// </summary>
    /// <param name="dataType">O nome do tipo de dado.</param>
    /// <returns>O nome padronizado do tipo de dado</returns>
    public static string GetName(this DataType dataType)
    {
      return dataType.ToString().ChangeCase(TextCase.CamelCase);
    }
  }
}