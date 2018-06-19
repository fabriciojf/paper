using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using Toolset;
using System.Reflection;
using System.Xml.Linq;
using Toolset.Xml;

namespace Toolset.Sequel
{
  internal static class Graph
  {

    public static T CreateGraph<T>(DbDataReader reader)
    {
      var instance = CreateGraph(reader, typeof(T));
      return (T)instance;
    }

    public static object CreateGraph(DbDataReader reader, Type type)
    {
      var instance = Activator.CreateInstance(type);

      for (var i = 0; i < reader.FieldCount; i++)
      {
        var value = reader.GetValue(i);
        if (value.IsNull())
          continue;

        var name = reader.GetName(i);
        var flags = BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public;
        var property = type.GetProperty(name, flags);
        if (property != null)
        {
          object convertedValue = value.ConvertTo(property.PropertyType);
          property.SetValue(instance, convertedValue, null);
        }
      }

      return instance;
    }

  }
}
