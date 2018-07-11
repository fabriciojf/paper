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
  /// Extensões de desenho de ações de objetos Entity.
  /// </summary>
  public static class EntityActionExtensions
  {
    #region Extensões de Entity

    /// <summary>
    /// Adiciona uma ação à entidade.
    /// </summary>
    /// <param name="entity">A entidade a ser modificada.</param>
    /// <param name="action">A instância da ação.</param>
    /// <returns>A própria instância da entidade modificada.</returns>
    public static Entity AddAction(this Entity entity, EntityAction action)
    {
      if (entity.Actions == null)
      {
        entity.Actions = new EntityActionCollection();
      }

      if (action.Name == null)
      {
        action.Name = MakeActionName(entity.Actions);
      }

      entity.Actions.RemoveAll(x => x.Name.EqualsIgnoreCase(action.Name));
      entity.Actions.Add(action);

      return entity;
    }

    /// <summary>
    /// Adiciona uma ação à entidade.
    /// </summary>
    /// <param name="entity">A entidade a ser modificada.</param>
    /// <param name="name">Nome da ação.</param>
    /// <param name="builder">Função de construção da ação.</param>
    /// <returns>A própria instância da entidade modificada.</returns>
    public static Entity AddAction(this Entity entity, string name, Action<EntityAction> builder)
    {
      if (entity.Actions == null)
      {
        entity.Actions = new EntityActionCollection();
      }

      var action = GetOrAddAction(entity.Actions, name);
      builder.Invoke(action);

      return entity;
    }

    /// <summary>
    /// Obtém a ação com o nome indicado ou cria uma nova.
    /// A ação criada é automaticamente inserida na lista de ações conhecidas.
    /// </summary>
    /// <param name="actions">A lista de ações conhecidas.</param>
    /// <param name="actionName">O nome da ação.</param>
    /// <returns>A ação obtida ou criada.</returns>
    private static EntityAction GetOrAddAction(EntityActionCollection actions, string actionName)
    {
      if (string.IsNullOrEmpty(actionName))
      {
        actionName = MakeActionName(actions);
      }

      var action = actions.FirstOrDefault(x => x.Name.EqualsIgnoreCase(actionName));
      if (action == null)
      {
        action = new EntityAction
        {
          Name = actionName,
          Title = actionName.Replace("Action", "Ação").ChangeCase(TextCase.ProperCase)
        };
        actions.Add(action);
      }

      return action;
    }

    /// <summary>
    /// Cria um nome único para uma ação.
    /// </summary>
    /// <param name="actions">A coleção das ações conhecidas.</param>
    /// <returns>O nome único para a ação.</returns>
    private static string MakeActionName(EntityActionCollection actions)
    {
      string actionName = null;

      var index = actions.Count + 1;
      do
      {
        actionName = $"Action{index}";
      } while (actions.Any(x => x.Name.EqualsIgnoreCase(actionName)));

      return actionName;
    }

    #endregion

    #region Extensões de EntityAction

    /// <summary>
    /// Define o tipo (MimeType) do documento retornado pelo link.
    /// Como "text/xml", "application/pdf", etc.
    /// </summary>
    /// <param name="action">A ação a ser modificada.</param>
    /// <param name="mediaType">O mime type do link.</param>
    /// <returns>A própria instância do link modificada.</returns>
    public static EntityAction AddMimeType(this EntityAction action, string mediaType)
    {
      action.Type = mediaType;
      return action;
    }

    /// <summary>
    /// Define o endpoint de destino da ação.
    /// Quando omitido a URL da ação é a mesma URL da sua entidade.
    /// </summary>
    /// <param name="action">A ação a ser modificada.</param>
    /// <param name="href">A URL de destino da ação.</param>
    /// <returns>A própria instância da ação modificada.</returns>
    public static EntityAction AddHref(this EntityAction action, string href)
    {
      action.Href = href;
      return action;
    }

    /// <summary>
    /// Define o método HTTP da ação.
    /// Por padrão o método usado é POST.
    /// Os valores possíveis são:
    /// -   GET
    /// -   POST
    /// -   PUT
    /// -   PATCH
    /// -   DELETE
    /// </summary>
    /// <param name="action">A ação a ser modificada.</param>
    /// <param name="method">O método HTTP.</param>
    /// <returns>A própria instância da ação modificada.</returns>
    public static EntityAction AddMethod(this EntityAction action, string method)
    {
      action.Method = method;
      return action;
    }

    /// <summary>
    /// Define o método HTTP da ação.
    /// Por padrão o método usado é POST.
    /// </summary>
    /// <param name="action">A ação a ser modificada.</param>
    /// <param name="method">O método HTTP.</param>
    /// <returns>A própria instância da ação modificada.</returns>
    public static EntityAction AddMethod(this EntityAction action, Method method)
    {
      action.Method = method.GetName();
      return action;
    }

    /// <summary>
    /// Adiciona um campo à ação.
    /// </summary>
    /// <param name="action">A ação a ser modificada.</param>
    /// <param name="field">O campo adicionado.</param>
    /// <returns>A própria instância da ação modificada.</returns>
    public static EntityAction AddField(this EntityAction action, Field field)
    {
      if (action.Fields == null)
      {
        action.Fields = new FieldCollection();
      }

      if (string.IsNullOrEmpty(field.Name))
      {
        field.Name = MakeFieldName(action.Fields);
      }

      action.Fields.RemoveAll(x => x.Name.EqualsIgnoreCase(field.Name));
      action.Fields.Add(field);

      return action;
    }

    /// <summary>
    /// Adiciona um campo à ação.
    /// </summary>
    /// <param name="action">A ação a ser modificada.</param>
    /// <param name="name">Nome do campo.</param>
    /// <param name="builder">Construtor do campo.</param>
    /// <returns>A própria instância da ação modificada.</returns>
    public static EntityAction AddField(this EntityAction action, string name, Action<Field> builder)
    {
      if (action.Fields == null)
      {
        action.Fields = new FieldCollection();
      }

      var field = GetOrAddField(action.Fields, name);
      builder.Invoke(field);

      return action;
    }

    /// <summary>
    /// Adiciona um campo à ação.
    /// </summary>
    /// <param name="action">A ação a ser modificada.</param>
    /// <param name="name">Nome do campo.</param>
    /// <param name="title">Título do campo.</param>
    /// <param name="value">Um valor opcional para o campo.</param>
    /// <returns>A própria instância da ação modificada.</returns>
    public static EntityAction AddField(this EntityAction action, string name, string title = null, object value = null)
    {
      if (action.Fields == null)
      {
        action.Fields = new FieldCollection();
      }

      var field = GetOrAddField(action.Fields, name);
      field.Title = title;
      field.Value = value;

      return action;
    }

    /// <summary>
    /// Adiciona um campo à ação.
    /// </summary>
    /// <param name="action">A ação a ser modificada.</param>
    /// <param name="name">Nome do campo.</param>
    /// <param name="title">Título do campo.</param>
    /// <param name="dataType">
    /// Tipo de dado do campo.
    /// Os valores possíveis são definidos nas constantes da classe <see cref="DataTypeNames"/>.
    /// </param>
    /// <param name="value">Um valor opcional para o campo.</param>
    /// <returns>A própria instância da ação modificada.</returns>
    public static EntityAction AddField(this EntityAction action, string name, string title, string dataType, object value = null)
    {
      if (action.Fields == null)
      {
        action.Fields = new FieldCollection();
      }

      var field = GetOrAddField(action.Fields, name);
      field.Title = title;
      field.DataType = dataType;
      field.Value = value;

      return action;
    }

    /// <summary>
    /// Adiciona um campo à ação.
    /// </summary>
    /// <param name="action">A ação a ser modificada.</param>
    /// <param name="name">Nome do campo.</param>
    /// <param name="title">Título do campo.</param>
    /// <param name="dataType">
    /// Tipo de dado do campo.
    /// Os valores possíveis são definidos nas constantes da classe <see cref="DataTypeNames"/>.
    /// </param>
    /// <param name="value">Um valor opcional para o campo.</param>
    /// <returns>A própria instância da ação modificada.</returns>
    public static EntityAction AddField(this EntityAction action, string name, string title, DataType dataType, object value = null)
    {
      if (action.Fields == null)
      {
        action.Fields = new FieldCollection();
      }

      var field = GetOrAddField(action.Fields, name);
      field.Title = title;
      field.DataType = dataType.GetName();
      field.Value = value;

      return action;
    }

    /// <summary>
    /// Adiciona um campo oculto à ação.
    /// </summary>
    /// <param name="action">A ação a ser modificada.</param>
    /// <param name="name">Nome do campo.</param>
    /// <param name="title">Título do campo.</param>
    /// <param name="dataType">
    /// Tipo de dado do campo.
    /// Os valores possíveis são definidos nas constantes da classe <see cref="DataTypeNames"/>.
    /// </param>
    /// <param name="value">Um valor opcional para o campo.</param>
    /// <returns>A própria instância da ação modificada.</returns>
    public static EntityAction AddFieldHidden(this EntityAction action, string name, string title, string dataType, object value = null)
    {
      if (action.Fields == null)
      {
        action.Fields = new FieldCollection();
      }

      var field = GetOrAddField(action.Fields, name);
      field.Title = title;
      field.DataType = dataType;
      field.Value = value;
      field.Type = FieldTypeNames.Hidden;

      return action;
    }

    /// <summary>
    /// Adiciona um campo oculto à ação.
    /// </summary>
    /// <param name="action">A ação a ser modificada.</param>
    /// <param name="name">Nome do campo.</param>
    /// <param name="title">Título do campo.</param>
    /// <param name="dataType">
    /// Tipo de dado do campo.
    /// Os valores possíveis são definidos nas constantes da classe <see cref="DataTypeNames"/>.
    /// </param>
    /// <param name="value">Um valor opcional para o campo.</param>
    /// <returns>A própria instância da ação modificada.</returns>
    public static EntityAction AddFieldHidden(this EntityAction action, string name, string title, DataType dataType, object value = null)
    {
      if (action.Fields == null)
      {
        action.Fields = new FieldCollection();
      }

      var field = GetOrAddField(action.Fields, name);
      field.Title = title;
      field.DataType = dataType.GetName();
      field.Value = value;
      field.Type = FieldTypeNames.Hidden;

      return action;
    }

    /// <summary>
    /// Obtém o campo com o nome indicado ou cria um novo.
    /// O campo criado é automaticamente inserido na lista de campos conhecidos.
    /// </summary>
    /// <param name="fields">A lista de campos conhecidos.</param>
    /// <param name="fieldName">O nome do campo.</param>
    /// <returns>O campo obtido ou criado.</returns>
    private static Field GetOrAddField(FieldCollection fields, string fieldName)
    {
      if (string.IsNullOrEmpty(fieldName))
      {
        fieldName = MakeFieldName(fields);
      }

      var field = fields.FirstOrDefault(x => x.Name.EqualsIgnoreCase(fieldName));
      if (field == null)
      {
        field = new Field
        {
          Name = fieldName,
          Title = fieldName.Replace("Field", "Campo").ChangeCase(TextCase.ProperCase)
        };
        fields.Add(field);
      }

      return field;
    }

    /// <summary>
    /// Cria um nome único para um campo.
    /// </summary>
    /// <param name="fields">A coleção dos campos conhecidos.</param>
    /// <returns>O nome único para o campo</returns>
    private static string MakeFieldName(FieldCollection fields)
    {
      string fieldName = null;

      var index = fields.Count + 1;
      do
      {
        fieldName = $"Field{index}";
      } while (fields.Any(x => x.Name.EqualsIgnoreCase(fieldName)));

      return fieldName;
    }

    #endregion

    #region Extensões de Field

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

    #endregion
  }
}