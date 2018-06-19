using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Text.Template
{
  /// <summary>
  /// Métodos utilitários para coleções e mapas.
  /// </summary>
  internal static class CollectionExtensions
  {
    /// <summary>
    /// Utilitário para obtenção de um valor contido em um dicionário.
    /// Se valor não existir será retornado o valor padrão do tipo.
    /// </summary>
    /// <typeparam name="TKey">Tipo da chave.</typeparam>
    /// <typeparam name="TValue">Tipo do valor estocado.</typeparam>
    /// <param name="map">Instância do dicionário.</param>
    /// <param name="key">Chave pesquisada.</param>
    /// <returns>Valor encontrado, caso contrário, valor padrão do tipo.</returns>
    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key)
    {
      return map.ContainsKey(key) ? map[key] : default(TValue);
    }
  }
}
