using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Toolset;

namespace Paper.Media
{
  /// <summary>
  /// Coleção de links.
  /// </summary>
  [CollectionDataContract(Namespace = Namespaces.Default, Name = "Headers")]
  public class HeaderCollection : List<Header>
  {
    /// <summary>
    /// Nome da propriedade de Entity que identifica a os cabeçalhos de propriedades da entidade.
    /// </summary>
    public const string DataHeadersName = "_DataHeaders";

    /// <summary>
    /// Nome da propriedade de Entity que identifica a coleção de cabeçalhos da grade.
    /// </summary>
    public const string RowsHeadersName = "_RowsHeaders";

    public HeaderCollection()
    {
    }

    public HeaderCollection(IEnumerable<Header> items)
    : base(items)
    {
    }

    public Header this[string name]
    {
      get => this.FirstOrDefault(x => x.Name.Value.EqualsIgnoreCase(name));
      set {
        this.RemoveAll(x => x.Name.Value.EqualsIgnoreCase(name));
        this.Add(value);
      }
    }

    public Header Add(string name)
    {
      var header = new Header { Name = name };
      Add(header);
      return header;
    }

    public Header Add(string name, string title, string type)
    {
      var header = new Header { Name = name, Title = title, Type = type };
      Add(header);
      return header;
    }
  }
}