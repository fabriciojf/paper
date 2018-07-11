using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Paper.Media
{
  [DataContract(Namespace = Namespaces.Default, Name = "Entity")]
  public class Entity : IEntity, IMediaObject
  {
    /// <summary>
    /// Tipo da entidade.
    /// 
    /// Mais de um tipo pode ser indicado para
    /// classes com mais de um comportamento.
    /// 
    /// A ordem dos tipos importa. O aplicativo cliente pode
    /// optar por refletir apenas o primeiro tipo definido, ou o
    /// formato de serialização pode não suportar mais de um tipo.
    /// Sempre defina o tipo primário antes dos tipos alternativos.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 10)]
    public virtual NameCollection Class { get; set; }

    /// <summary>
    /// Texto descritivo sobre a entidade. Opcional.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 20)]
    public virtual string Title { get; set; }

    /// <summary>
    /// Determina o relacionamento da subentidade com a sua entidade principal.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 30)]
    public virtual NameCollection Rel { get; set; }

    /// <summary>
    /// Coleção das propriedades da entidade. Opcional.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 40)]
    public virtual PropertyCollection Properties { get; set; }

    /// <summary>
    /// Coleção das demais entidades relacionadas à entidade. Opcional.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 50)]
    public virtual EntityCollection Entities { get; set; }

    /// <summary>
    /// Coleção das ações aplicáveis à entidade. Opcional.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 60)]
    public virtual EntityActionCollection Actions { get; set; }

    /// <summary>
    /// Coleção dos links de navegação associados à entidade. Opcional.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 70)]
    public virtual LinkCollection Links { get; set; }
  }
}