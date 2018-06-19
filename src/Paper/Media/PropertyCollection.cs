using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Toolset.Collections;
using Toolset;

namespace Paper.Media
{
  [CollectionDataContract(Namespace = Namespaces.Default, Name = "Properties")]
  public class PropertyCollection : Collection<Property>
  {
    public PropertyCollection()
    {
    }

    public PropertyCollection(IEnumerable<Property> items)
    : base(items)
    {
    }

    public PropertyCollection(params Property[] items)
    : base(items)
    {
    }

    #region Sobreposicao de Add e Remove para garantir a unicidade de cada argumento

    protected override void OnCommitAdd(ItemStore store, IEnumerable<Property> items, int index = -1)
    {
      var names = items.Select(x => x.Name);
      store.RemoveWhen(item => names.Any(name => name.EqualsIgnoreCase(item.Name)));
      base.OnCommitAdd(store, items, index);
    }

    #endregion

    public static PropertyCollection Create(object graph)
    {
      var collection = new PropertyCollection();
      if (graph != null)
      {
        var properties =
          from property in graph.GetType().GetProperties()
          where !property.GetIndexParameters().Any()
          select property;

        foreach (var property in properties)
        {
          var value = property.GetValue(graph);
          collection.Add(property.Name, value);
        }
      }
      return collection;
    }

    public string[] PropertyNames
      => this.Select(x => x.Name).ToArray();

    public Property this[string propertyName]
    {
      get => this.FirstOrDefault(x => x.Name.EqualsIgnoreCase(propertyName));
      set => this.Add(value);
    }

    public Property Add(string name, object value)
    {
      var property = this.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name));
      if (property == null)
      {
        Add(property = new Property { Name = name });
      }
      property.Value = value;
      return property;
    }

    public bool ContainsKey(string PropertyName)
    {
      return this.Any(x => x.Name.EqualsIgnoreCase(PropertyName));
    }

    public override string ToString()
    {
      return string.Join(",", this);
    }
  }
}