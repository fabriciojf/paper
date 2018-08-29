using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Toolset;

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
  public class EntityAction : IMediaObject
  {
    private string _title { get; set; }

    /// <summary>
    /// Tipo da ação. Opcional.
    /// 
    /// Mais de um tipo pode ser indicado quando a ação se comporta
    /// de mais de uma forma.
    /// 
    /// A ordem dos tipos importa. O aplicativo cliente pode
    /// optar por refletir apenas o primeiro tipo definido, ou o
    /// formato de serialização pode não suportar mais de um tipo.
    /// Sempre defina o tipo primário antes dos tipos alternativos.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 10)]
    public NameCollection Class { get; set; }

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
    public string Title
    {
      get => _title ?? (Name ?? "").ChangeCase(TextCase.ProperCase);
      set => _title = value;
    }

    /// <summary>
    /// Natureza da relação entre a ação e a entidade, segundo o modelo
    /// Web Linking (RFC5988).
    /// 
    /// Para uma lista de relações pré-definidas consulte a seção;
    /// -   6.2.2. Initial Registry Contents" do RFC5988:
    ///     -   https://tools.ietf.org/html/rfc5988#section-6.2.2
    /// 
    /// As relações mais comuns estão disponíveis na classe RelValues.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 35)]
    public NameCollection Rel { get; set; }

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