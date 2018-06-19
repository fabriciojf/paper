using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Toolset.Serialization.Graph
{
  internal sealed class GraphInfo : IGraphInfo
  {
    private IGraphInfo instancia;

    public GraphInfo(Type type)
    {
      var isDataContract = type.GetCustomAttributes(typeof(DataContractAttribute), true).Any();
      var isXml =
        type.GetCustomAttributes(typeof(XmlTypeAttribute), true).Any()
        || type.GetCustomAttributes(typeof(XmlRootAttribute), true).Any();
      
      if (isDataContract)
        instancia = new DataContractGraphInfo(type);
      else if (isXml)
        instancia = new XmlGraphInfo(type);
      else
        instancia = new ObjectGraphInfo(type);
    }

    public string GetLabel()
    {
      return instancia.GetLabel();
    }

    public string GetPropertyLabel(PropertyInfo property)
    {
      return instancia.GetPropertyLabel(property);
    }

    public IEnumerable<PropertyInfo> GetProperties()
    {
      return instancia.GetProperties();
    }
  }
}
