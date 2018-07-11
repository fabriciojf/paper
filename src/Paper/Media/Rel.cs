using System;
using System.Collections.Generic;
using System.Text;
using Toolset;

namespace Paper.Media
{
  public enum Rel
  {
    #region Convencionados

    About = 1,
    Alternate,
    Appendix,
    Archives,
    Author,
    Bookmark,
    Chapter,
    Collection,
    Contents,
    Copyright,
    Current,
    Describes,
    Edit,
    EditForm,
    First,
    Glossary,
    Help,
    Icon,
    Index,
    Item,
    Last,
    LatestVersion,
    License,
    Memento,
    Next,
    Original,
    Payment,
    Prev,
    Preview,
    Previous,
    Profile,
    Related,
    Replies,
    Search,
    Section,
    Self,
    Service,
    Start,
    Subsection,
    Tag,
    Type,
    Up,

    #endregion

    #region Personalizados

    Link = 1000,

    /// <summary>
    /// Estabecele relacionamento entre o alvo e uma proprieade da entidade.
    /// </summary>
    Property,

    /// <summary>
    /// Estabelece relacionamento entre o alvo e uma entidade que representa dados.
    /// </summary>
    Data,

    /// <summary>
    /// Estabelece relacionamento entre o alvo e uma entidade que representa uma 
    /// coleção de registros.
    /// </summary>
    Rows,

    /// <summary>
    /// Estabelece relacionamento entre o alvo e uma entidade que representa um 
    /// registro em uma coleção de registros.
    /// </summary>
    Row,

    /// <summary>
    /// Estabelece relacionamento entre o alvo e um cabeçalho.
    /// </summary>
    Header,

    #endregion
  }
}