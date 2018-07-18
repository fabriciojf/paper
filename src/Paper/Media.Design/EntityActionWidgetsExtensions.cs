using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Paper.Media.Design.Widgets;
using Toolset;
using Toolset.Collections;
using Toolset.Reflection;

namespace Paper.Media.Design
{
  /// <summary>
  /// Extensões de desenho de ações de objetos Entity.
  /// </summary>
  public static class EntityActionWidgetsExtensions
  {
    #region Extensões de EntityAction

    /// <summary>
    /// Adiciona um Widget como um campo da ação de entidade indicada.
    /// </summary>
    /// <typeparam name="T">O tipo do Widget.</typeparam>
    /// <param name="action">A ação a ser modificada.</param>
    /// <param name="name">O nome do campo.</param>
    /// <param name="widget">O Widget a ser adicionado à ação.</param>
    /// <returns>A própria instância da ação modificada</returns>
    public static EntityAction AddWidget(this EntityAction action, string name, Widget widget)
    {
      var field = new Field();
      field._CopyFrom(widget);

      if (action.Fields == null)
      {
        action.Fields = new FieldCollection();
      }
      action.Fields[name] = field;

      return action;
    }

    /// <summary>
    /// Adiciona um Widget como um campo da ação de entidade indicada.
    /// </summary>
    /// <typeparam name="T">O tipo do Widget.</typeparam>
    /// <param name="action">A ação a ser modificada.</param>
    /// <param name="name">O nome do campo.</param>
    /// <param name="builder">O construtor do Widget.</param>
    /// <returns>A própria instância da ação modificada</returns>
    public static EntityAction AddWidget<T>(this EntityAction action, string name, Action<T> builder = null)
      where T : Widget
    {
      var widget = (T)Activator.CreateInstance(typeof(T), name);
      builder?.Invoke(widget);

      var field = new Field();
      field._CopyFrom(widget);

      if (action.Fields == null)
      {
        action.Fields = new FieldCollection();
      }
      action.Fields[name] = field;

      return action;
    }

    #endregion
  }
}