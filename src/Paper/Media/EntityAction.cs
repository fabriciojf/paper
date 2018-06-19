using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Paper.Media
{
  /// <summary>
  /// Definição de uma ação aplicada sobre uma entidade.
  /// Uma ação geralmente modifica o estado de uma entidade e
  /// contém informação suficiente para o aplicativo cliente
  /// captar os dados do usuário e enviar para o serviço de
  /// execução da ação.
  /// 
  /// Ações são comumente usadas na criação de forms HTML.
  /// </summary
  [DataContract(Namespace = Namespaces.Default, Name = "Action")]
  public class EntityAction
  {
    /// <summary>
    /// Nome da ação.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 20)]
    [CaseVariantString]
    public string Name { get; set; }

    /// <summary>
    /// Título da ação. Opcional.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 30)]
    public string Title { get; set; }

    /// <summary>
    /// Método HTTP. Como POST, PUT, GET, etc. Opcional.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 40)]
    public string Method { get; set; }

    /// <summary>
    /// URL do serviço de processamento da ação.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 50)]
    public string Href { get; set; }

    /// <summary>
    /// Tipo do conteúdo submetido ao servidor.
    /// Deve ser um mimetype válido, como "text/json", etc.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 60)]
    public string Type { get; set; }

    /// <summary>
    /// Coleção dos campos que compõem o form usado na coleta dos
    /// dados que acompanham a ação. Opcional.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 70)]
    public FieldCollection Fields { get; set; }
  }
}