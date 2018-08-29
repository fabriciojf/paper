using System;
using System.Collections.Generic;
using System.Text;
using Toolset;

namespace Paper.Media
{
  public static class ClassExtensions
  {
    /// <summary>
    /// Obtém o nome padronizado da classe.
    /// O nome obtido equivale àquele declarado nas constantes de
    /// ClassNames.
    /// </summary>
    /// <param name="clazz">O nome da classe.</param>
    /// <returns>O nome padronizado da classe.</returns>
    public static string GetName(this Class clazz)
    {
      return clazz.ToString().ChangeCase(TextCase.CamelCase);
    }
  }
}