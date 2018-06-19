using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Text.Template
{
  /// <summary>
  /// Opções de configuração do parser do TextTemplate.
  /// </summary>
  [Flags]
  public enum ParseOptions
  {
    None = 0,

    /// <summary>
    /// Ignora espaços em branco no template.
    /// Usual para expressões escritas em muitas linhas ou identadas.
    /// </summary>
    IgnoreWhitespace = 1
  }
}