using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolset.Text.Template
{
  /// <summary>
  /// Adaptador para suporte à dicionários.
  /// Apenas dicionários com chaves do tipo string é suportado.
  /// Para dicionários com chaves de outros tipos nenhum valor é realmente obtido.
  /// </summary>
  class DictionaryAdapter : IKeyValueCollection
  {
    private readonly IDictionary dictionary;

    public DictionaryAdapter(IDictionary dictionary)
    {
      this.dictionary = dictionary;
    }

    /// <summary>
    /// Nomes conhecidos no dicionario.
    /// </summary>
    public IEnumerable<string> KeyNames => this.dictionary.Keys.OfType<string>();

    /// <summary>
    /// Valor associado a um nome no dicionario.
    /// Caso o nome não exista nulo é retornado.
    /// </summary>
    /// <param name="name">Nome procurado no dicionario.</param>
    /// <returns>
    /// Valor associado a um nome no dicionario.
    /// Caso o nome não exista nulo é retornado.
    /// </returns>
    public object GetValue(string name)
    {
      return this.dictionary.Contains(name) ? this.dictionary[name] : null;
    }
  }
}
