using System;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Collections
{
  /// <summary>
  /// Extensões para dicionários.
  /// </summary>
  public static class DictionaryExtensions
  {
    /// <summary>
    /// Tenta obter o valor da chave.
    /// Se a chave não existir o valor padrão do tipo é retornado.
    /// </summary>
    /// <typeparam name="TKey">O tipo da chave.</typeparam>
    /// <typeparam name="TValue">O tipo do valor.</typeparam>
    /// <param name="dictionary">A instância de dicionário.</param>
    /// <param name="key">A chave pesquisada.</param>
    /// <returns>O valor da chave ou o valor padrão do tipo, se a chave não for encontrada.</returns>
    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
      return dictionary.ContainsKey(key) ? dictionary[key] : default(TValue);
    }

    /// <summary>
    /// Tenta obter o valor da chave.
    /// Se a chave não existir o valor padrão indicado é retornado.
    /// </summary>
    /// <typeparam name="TKey">O tipo da chave.</typeparam>
    /// <typeparam name="TValue">O tipo do valor.</typeparam>
    /// <param name="dictionary">A instância de dicionário.</param>
    /// <param name="key">A chave pesquisada.</param>
    /// <param name="defaultValue">Valor padrão retornado case a chave não seja encontrada.</param>
    /// <returns>O valor da chave ou o valor padrão indicado, se a chave não for encontrada.</returns>
    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
      return dictionary.ContainsKey(key) ? dictionary[key] : defaultValue;
    }

    /// <summary>
    /// Acrescenta vários itens ao mapa.
    /// </summary>
    /// <typeparam name="TKey">O tipo da chave.</typeparam>
    /// <typeparam name="TValue">O tipo do valor.</typeparam>
    /// <param name="dictionary">O mapa a ser modificado.</param>
    /// <param name="items">Os itens a serem inseridos.</param>
    public static void AddMany<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
      items.ForEach(item => dictionary[item.Key] = item.Value);
    }
  }
}