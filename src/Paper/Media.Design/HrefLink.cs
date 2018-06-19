using System;
using System.Collections.Generic;
using System.Text;
using Toolset.Reflection;

namespace Paper.Media.Design
{
  public class HrefLink : ILink
  {
    /// <summary>
    /// Tipo do link. Opcional.
    /// 
    /// Mais de um tipo pode ser indicado quando o link se comporta
    /// de mais de uma forma.
    /// 
    /// A ordem dos tipos importa. O aplicativo cliente pode
    /// optar por refletir apenas o primeiro tipo definido, ou o
    /// formato de serialização pode não suportar mais de um tipo.
    /// Sempre defina o tipo primário antes dos tipos alternativos.
    /// </summary>
    public NameCollection Class { get; set; }

    /// <summary>
    /// Título do link. Opcional.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Natureza da relação entre o link e a entidade, segundo o modelo
    /// Web Linking (RFC5988).
    /// 
    /// Para uma lista de relações pré-definidas consulte a seção;
    /// -   6.2.2. Initial Registry Contents" do RFC5988:
    ///     -   https://tools.ietf.org/html/rfc5988#section-6.2.2
    /// 
    /// As relações mais comuns estão disponíveis na classe RelValues.
    /// </summary>
    public NameCollection Rel { get; set; }

    /// <summary>
    /// URL de referência do link.
    /// </summary>
    public string Href { get; set; }

    /// <summary>
    /// Tipo do conteúdo oferecido pelo link.
    /// Um mimetype como "text/json", etc.
    /// </summary>
    public string Type { get; set; }
  }
}
