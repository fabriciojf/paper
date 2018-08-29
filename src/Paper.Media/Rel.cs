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
    /// Relacionamento entre um link e um cabeçalho.
    /// </summary>
    HeaderLink,

    /// <summary>
    /// Estabecele relacionamento entre o alvo e uma proprieade da entidade.
    /// </summary>
    Property,

    /// <summary>
    /// Relacionamento entre o registro e a entidade pai.
    /// </summary>
    Row,

    /// <summary>
    /// Estabelece relacionamento entre o alvo e um cabeçalho de dados.
    /// </summary>
    DataHeader,

    /// <summary>
    /// Estabelece relacionamento entre o alvo e um cabeçalho de registro.
    /// </summary>
    RowHeader,

    /// <summary>
    /// Relaciona um link a um dado.
    /// </summary>
    DataLink,

    /// <summary>
    /// Relaciona um link a um registro.
    /// </summary>
    RowLink,

    /// <summary>
    /// Representação de um link primário, numa coleção de links
    /// </summary>
    PrimaryLink,

    #endregion
  }
}