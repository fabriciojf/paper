using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Toolset;
using Toolset.Collections;

namespace Paper.Media.Design
{
  /// <summary>
  /// Extensões de desenho de instâncias de Field
  /// </summary>
  public static class FieldExtensions
  {
    /// <summary>
    /// Adiciona um texto substitudo para o campo.
    /// Categorias são usadas como agrupadores de campos.
    /// </summary>
    /// <param name="field">O campo a ser modificado.</param>
    /// <param name="placeholder">O valor do campo.</param>
    /// <returns>A própria instância do campo modificado.</returns>
    public static Field AddPlaceholder(this Field field, string placeholder)
    {
      field.Placeholder = placeholder;
      return field;
    }

    /// <summary>
    /// Adiciona uma categoria ao campo.
    /// Categorias são usadas como agrupadores de campos.
    /// </summary>
    /// <param name="field">O campo a ser modificado.</param>
    /// <param name="category">A categoria do campo.</param>
    /// <returns>A própria instância do campo modificado.</returns>
    public static Field AddCategory(this Field field, string category)
    {
      field.Category = category;
      return field;
    }

    /// <summary>
    /// Define o tipo de dado do campo.
    /// </summary>
    /// <param name="field">O campo a ser modificado.</param>
    /// <param name="dataType">A categoria do campo.</param>
    /// <returns>A própria instância do campo modificado.</returns>
    public static Field AddDataType(this Field field, string dataType)
    {
      field.DataType = dataType;
      return field;
    }

    /// <summary>
    /// Define o tipo de dado do campo.
    /// </summary>
    /// <param name="field">O campo a ser modificado.</param>
    /// <param name="dataType">A categoria do campo.</param>
    /// <returns>A própria instância do campo modificado.</returns>
    public static Field AddDataType(this Field field, DataType dataType)
    {
      field.DataType = dataType.GetName();
      return field;
    }

    /// <summary>
    /// Marca ou desmarca o campo como somente leitura.
    /// </summary>
    /// <param name="field">O campo a ser modificado.</param>
    /// <param name="readOnly">O valor da propriedade somente leitura do campo.</param>
    /// <returns>A própria instância do campo modificado.</returns>
    public static Field AddReadOnly(this Field field, bool readOnly = true)
    {
      field.ReadOnly = readOnly;
      return field;
    }

    /// <summary>
    /// Marca ou desmarca o campo como requerido.
    /// </summary>
    /// <param name="field">O campo a ser modificado.</param>
    /// <param name="required">O valor da propriedade requerida do campo.</param>
    /// <returns>A própria instância do campo modificado.</returns>
    public static Field AddRequired(this Field field, bool required = true)
    {
      field.Required = required;
      return field;
    }

    /// <summary>
    /// Marca ou desmarca o campo como oculto.
    /// </summary>
    /// <param name="field">O campo a ser modificado.</param>
    /// <param name="hidden">O valor da propriedade oculta do campo.</param>
    /// <returns>A própria instância do campo modificado.</returns>
    public static Field AddHidden(this Field field, bool hidden = true)
    {
      field.Type = hidden ? FieldTypeNames.Hidden : null;
      return field;
    }

    /// <summary>
    /// Define o valor do campo.
    /// </summary>
    /// <param name="field">O campo a ser modificado.</param>
    /// <param name="value">O valor do campo.</param>
    /// <returns>A própria instância do campo modificado.</returns>
    public static Field AddValue(this Field field, object value)
    {
      field.Value = value;
      return field;
    }

    /// <summary>
    /// Habilita ou desabilita o suporte a múltiplos valores para o campo.
    /// </summary>
    /// <param name="field">O campo a ser modificado.</param>
    /// <param name="allowMany">O valor do campo.</param>
    /// <returns>A própria instância do campo modificado.</returns>
    public static Field AddAllowMany(this Field field, bool allowMany = true)
    {
      field.AllowMany = allowMany;
      return field;
    }

    /// <summary>
    /// Habilita ou desabilita o suporte a períodos, início e fim, para o campo.
    /// </summary>
    /// <param name="field">O campo a ser modificado.</param>
    /// <param name="allowRange">O valor do campo.</param>
    /// <returns>A própria instância do campo modificado.</returns>
    public static Field AddAllowRange(this Field field, bool allowRange = true)
    {
      field.AllowRange = allowRange;
      return field;
    }

    /// <summary>
    /// Habilita ou desabilita o suporte a caracteres curingas para o campo.
    /// </summary>
    /// <param name="field">O campo a ser modificado.</param>
    /// <param name="allowWildcards">O valor do campo.</param>
    /// <returns>A própria instância do campo modificado.</returns>
    public static Field AddAllowWildcards(this Field field, bool allowWildcards = true)
    {
      field.AllowWildcards = allowWildcards;
      return field;
    }

    /// <summary>
    /// Habilita ou desabilita o suporte a múltiplas linhas para o campo.
    /// </summary>
    /// <param name="field">O campo a ser modificado.</param>
    /// <param name="multiline">O valor do campo.</param>
    /// <returns>A própria instância do campo modificado.</returns>
    public static Field AddMultiline(this Field field, bool multiline = true)
    {
      field.Multiline = multiline;
      return field;
    }

    /// <summary>
    /// Define o tamanho máximo para o campo.
    /// </summary>
    /// <param name="field">O campo a ser modificado.</param>
    /// <param name="maxLength">O valor do campo.</param>
    /// <returns>A própria instância do campo modificado.</returns>
    public static Field AddMaxLength(this Field field, int maxLength)
    {
      field.MaxLength = maxLength;
      return field;
    }

    /// <summary>
    /// Define o tamanho mínimo para o campo.
    /// </summary>
    /// <param name="field">O campo a ser modificado.</param>
    /// <param name="minLength">O valor do campo.</param>
    /// <returns>A própria instância do campo modificado.</returns>
    public static Field AddMinLength(this Field field, int minLength)
    {
      field.MinLength = minLength;
      return field;
    }

    /// <summary>
    /// Define um padrão de texto para validação do campo.
    /// </summary>
    /// <param name="field">O campo a ser modificado.</param>
    /// <param name="pattern">O valor do campo.</param>
    /// <returns>A própria instância do campo modificado.</returns>
    public static Field AddPattern(this Field field, string pattern)
    {
      field.Pattern = pattern;
      return field;
    }

    /// <summary>
    /// Víncula um provedor de dados para o campo.
    /// </summary>
    /// <param name="field">O campo a ser modificado.</param>
    /// <param name="href">A rota do provedor de dados.</param>
    /// <param name="keys">Nomes dos campos chaves.</param>
    /// <returns>A própria instância do campo modificado.</returns>
    public static Field AddProvider(this Field field, string href, params string[] keys)
    {
      field.Provider = new FieldProvider();
      field.Provider.Href = href;
      field.Provider.Keys = new NameCollection(keys);
      return field;
    }

    /// <summary>
    /// Víncula um provedor de dados para o campo.
    /// </summary>
    /// <param name="field">O campo a ser modificado.</param>
    /// <param name="href">A rota do provedor de dados.</param>
    /// <param name="keys">Nomes dos campos chaves.</param>
    /// <returns>A própria instância do campo modificado.</returns>
    public static Field AddProvider(this Field field, string href, IEnumerable<string> keys)
    {
      if (keys == null)
      {
        keys = Enumerable.Empty<string>();
      }

      field.Provider = new FieldProvider();
      field.Provider.Href = href;
      field.Provider.Keys = new NameCollection(keys);
      return field;
    }
  }
}