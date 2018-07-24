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

    #endregion
  }
}