using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Cache de objetos.
  /// </summary>
  public interface ICache
  {
    /// <summary>
    /// Quantidade de objetos no cache.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Chaves de acesso aos objetos do cache.
    /// </summary>
    ICollection<string> Keys { get; }

    /// <summary>
    /// Determina se existe um objeto sob a chave indicada.
    /// </summary>
    /// <param name="key">A chave procurada.</param>
    /// <returns>Verdadeiro se o objeto existe; Falso caso contrário.</returns>
    bool ContainsKey(string key);

    /// <summary>
    /// Obtém o objeto sob a chave.
    /// Se o objeto não existir nulo é retornado.
    /// </summary>
    /// <param name="key">A chave procurada.</param>
    /// <returns>O objeto encontrado; Nulo caso contrário.</returns>
    object Get(string key);

    /// <summary>
    /// Guarda um objeto no cache sob a chave indicada.
    /// </summary>
    /// <param name="key">A chave procurada.</param>
    /// <param name="value">O objeto a ser estocado na chave.</param>
    void Set(string key, object value);
  }
}