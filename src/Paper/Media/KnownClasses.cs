using System;
using System.Linq;
using System.Collections.Generic;

namespace Paper.Media
{
  /// <summary>
  /// Classes conhecidas de entidades.
  /// </summary>
  public static class KnownClasses
  {
    /// <summary>
    /// Nome de classe para ume entidade que se comporta como dados.
    /// </summary>
    public const string Data = "data";

    /// <summary>
    /// Nome de classe para ume entidade que se comporta como uma coleção de registros.
    /// </summary>
    public const string Rows = "rows";

    /// <summary>
    /// Nome de classe para ume entidade que se comporta como um registro de coleção de registros.
    /// </summary>
    public const string Row = "row";

    /// <summary>
    /// Nome de classe para uma entidade que se comporta como status de execução.
    /// </summary>
    public const string Status = "status";

    /// <summary>
    /// Nome de classe para uma entidade que se comporta como erro.
    /// </summary>
    public const string Error = "error";

    /// <summary>
    /// Nome de classe para uma entidade que se comporta como lista.
    /// </summary>
    public const string List = "list";

    /// <summary>
    /// Nome de classe para uma entidade que se comporta como um único objeto.
    /// </summary>
    public const string Single = "single";

    /// <summary>
    /// Nome de classe para uma entidade que se comporta como um item de lista.
    /// </summary>
    public const string Item = "item";

    /// <summary>
    /// Nome de classe para uma ação que se comporta como filtro de lista.
    /// </summary>
    public const string Filter = "filter";

    /// <summary>
    /// Nome de classe para uma ação, entidade ou link que se comporta como um hiperlink.
    /// </summary>
    public const string Hyperlink = "hyperlink";

    /// <summary>
    /// Nome de classe para uma entidade que transporta um valor literal apenas, como um 
    /// texto, número, etc.
    /// </summary>
    public const string Literal = "literal";

    /// <summary>
    /// Classe de uma entidade que representa um propriedade ou coluna de dados.
    /// </summary>
    public const string Field = "field";
  }
}