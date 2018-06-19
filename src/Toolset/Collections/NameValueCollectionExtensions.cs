using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolset.Collections
{
  /// <summary>
  /// Extensões para um NameValueCollection.
  /// </summary>
  public static class NameValueCollectionExtensions
  {
    /// <summary>
    /// Obtém o valor de chave no formato especificado.
    /// Se o valor não existir ou não for conversivel o valor padrão indicado será retornado.
    /// </summary>
    /// <typeparam name="T">O tipo esperado como retorno.</typeparam>
    /// <param name="collection">A coleção de nome/valor..</param>
    /// <param name="key">A chave procurada.</param>
    /// <param name="defaultValue">
    /// O valor padrão retornado.
    /// Se omitido o valor padrão do tipo será usado.
    /// </param>
    /// <returns>O valor convertido ou o valor padrão indicado.</returns>
    public static T GetValue<T>(this NameValueCollection collection, string key, T defaultValue = default(T))
    {
      var text = collection[key];
      if (text == null)
        return defaultValue;

      var value = text.ToOrDefault<T>();
      return value;
    }

    /// <summary>
    /// Define o valor de uma chave em uma coleção NameValueCollection.
    /// O valor é convertido para string segundo as conveções de Toolset.TextConvention.
    /// </summary>
    /// <typeparam name="T">O tipo esperado como retorno.</typeparam>
    /// <param name="collection">A coleção de nome/valor..</param>
    /// <param name="key">A chave procurada.</param>
    /// <param name="value">O valor a ser convertido.</param>
    public static void SetValue<T>(this NameValueCollection collection, string key, T value)
    {
      var options = TextConvention.Options.IgnoreQuotes;
      var text = TextConvention.ToString(value, options);
      collection[key] = text;
    }
  }
}
