using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolset.Text.Template
{
  /// <summary>
  /// Adaptador para suporte à coleções de nome/valor.
  /// </summary>
  class NameValueCollectionAdapter : IKeyValueCollection
  {
    private readonly NameValueCollection collection;

    public NameValueCollectionAdapter(NameValueCollection collection)
    {
      this.collection = collection;
    }

    /// <summary>
    /// Nomes conhecidos na coleção.
    /// </summary>
    public IEnumerable<string> KeyNames => this.collection.AllKeys;

    /// <summary>
    /// Valor associado a um nome na coleção.
    /// Caso o nome não exista nulo é retornado.
    /// </summary>
    /// <param name="name">Nome procurado na coleção.</param>
    /// <returns>
    /// Valor associado a um nome na coleção.
    /// Caso o nome não exista nulo é retornado.
    /// </returns>
    public object GetValue(string name)
    {
      return this.collection[name];
    }
  }
}
