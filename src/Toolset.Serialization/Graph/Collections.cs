using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Graph
{
  internal static class Collections
  {

    #region Coleções...

    public static bool IsCollection(Type type)
    {
      var ifaces = type.GetInterfaces();
      return ifaces.Any(t => t == typeof(ICollection))
          || ifaces.Any(t =>
                t.IsGenericType
                && (t.GetGenericTypeDefinition() == typeof(ICollection<>))
             );
    }

    public static bool IsCollection(object graph)
    {
      return (graph != null) ? IsCollection(graph.GetType()) : false;
    }

    public static bool IsGenericCollection(Type type)
    {
      var ifaces = type.GetInterfaces();
      return ifaces.Any(t => t.GetGenericTypeDefinition() == typeof(ICollection<>));
    }

    public static bool IsGenericCollection(object graph)
    {
      return (graph != null) ? IsGenericCollection(graph.GetType()) : false;
    }

    #endregion

    #region Dicionários...

    public static bool IsDictionary(Type type)
    {
      var ifaces = type.GetInterfaces();
      return ifaces.Any(t => t == typeof(IDictionary))
          || ifaces.Any(t =>
                t.IsGenericType
                && (t.GetGenericTypeDefinition() == typeof(IDictionary<,>))
             );
    }

    public static bool IsDictionary(object graph)
    {
      return (graph != null) ? IsDictionary(graph.GetType()) : false;
    }

    public static bool IsGenericDictionary(Type type)
    {
      var ifaces = type.GetInterfaces();
      return ifaces.Any(t =>
                t.IsGenericType
                && (t.GetGenericTypeDefinition() == typeof(IDictionary<,>))
             );
    }

    public static bool IsGenericDictionary(object graph)
    {
      return (graph != null) ? IsGenericDictionary(graph.GetType()) : false;
    }

    #endregion

    #region Argumentos do tipo genérico...

    public static Type[] GetGenericArguments(Type type)
    {
      var genericIface = IsGenericDictionary(type) ? typeof(IDictionary<,>) : typeof(ICollection<>);
      var iface = type.GetInterfaces().FirstOrDefault(t => t.GetGenericTypeDefinition() == genericIface);
      return (iface != null) ? iface.GetGenericArguments() : new Type[0];
    }

    public static Type[] GetGenericArguments(object graph)
    {
      return (graph != null) ? GetGenericArguments(graph.GetType()) : new Type[0];
    }

    #endregion

  }
}
