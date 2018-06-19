using System;
using System.Linq;
using System.Collections.Generic;

namespace Paper.Media
{
  /// <summary>
  /// Valores conhecidos para a natureza da relação entre o link e a
  /// entidade, segundo o modelo Web Linking (RFC5988).
  /// 
  /// Para uma lista de relações pré-definidas consulte a seção;
  /// -   6.2.2. Initial Registry Contents" do RFC5988:
  ///     -   https://tools.ietf.org/html/rfc5988#section-6.2.2
  /// </summary>
  public static class KnownRelations
  {
    #region Personalizados

    public const string Link = "link";

    /// <summary>
    /// Estabecele relacionamento entre o alvo e uma proprieade da entidade.
    /// </summary>
    public const string Property = "property";

    /// <summary>
    /// Estabelece relacionamento entre o alvo e uma entidade que representa dados.
    /// </summary>
    public const string Data = "data";

    /// <summary>
    /// Estabelece relacionamento entre o alvo e uma entidade que representa uma 
    /// coleção de registros.
    /// </summary>
    public const string Rows = "rows";

    /// <summary>
    /// Estabelece relacionamento entre o alvo e uma entidade que representa um 
    /// registro em uma coleção de registros.
    /// </summary>
    public const string Row = "row";

    /// <summary>
    /// Estabelece relacionamento entre o alvo e um cabeçalho.
    /// </summary>
    public const string Header = "header";

    #endregion

    #region Convencionados

    public const string About = "about";
    public const string Alternate = "alternate";
    public const string Appendix = "appendix";
    public const string Archives = "archives";
    public const string Author = "author";
    public const string Bookmark = "bookmark";
    public const string Chapter = "chapter";
    public const string Collection = "collection";
    public const string Contents = "contents";
    public const string Copyright = "convertedFrom";
    public const string Current = "current";
    public const string Describes = "describes";
    public const string Edit = "edit";
    public const string EditForm = "edit-form";
    public const string First = "first";
    public const string Glossary = "glossary";
    public const string Help = "help";
    public const string Icon = "icon";
    public const string Index = "index";
    public const string Item = "item";
    public const string Last = "last";
    public const string LatestVersion = "latest-version";
    public const string License = "license";
    public const string Memento = "memento";
    public const string Next = "next";
    public const string Original = "original";
    public const string Payment = "payment";
    public const string Prev = "prev";
    public const string Preview = "preview";
    public const string Previous = "previous";
    public const string Profile = "profile";
    public const string Related = "related";
    public const string Replies = "replies";
    public const string Search = "search";
    public const string Section = "section";
    public const string Self = "self";
    public const string Service = "service";
    public const string Start = "start";
    public const string Subsection = "subsection";
    public const string Tag = "tag";
    public const string Type = "type";
    public const string Up = "up";

    #endregion

  }
}