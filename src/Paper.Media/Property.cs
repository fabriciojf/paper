using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections;
using System.ComponentModel;
using Toolset;
using Paper.Media.Utilities;

namespace Paper.Media
{
  /// <summary>
  /// Propriedade de uma entidade.
  /// </summary>
  [DataContract(Namespace = Namespaces.Default)]
  [KnownType(typeof(NameCollection))]
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
      set => _value = CreatePropertyValue(value);
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

    public static object CreatePropertyValue(object value)
    {
      if (value == null)
        return null;

      var type = value.GetType();

      if (type.IsPrimitive
      || type.Namespace == "System"
      || type.Namespace?.StartsWith("System.") == true
      || StringUtils.IsStringCompatible(value)
      || value is IEnumerable
      || value is CaseVariantString
      || value is PropertyCollection
      || value is NameCollection)
        return value;

      var properties = UnwrapPropertyValues(value);
      return new PropertyCollection(properties);
    }

    public static IEnumerable<Info> UnwrapPropertyInfo(object typeOrInstance)
    {
      if (typeOrInstance == null)
        yield break;

      var type = (typeOrInstance as Type) ?? typeOrInstance.GetType();
      if (type.GetElementType() != null)
      {
        type = type.GetElementType();
      }

      foreach (var property in type.GetProperties())
      {
        var hasArgs = property.GetIndexParameters().Any();
        if (hasArgs)
          continue;

        var displayNameAttr =
          property
            .GetCustomAttributes(true)
            .OfType<DisplayNameAttribute>()
            .FirstOrDefault();
        
        var info = new Info
        {
          Name = property.Name,
          Type = property.PropertyType,
          Title = displayNameAttr?.DisplayName
        };
        yield return info;
      }
    }

    public static IEnumerable<Property> UnwrapPropertyValues(object value)
    {
      if (value == null)
        yield break;

      var type = value.GetType();

      foreach (var property in type.GetProperties())
      {
        var hasArgs = property.GetIndexParameters().Any();
        if (hasArgs)
          continue;

        var propertyValue = property.GetValue(value);
        var compatibleValue = CreatePropertyValue(propertyValue);
        if (compatibleValue != null)
        {
          yield return new Property
          {
            Name = property.Name,
            Value = compatibleValue
          };
        }
      }
    }

    /// <summary>
    /// Coleção de informações adicionais sobre a propriedade.
    /// </summary>
    public class Info
    {
      /// <summary>
      /// Nome do campo.
      /// </summary>
      public string Name { get; internal set; }

      /// <summary>
      /// Tipo de dado do campo.
      /// </summary>
      public Type Type { get; internal set; }

      /// <summary>
      /// Título do campo.
      /// </summary>
      public string Title { get; internal set; }
    }
  }
}