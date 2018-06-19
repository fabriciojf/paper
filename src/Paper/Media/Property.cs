using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections;
using Paper.Media.Serialization;

namespace Paper.Media
{
  /// <summary>
  /// Propriedade de uma entidade.
  /// </summary>
  [DataContract(Namespace = Namespaces.Default)]
  [KnownType(typeof(Many))]
  [KnownType(typeof(NameCollection))]
  [KnownType(typeof(HeaderCollection))]
  [KnownType(typeof(PropertyCollection))]
  [KnownType(typeof(CaseVariantString))]
  public class Property
  {
    private object _value;

    /// <summary>
    /// Nome da propriedade.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 10)]
    public string Name { get; set; }

    /// <summary>
    /// Valor da propriedade.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 20)]
    public object Value
    {
      get => _value;
      set => _value = CreateValue(value);
    }

    public Property()
    {
    }

    public Property(string name)
    {
      this.Name = name;
    }

    public Property(string name, object value)
    {
      this.Name = name;
      this.Value = value;
    }

    public override string ToString()
    {
      return $"{Name}={Value}";
    }

    public static object CreateValue(object value)
    {
      if (value == null)
        return null;

      var type = value.GetType();

      if (type.IsPrimitive
      || type.Namespace == "System"
      || type.Namespace?.StartsWith("System.") == true
      || SerializationUtilities.IsStringCompatible(value)
      || value is IEnumerable
      || value is CaseVariantString
      || value is PropertyCollection
      || value is HeaderCollection
      || value is NameCollection
      || value is Many)
        return value;

      var collection = new PropertyCollection();
      foreach (var property in type.GetProperties())
      {
        var hasArgs = property.GetIndexParameters().Any();
        if (hasArgs)
          continue;

        var propertyValue = property.GetValue(value);
        var compatibleValue = CreateValue(propertyValue);
        if (compatibleValue != null)
        {
          collection.Add(property.Name, compatibleValue);
        }
      }
      return collection;
    }
  }
}