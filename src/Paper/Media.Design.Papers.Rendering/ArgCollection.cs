using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset;
using Toolset.Collections;

namespace Paper.Media.Design.Papers.Rendering
{
  public class ArgCollection : IEnumerable<KeyValuePair<string, string>>
  {
    private readonly Map<string, string> map;

    public ArgCollection()
    {
      this.map = new Map<string, string>();
    }

    public ArgCollection(IEnumerable<KeyValuePair<string, string>> items)
    {
      this.map = new Map<string, string>(items);
    }

    public ICollection<string> Keys => map.Keys;

    public string this[string key]
    {
      get => Get(key);
      set => Set(key, value);
    }

    /// <summary>
    /// Determina se o argumento existe.
    /// </summary>
    /// <param name="key">O nome do argumento.</param>
    /// <returns>
    /// <c>true</c> se o argumento existe ou <c>false</c>, caso contrário.
    /// </returns>
    public bool ContainsKey(string key)
    {
      return map.ContainsKey(key);
    }

    /// <summary>
    /// Obtém o valor do argumento convertido para o tipo indicado.
    /// Se o valor não existir ou não for conversível o valor padrão
    /// do tipo é retornado.
    /// </summary>
    /// <typeparam name="T">O tipo do valor esperado.</typeparam>
    /// <param name="key">O nome do argumento.</param>
    /// <returns>O valor do argumento.</returns>
    public T Get<T>(string key)
    {
      var value = this.map[key];
      if (value == null)
        return default(T);

      if (typeof(T).IsArray)
      {
        if (value.Contains(";"))
          return (T)(object)value.Split(';').Select(x => x.Trim()).ToArray();
        else
          return (T)(object)value.Split(',').Select(x => x.Trim()).ToArray();
      }

      return Change.To<T>(value);
    }

    /// <summary>
    /// Obtém o valor do argumento estocado.
    /// </summary>
    /// <param name="key">O nome do argumento.</param>
    /// <returns>O valor do argumento.</returns>
    public string Get(string key)
    {
      return this.map[key];
    }

    /// <summary>
    /// Define o valor do agumento.
    /// </summary>
    /// <param name="key">O nome do argumento.</param>
    /// <param name="value">O valor do argumento.</param>
    public void Set(string key, string value)
    {
      this.map[key] = value;
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
      return this.map.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.map.GetEnumerator();
    }
  }
}