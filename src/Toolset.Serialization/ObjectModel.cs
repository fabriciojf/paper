using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Toolset.Serialization;

namespace Toolset.Serialization
{
  public class ObjectModel : NodeModel
  {
    private readonly List<PropertyModel> properties = new List<PropertyModel>();
    private readonly NodeType nodeType = NodeType.Object;
    private object nodeValue;

    #region Construtores...

    public ObjectModel()
    {
    }

    public ObjectModel(string name)
    {
      this.Name = name;
    }

    public ObjectModel(IEnumerable<PropertyModel> properties)
    {
      this.AddPropertyRange(properties);
    }

    public ObjectModel(PropertyModel property, params PropertyModel[] properties)
    {
      this.AddProperty(property);
      this.AddPropertyRange(properties);
    }

    public ObjectModel(string name, IEnumerable<PropertyModel> properties)
    {
      this.Name = name;
      this.AddPropertyRange(properties);
    }

    public ObjectModel(string name, PropertyModel property, params PropertyModel[] properties)
    {
      this.Name = name;
      this.AddProperty(property);
      this.AddPropertyRange(properties);
    }

    #endregion

    public override NodeModel Parent
    {
      get;
      internal set;
    }

    public override NodeType SerializationType
    {
      get { return nodeType; }
    }

    public override object SerializationValue
    {
      get { return nodeValue; }
      set { nodeValue = value; }
    }

    public override IEnumerable<NodeModel> Children()
    {
      return properties;
    }

    public string Name
    {
      get { return nodeValue?.ToString(); }
      set { nodeValue = value; }
    }

    public int PropertyCount
    {
      get { return properties.Count; }
    }

    public new IEnumerable<string> PropertyNames
    {
      get { return properties.Select(p => p.Name).Distinct(); }
    }

    public new PropertyModel this[string name]
    {
      get { return properties.FirstOrDefault(p => p.Name == name); }
    }

    public new PropertyModel this[int index]
    {
      get { return properties.ElementAtOrDefault(index); }
    }

    public void AddProperty(PropertyModel property)
    {
      Adopt(property);
      properties.Add(property);
    }

    public void AddProperty(string name, object value)
    {
      var property = new PropertyModel(name, value);
      Adopt(property);
      properties.Add(property);
    }

    public void AddPropertyRange(IEnumerable<PropertyModel> range)
    {
      Adopt(range);
      properties.AddRange(range);
    }

    public void InsertProperty(int index, PropertyModel property)
    {
      Adopt(property);
      properties.Insert(index, property);
    }

    public void InsertProperty(int index, string name, object value)
    {
      var property = new PropertyModel(name, value);
      Adopt(property);
      properties.Insert(index, property);
    }

    public void InsertPropertyRange(int index, IEnumerable<PropertyModel> range)
    {
      Adopt(range);
      properties.InsertRange(index, range);
    }

    public void RemoveProperty(PropertyModel property)
    {
      Reject(property);
      properties.Remove(property);
    }

    public void RemoveProperty(string name)
    {
      var range = properties.Where(p => p.Name == name);
      Reject(range);
      properties.RemoveAll(p => p.Name == name);
    }

    public void RemovePropertyAt(int index)
    {
      var property = properties[index];
      Reject(property);
      properties.RemoveAt(index);
    }

    public void RemovePropertyRange(IEnumerable<PropertyModel> range)
    {
      Reject(range);
      properties.RemoveAll(p => range.Contains(p));
    }

    public void RemoveAllProperties(Func<PropertyModel, bool> match)
    {
      var range = properties.Where(match);
      Reject(range);
      properties.RemoveAll(p => match.Invoke(p));
    }

    public void RemoveAllProperties()
    {
      Reject(properties);
      properties.Clear();
    }

    private void Adopt(IEnumerable<PropertyModel> nodes)
    {
      foreach (var node in nodes)
      {
        node.Parent = this;
      }
    }

    private void Adopt(PropertyModel node)
    {
      node.Parent = this;
    }

    private void Reject(IEnumerable<PropertyModel> nodes)
    {
      foreach (var node in nodes)
      {
        if (node.Parent == this)
        {
          node.Parent = null;
        }
      }
    }

    private void Reject(PropertyModel node)
    {
      if (node.Parent == this)
      {
        node.Parent = null;
      }
    }

  }
}