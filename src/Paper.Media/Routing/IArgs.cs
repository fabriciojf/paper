using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Coleção de argumentos extraídos da rota pela aplicação do template de URI.
  /// </summary>
  public interface IArgs
  {
    /// <summary>
    /// Quantidade de argumentos definidos.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Nomes dos argumentos definidos.
    /// </summary>
    ICollection<string> Names { get; }

    /// <summary>
    /// Obtém o argumento da posição.
    /// </summary>
    /// <param name="index">A posição do argumento.</param>
    /// <returns>O valor do argumento.</returns>
    object this[int index] { get; set; }

    /// <summary>
    /// Obtém o valor do argumento.
    /// </summary>
    /// <param name="name">Nome do argumento.</param>
    /// <returns>O valor do argumento.</returns>
    object this[string name] { get; set; }
  }
}