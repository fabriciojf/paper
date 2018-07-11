
using System;
using System.Collections.Generic;
using System.Text;
using Toolset;

namespace Paper.Media
{
  public static class MethodExtensions
  {
    /// <summary>
    /// Obtém o nome padronizado do método HTTP.
    /// O nome obtido equivale àquele declarado nas constantes de
    /// MethodNames.
    /// </summary>
    /// <param name="method">O método HTTP.</param>
    /// <returns>O nome padronizado do método HTTP</returns>
    public static string GetName(this Method method)
    {
      return method.ToString().ToUpper();
    }
  }
}