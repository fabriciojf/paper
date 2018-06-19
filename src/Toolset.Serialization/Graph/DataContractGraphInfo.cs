using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Runtime.Serialization;

namespace Toolset.Serialization.Graph
{
  internal sealed class DataContractGraphInfo : IGraphInfo
  {
    private Type type;
    private PropertyInfo[] properties;

    public DataContractGraphInfo(Type type)
    {
      this.type = type;
    }

    public string GetLabel()
    {
      string name = null;
      if (type.GetCustomAttributes(true).OfType<DataContractAttribute>().Any())
      {
        name = 
          type
            .GetCustomAttributes(true)
            .OfType<DataContractAttribute>()
            .Select(x => x.Name)
            .SingleOrDefault();
      }
      else if (type.GetCustomAttributes(true).OfType<CollectionDataContractAttribute>().Any())
      {
        name =
          type
            .GetCustomAttributes(true)
            .OfType<CollectionDataContractAttribute>()
            .Select(x => x.Name)
            .SingleOrDefault();
      }
      return name ?? type.Name;
    }

    public string GetPropertyLabel(PropertyInfo property)
    {
      var attribute = property.GetCustomAttributes(typeof(DataMemberAttribute), true).FirstOrDefault();
      var contract = (DataMemberAttribute)attribute;
      return contract.Name ?? property.Name;
    }

    public bool ShouldIgnoreEmptyValue(PropertyInfo property)
    {
      var attribute = property.GetCustomAttributes(typeof(DataMemberAttribute), true).FirstOrDefault();
      var contract = (DataMemberAttribute)attribute;
      return !contract.EmitDefaultValue;
    }

    public IEnumerable<PropertyInfo> GetProperties()
    {
      if (properties == null)
      {
        properties = (
          from p in type.GetProperties()
          let contract =
            (DataMemberAttribute)
              p.GetCustomAttributes(typeof(DataMemberAttribute), true).FirstOrDefault()
          where contract != null
          orderby contract.Order
          select p
          ).ToArray();
      }
      return properties;
    }
  }
}
