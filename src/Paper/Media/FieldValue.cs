using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Paper.Media
{
  /// <summary>
  /// Representação de um item de campo multi-valorado.
  /// </summary>
  [DataContract(Namespace = Namespaces.Default)]
  [KnownType(typeof(CaseVariantString))]
  public class FieldValue
  {
    /// <summary>
    /// Título do item.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 10)]
    public string Title { get; set; }

    /// <summary>
    /// Texto adicional com uma breve instrução de uso do widget.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 15)]
    public string Placeholder { get; set; }

    /// <summary>
    /// Valor do item.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 20)]
    public object Value { get; set; }

    /// <summary>
    /// Ativa ou desativa a marcação de seleção do item.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 30)]
    public bool Selected { get; set; }
  }
}