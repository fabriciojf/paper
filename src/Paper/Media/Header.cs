using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Toolset;

namespace Paper.Media
{
  /// <summary>
  /// Representação de um link associado a uma entidade.
  /// </summary>
  [DataContract(Namespace = Namespaces.Default)]
  public class Header
  {
    private string _title;
    private string _type;

    /// <summary>
    /// Título do link. Opcional.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 10)]
    public CaseVariantString Name { get; set; }

    /// <summary>
    /// Título do campo.
    /// Se omitido o nome do campo será usado.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 20)]
    public string Title
    {
      get => _title ?? Name?.ChangeCase(TextCase.ProperCase);
      set => _title = value;
    }

    /// <summary>
    /// Tipo da coluna.
    /// Um dos valores conhecidos em: Domain.KnownFieldDataTypes.
    /// Se omitido o tipo será "texto".
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 30)]
    public string Type
    {
      get => _type ?? KnownFieldDataTypes.Text;
      set => _type = value;
    }

    public override string ToString()
    {
      return $"{Name}={Type}:{Title}";
    }
  }
}
