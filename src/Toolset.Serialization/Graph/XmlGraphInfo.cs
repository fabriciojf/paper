using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Toolset.Serialization.Graph
{
  internal sealed class XmlGraphInfo : IGraphInfo
  {
    private Type type;
    private PropertyInfo[] properties;

    public XmlGraphInfo(Type type)
    {
      this.type = type;
    }

    public string GetLabel()
    {
      var attribute = type.GetCustomAttributes(typeof(XmlRootAttribute), true).FirstOrDefault();
      var contract = (XmlRootAttribute)attribute;
      return
        ((contract != null) && !string.IsNullOrEmpty(contract.ElementName))
          ? contract.ElementName
          : type.Name;
    }

    public string GetPropertyLabel(PropertyInfo property)
    {
      var attribute = property.GetCustomAttributes(typeof(XmlElementAttribute), true).FirstOrDefault();
      var contract = (XmlElementAttribute)attribute;
      return 
        ((contract != null) && !string.IsNullOrEmpty(contract.ElementName))
          ? contract.ElementName
          : property.Name;
    }

    public IEnumerable<PropertyInfo> GetProperties()
    {
      if (properties == null)
      {
        properties = (
          from p in type.GetProperties()
          where !p.GetCustomAttributes(typeof(XmlIgnoreAttribute), true).Any()
          let contract =
            (XmlElementAttribute)
              p.GetCustomAttributes(typeof(XmlElementAttribute), true).FirstOrDefault()
          orderby (contract != null) ? contract.Order : 0
          select p
          ).ToArray();
      }
      return properties;
    }
  }
}
