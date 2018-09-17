using System.Collections;
using System.Collections.Generic;
using Toolset.Collections;

namespace Media.Utilities.Types
{
  /// <summary>
  /// Cache de dados para o contexto de renderização do Paper.
  /// </summary>
  /// <seealso cref="System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{System.String, System.Object}}" />
  public class EntryCollection : IEnumerable<KeyValuePair<string, object>>
  {
    private readonly HashMap map;

    public EntryCollection()
    {
      this.map = new HashMap();
    }

    public EntryCollection(IEnumerable<KeyValuePair<string, object>> entries)
    {
      this.map = new HashMap(entries);
    }

    /// <summary>
    /// Determina se um dado para o tipo indicado existe.
    /// </summary>
    /// <typeparam name="T">O tipo do dado estocado.</typeparam>
    /// <returns>
    /// <c>true</c> se o dado existe ou <c>false</c>, caso contrário.
    /// </returns>
    public bool Contains<T>()
    {
      var key = typeof(T).FullName;
      return map.ContainsKey(key);
    }

    /// <summary>
    /// Determina se o dado existe.
    /// </summary>
    /// <param name="key">A chave sob a qual o dado estaria estocado.</param>
    /// <returns>
    /// <c>true</c> se o dado existe ou <c>false</c>, caso contrário.
    /// </returns>
    public bool ContainsKey(string key)
    {
      return map.ContainsKey(key);
    }

    /// <summary>
    /// Obtém um dado do tipo indicado.
    /// Se o dado não existir o valor padrão do tipo é retornado.
    /// </summary>
    /// <typeparam name="T">O tipo do dado procurado.</typeparam>
    /// <returns>O dado obtido ou o valor padrão do tipo.</returns>
    public T Get<T>()
    {
      var key = typeof(T).FullName;
      var value = this.map[key];
      return (value is T) ? (T)value : default(T);
    }

    /// <summary>
    /// Obtém um dado do tipo indicado estocado na chave indicada.
    /// Se o dado não existir o valor padrão do tipo é retornado.
    /// </summary>
    /// <typeparam name="T">O tipo do dado procurado.</typeparam>
    /// <param name="key">A chave sob a qual o dado está estocado.</param>
    /// <returns>O dado obtido ou o valor padrão do tipo.</returns>
    public T Get<T>(string key)
    {
      var value = this.map[key];
      return (value is T) ? (T)value : default(T);
    }

    /// <summary>
    /// Obtém um dado estocado na chave indicada.
    /// Se o dado não existir nulo é retornado.
    /// </summary>
    /// <param name="key">A chave sob a qual o dado está estocado.</param>
    /// <returns>O dado obtido ou nulo.</returns>
    public object Get(string key)
    {
      return this.map[key];
    }

    /// <summary>
    /// Estoca o dado indicado.
    /// Para obter o objeto deve se usar GetEntry<T>() ou indicar o nome
    /// completo do tipo como chave, exemplo: GetEntry("NamespaceTal.TipoTal").
    /// </summary>
    /// <typeparam name="T">O tipo do dado estocado.</typeparam>
    /// <param name="value">O valor a ser estocado.</param>
    public void Set<T>(T value)
    {
      var key = typeof(T).FullName;
      this.map[key] = value;
    }

    /// <summary>
    /// Estoca o dado indicado sob a chave indicada.
    /// </summary>
    /// <param name="key">A chave de escotagem.</param>
    /// <param name="value">O valor a ser estocado.</param>
    public void Set(string key, object value)
    {
      this.map[key] = value;
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
      return this.map.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.map.GetEnumerator();
    }
  }
}