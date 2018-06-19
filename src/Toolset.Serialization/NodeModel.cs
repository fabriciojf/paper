using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Toolset.Serialization;
using System.Reflection;
using System.Globalization;
using System.Xml.Linq;

namespace Toolset.Serialization
{
  public abstract class NodeModel
  {
    public abstract NodeModel Parent { get; internal set; }

    public abstract NodeType SerializationType { get; }

    public abstract object SerializationValue { get; set; }

    public bool IsObject { get { return this is ObjectModel; } }
    public bool IsCollection { get { return this is CollectionModel; } }
    public bool IsProperty { get { return this is PropertyModel; } }
    public bool IsValue { get { return this is ValueModel; } }

    public virtual PropertyModel this[string name]
    {
      get { return this.ChildProperties().FirstOrDefault(x => x.Name.EqualsIgnoreCase(name)); }
    }

    public virtual NodeModel this[int index]
    {
      get { return this.Children().ElementAtOrDefault(index); }
    }

    public int Count
    {
      get { return this.Children().Count(); }
    }

    public string[] PropertyNames
    {
      get { return this.ChildProperties().Select(x => x.Name).ToArray(); }
    }

    #region Children...

    public abstract IEnumerable<NodeModel> Children();

    public virtual IEnumerable<NodeModel> ChildrenAndSelf()
    {
      if (this is NodeModel)
        yield return (NodeModel)this;

      foreach (var child in Children())
        yield return child;
    }

    public virtual IEnumerable<ObjectModel> ChildObjects()
    {
      return Children().OfType<ObjectModel>();
    }

    public virtual IEnumerable<ObjectModel> ChildObjectsAndSelf()
    {
      return ChildrenAndSelf().OfType<ObjectModel>();
    }

    public virtual IEnumerable<CollectionModel> ChildCollections()
    {
      return Children().OfType<CollectionModel>();
    }

    public virtual IEnumerable<CollectionModel> ChildCollectionsAndSelf()
    {
      return ChildrenAndSelf().OfType<CollectionModel>();
    }

    public virtual IEnumerable<PropertyModel> ChildProperties()
    {
      return Children().OfType<PropertyModel>();
    }

    public virtual IEnumerable<PropertyModel> ChildPropertiesAndSelf()
    {
      return ChildrenAndSelf().OfType<PropertyModel>();
    }

    public virtual IEnumerable<ValueModel> ChildValues()
    {
      return Children().OfType<ValueModel>();
    }

    public virtual IEnumerable<ValueModel> ChildValuesAndSelf()
    {
      return ChildrenAndSelf().OfType<ValueModel>();
    }

    #endregion

    #region Ancestors...

    public virtual IEnumerable<NodeModel> Ancestors()
    {
      var node = this as NodeModel;
      while ((node = node.Parent) != null)
      {
        yield return node;
      }
    }

    public virtual IEnumerable<NodeModel> AncestorsAndSelf()
    {
      var node = this as NodeModel;
      while (node != null)
      {
        yield return node;
        node = node.Parent;
      }
    }

    #endregion

    #region Descendants...

    public virtual IEnumerable<NodeModel> Descendants()
    {
      var stack = new Stack<NodeModel>();
      stack.Push(this);

      while (stack.Count > 0)
      {
        var node = stack.Pop();
        foreach (var child in node.Children())
        {
          yield return child;
          stack.Push(child);
        }
      }
    }

    public virtual IEnumerable<NodeModel> DescendantsAndSelf()
    {
      var stack = new Stack<NodeModel>();
      stack.Push(this);

      while (stack.Count > 0)
      {
        var node = stack.Pop();
        if (node is NodeModel)
          yield return (NodeModel)node;

        foreach (var child in node.Children())
        {
          stack.Push(child);
        }
      }
    }

    public virtual IEnumerable<ObjectModel> DescendantObjects()
    {
      return Descendants().OfType<ObjectModel>();
    }

    public virtual IEnumerable<ObjectModel> DescendantObjectsAndSelf()
    {
      return DescendantsAndSelf().OfType<ObjectModel>();
    }

    public virtual IEnumerable<CollectionModel> DescendantCollections()
    {
      return Descendants().OfType<CollectionModel>();
    }

    public virtual IEnumerable<CollectionModel> DescendantCollectionsAndSelf()
    {
      return DescendantsAndSelf().OfType<CollectionModel>();
    }

    public virtual IEnumerable<PropertyModel> DescendantProperties()
    {
      return Descendants().OfType<PropertyModel>();
    }

    public virtual IEnumerable<PropertyModel> DescendantPropertiesAndSelf()
    {
      return DescendantsAndSelf().OfType<PropertyModel>();
    }

    public virtual IEnumerable<ValueModel> DescendantValues()
    {
      return Descendants().OfType<ValueModel>();
    }

    public virtual IEnumerable<ValueModel> DescendantValuesAndSelf()
    {
      return DescendantsAndSelf().OfType<ValueModel>();
    }

    #endregion

  }
}