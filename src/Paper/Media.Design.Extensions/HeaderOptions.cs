using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Extensions
{
  /// <summary>
  /// Construtor de uma entidade que representa um cabeçalho.
  /// </summary>
  public class HeaderOptions
  {
    private readonly Entity entity;

    internal HeaderOptions(Entity entity)
    {
      this.entity = entity;
    }

    /// <summary>
    /// Nome da coluna.
    /// </summary>
    public string Name => entity.Properties?["Name"]?.Value as string;

    /// <summary>
    /// Adiciona o título do cabeçalho.
    /// </summary>
    /// <param name="builder">O construtor do cabeçalho.</param>
    /// <param name="title">O valor do campo.</param>
    /// <returns>A própria instância do construtor do cabeçalho.</returns>
    public HeaderOptions AddTitle(string title)
    {
      entity.AddProperty("Title", title);
      return this;
    }

    /// <summary>
    /// Adiciona o tipo de dado de um campo.
    /// </summary>
    /// <param name="builder">O construtor do cabeçalho.</param>
    /// <param name="dataType">O valor do campo.</param>
    /// <returns>A própria instância do construtor do cabeçalho.</returns>
    public HeaderOptions AddDataType(string dataType)
    {
      entity.AddProperty("DataType", dataType);
      return this;
    }

    /// <summary>
    /// Marca ou desmarca um campo como visível.
    /// </summary>
    /// <param name="builder">O construtor do cabeçalho.</param>
    /// <param name="hidden">O valor do campo.</param>
    /// <returns>A própria instância do construtor do cabeçalho.</returns>
    public HeaderOptions AddHidden(bool hidden = true)
    {
      if (hidden)
      {
        entity.AddProperty("Hidden", hidden);
      }
      else
      {
        entity.Properties?.Remove("Hidden");
      }
      return this;
    }
  }
}