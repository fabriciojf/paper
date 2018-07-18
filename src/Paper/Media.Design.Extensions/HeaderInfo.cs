using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paper.Media.Design.Extensions
{
  /// <summary>
  /// Coleção de propriedades de uma coluna de dados.
  /// </summary>
  public class HeaderInfo
  {
    private readonly PropertyCollection properties;

    public HeaderInfo()
    {
      this.properties = new PropertyCollection();
    }

    public HeaderInfo(PropertyCollection properties)
    {
      this.properties = properties;
    }

    private T Get<T>(string property)
    {
      var value = properties[property]?.Value;
      return (value is T) ? (T)value : default(T);
    }

    private void Set(string property, object value)
    {
      properties[property] = new Property { Value = value };
    }

    /// <summary>
    /// Nome da coluna.
    /// </summary>
    public string Name
    {
      get => Get<string>(nameof(Name));
      set => Set(nameof(Name), value);
    }

    /// <summary>
    /// Título da coluna.
    /// </summary>
    public string Title
    {
      get => Get<string>(nameof(Title));
      set => Set(nameof(Title), value);
    }

    /// <summary>
    /// Tipo do dado da coluna.
    /// </summary>
    public string DataType
    {
      get => Get<string>(nameof(DataType));
      set => Set(nameof(DataType), value);
    }

    /// <summary>
    /// Marca ou desmarca a coluna como visível.
    /// </summary>
    public bool? Hidden
    {
      get => Get<bool?>(nameof(Hidden));
      set => Set(nameof(Hidden), value);
    }

    /// <summary>
    /// Copia as propriedades da coluna para a coleção de propriedades indicada.
    /// </summary>
    /// <param name="properties">A coleção de propriedade destino.</param>
    public void CopyToPropertyCollection(PropertyCollection properties)
    {
      var names = GetType().GetProperties().Select(x => x.Name);
      foreach (var name in names)
      {
        var value = GetType().GetProperty(name).GetValue(this);
        if (value != null)
        {
          properties[name] = new Property { Value = value };
        }
        else
        {
          properties.Remove(name);
        }
      }
    }

    /// <summary>
    /// Copia as propriedades definidas para a coleção de opções de coluna indicada.
    /// </summary>
    /// <param name="options">As opções de coluna.</param>
    public void CopyToHeaderOptions(HeaderOptions options)
    {
      if (Title != null)
        options.AddTitle(Title);

      if (DataType != null)
        options.AddDataType(DataType);

      if (Hidden == true)
        options.AddHidden();
    }
  }
}