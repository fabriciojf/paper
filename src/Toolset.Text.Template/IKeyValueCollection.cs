using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolset.Text.Template
{
  /// <summary>
  /// Interface para coleções que suportam elementos na forma chave/valor.
  /// </summary>
  public interface IKeyValueCollection
  {
    /// <summary>
    /// Nomes das chaves constantes na coleção.
    /// </summary>
    IEnumerable<string> KeyNames { get; }

    /// <summary>
    /// Valor de uma chave na coleção.
    /// Caso a chave não exista nulo é retornado.
    /// </summary>
    /// <param name="keyName">Nome da chave procurada.</param>
    /// <returns>
    /// Valor da chave.
    /// Nulo caso a chave não exista.
    /// </returns>
    object GetValue(string keyName);
  }
}
