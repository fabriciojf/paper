using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Toolset.Serialization.Graph
{
  internal sealed class ObjectGraphInfo : IGraphInfo
  {
    private Type type;
    private PropertyInfo[] properties;

    public ObjectGraphInfo(Type type)
    {
      this.type = type;
    }

    public string GetLabel()
    {
      var contract = 
        type.GetCustomAttributes(typeof(GraphAttribute), true)
          .Cast<GraphAttribute>()
          .FirstOrDefault();
      return (contract != null && contract.Name != null) ? contract.Name : type.Name;
    }

    public string GetPropertyLabel(PropertyInfo property)
    {
      return property.Name;
    }

    public IEnumerable<PropertyInfo> GetProperties()
    {
      if (properties == null)
      {
        properties = (
          from p in type.GetProperties()
          where !p.GetCustomAttributes(typeof(GraphIgnoreAttribute), true).Any()
          select p
          ).ToArray();
      }
      return properties;
    }
  }
}
