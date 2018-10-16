using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  public static class CacheExtensions
  {
    /// <summary>
    /// Determina se um dado para o tipo indicado existe.
    /// </summary>
    /// <typeparam name="T">O tipo do dado estocado.</typeparam>
    /// <returns>
    /// <c>true</c> se o dado existe ou <c>false</c>, caso contrário.
    /// </returns>
    public static bool Contains<T>(this ICache cache)
    {
      var key = typeof(T).FullName;
      return cache.ContainsKey(key);
    }

    /// <summary>
    /// Obtém um dado do tipo indicado.
    /// Se o dado não existir o valor padrão do tipo é retornado.
    /// </summary>
    /// <typeparam name="T">O tipo do dado procurado.</typeparam>
    /// <returns>O dado obtido ou o valor padrão do tipo.</returns>
    public static T Get<T>(this ICache cache)
    {
      var key = typeof(T).FullName;
      var value = cache.Get(key);
      return (value is T) ? (T)value : default(T);
    }

    /// <summary>
    /// Obtém um dado do tipo indicado estocado na chave indicada.
    /// Se o dado não existir o valor padrão do tipo é retornado.
    /// </summary>
    /// <typeparam name="T">O tipo do dado procurado.</typeparam>
    /// <param name="key">A chave sob a qual o dado está estocado.</param>
    /// <returns>O dado obtido ou o valor padrão do tipo.</returns>
    public static T Get<T>(this ICache cache, string key)
    {
      var value = cache.Get(key);
      return (value is T) ? (T)value : default(T);
    }

    /// <summary>
    /// Estoca o dado indicado.
    /// Para obter o objeto deve se usar GetEntry<T>() ou indicar o nome
    /// completo do tipo como chave, exemplo: GetEntry("NamespaceTal.TipoTal").
    /// </summary>
    /// <typeparam name="T">O tipo do dado estocado.</typeparam>
    /// <param name="value">O valor a ser estocado.</param>
    public static void Set<T>(this ICache cache, T value)
    {
      var key = typeof(T).FullName;
      cache.Set(key, value);
    }
  }
}