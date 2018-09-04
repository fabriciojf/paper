using System;
using System.Linq;
using System.Collections.Generic;

namespace Paper.Media
{
  /// <summary>
  /// Classes conhecidas de entidades.
  /// </summary>
  public enum Class
  {
    /// <summary>
    /// Nome de classe para ume entidade que se comporta como dados.
    /// </summary>
    Data,

    /// <summary>
    /// Nome de classe para ume entidade que se comporta como uma coleção de registros.
    /// </summary>
    Rows,

    /// <summary>
    /// Nome de classe para ume entidade que se comporta como um registro de coleção de registros.
    /// </summary>
    Row,

    /// <summary>
    /// Nome de classe para ume entidade que se comporta como uma coleção de cards.
    /// </summary>
    Cards,

    /// <summary>
    /// Nome de classe para ume entidade que se comporta como um registro de coleção de cards.
    /// </summary>
    Card,

    /// <summary>
    /// Nome de classe para ume entidade que se comporta como um cabeçalho.
    /// </summary>
    Header,

    /// <summary>
    /// Nome de classe para uma entidade que se comporta como status de execução.
    /// </summary>
    Status,

    /// <summary>
    /// Nome de classe para uma entidade que se comporta como erro.
    /// </summary>
    Error,

    /// <summary>
    /// Nome de classe para uma entidade que se comporta como lista.
    /// </summary>
    List,

    /// <summary>
    /// Nome de classe para uma entidade que se comporta como um único objeto.
    /// </summary>
    Single,

    /// <summary>
    /// Nome de classe para uma entidade que se comporta como um item de lista.
    /// </summary>
    Item,

    /// <summary>
    /// Nome de classe para uma ação que se comporta como filtro de lista.
    /// </summary>
    Filter,

    /// <summary>
    /// Nome de classe para uma ação, entidade ou link que se comporta como um hiperlink.
    /// </summary>
    Hyperlink,

    /// <summary>
    /// Nome de classe para uma entidade que transporta um valor literal apenas, como um 
    /// texto, número, etc.
    /// </summary>
    Literal,

    /// <summary>
    /// Classe de uma entidade que representa um propriedade ou coluna de dados.
    /// </summary>
    Field,
  }
}