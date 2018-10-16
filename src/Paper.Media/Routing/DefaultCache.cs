using System;
using System.Collections.Generic;
using System.Text;
using Toolset.Collections;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Cache de objetos.
  /// </summary>
  class DefaultCache : ICache
  {
    private HashMap cache = new HashMap();

    /// <summary>
    /// Quantidade de objetos no cache.
    /// </summary>
    public int Count => cache.Count;

    /// <summary>
    /// Chaves de acesso aos objetos do cache.
    /// </summary>
    public ICollection<string> Keys => cache.Keys;

    /// <summary>
    /// Determina se existe um objeto sob a chave indicada.
    /// </summary>
    /// <param name="key">A chave procurada.</param>
    /// <returns>Verdadeiro se o objeto existe; Falso caso contrário.</returns>
    public bool ContainsKey(string key) => cache.ContainsKey(key);

    /// <summary>
    /// Obtém o objeto sob a chave.
    /// Se o objeto não existir nulo é retornado.
    /// </summary>
    /// <param name="key">A chave procurada.</param>
    /// <returns>O objeto encontrado; Nulo caso contrário.</returns>
    public object Get(string key) => cache.Get(key);

    /// <summary>
    /// Guarda um objeto no cache sob a chave indicada.
    /// </summary>
    /// <param name="key">A chave procurada.</param>
    /// <param name="value">O objeto a ser estocado na chave.</param>
    public void Set(string key, object value) => cache[key] = value;
  }
}