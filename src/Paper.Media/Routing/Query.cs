using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Coleção dos argumentos de URI interpretados.
  /// </summary>
  public class Query
  {
    public Query(string queryString)
    {
    }

    /// <summary>
    /// Resolve o valor de um argumento de URI.
    /// </summary>
    /// <param name="name">Nome do argumento.</param>
    /// <returns>O valor do argumento.</returns>
    public string this[string name] { get => null; }
  }
}
