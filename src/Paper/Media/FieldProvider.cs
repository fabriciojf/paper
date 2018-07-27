using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Paper.Media
{
  /// <summary>
  /// Provedor de dados de um campo.
  /// </summary>
  [DataContract(Namespace = Namespaces.Default)]
  public class FieldProvider
  {
    /// <summary>
    /// URL de referência do provedor de dados.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 10)]
    public string Href { get; set; }

    /// <summary>
    /// Nome das chaves de relacionamento entre o dado e
    /// o campo.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 20)]
    public NameCollection Keys { get; set; }
  }
}
