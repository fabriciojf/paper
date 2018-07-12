using System;
using System.Collections.Generic;
using System.Text;
using Toolset;

namespace Paper.Media
{
  public static class RelExtensions
  {
    /// <summary>
    /// Obtém o nome padronizado do relacionamento.
    /// O nome obtido equivale àquele declarado nas constantes de
    /// RelNames.
    /// </summary>
    /// <param name="rel">O nome da relação.</param>
    /// <returns>O nome padronizado do relacionamento</returns>
    public static string GetName(this Rel rel)
    {
      return rel.ToString().ChangeCase(TextCase.CamelCase);
    }
  }
}