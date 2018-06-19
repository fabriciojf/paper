using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Sequel
{
  /// <summary>
  /// Utilitário para geração de nomes de parâmetros dinâmicos.
  /// O utilitário modifica o nome do parâmetro original com uma
  /// variação única para evitar conflito com nomes de parâmetros
  /// informados pelo cliente.
  /// </summary>
  internal class KeyGen
  {
    private int indexGenerator;

    /// <summary>
    /// Aplica uma modificação no nome do parâmetro para torná-lo único e
    /// diferenciá-lo de parâmetro informado pelo cliente.
    /// </summary>
    /// <param name="parameter">O nome sugerido para o parâmetro.</param>
    /// <returns>O novo nome para o parâmetro.</returns>
    public string Rename(string parameter, bool force = false)
    {
      if (parameter.Contains("@"))
        parameter = parameter.Replace("@", "");

      if (parameter.Contains("___"))
        parameter = parameter.Substring(0, parameter.IndexOf("___"));

      parameter += "___" + (++indexGenerator);
      return parameter;
    }
  }
}
