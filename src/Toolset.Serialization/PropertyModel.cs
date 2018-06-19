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
  public class PropertyModel : NodeModel
  {
    private readonly NodeType nodeType = NodeType.Property;
    private object nodeValue;

    #region Construtores...

    public PropertyModel()
    {
    }

    public PropertyModel(string name)
    {
      this.Name = name;
    }

    public PropertyModel(string name, NodeModel value)
    {
      this.Name = name;
      this.Value = value;
    }

    public PropertyModel(string name, object value)
    {
      this.Name = name;
      this.Value = new ValueModel { Value = value };
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
      return (Value != null) ? new[] { Value } : Enumerable.Empty<NodeModel>();
    }

    public string Name
    {
      get { return (nodeValue != null) ? nodeValue.ToString() : null; }
      set { nodeValue = value; }
    }

    public NodeModel Value
    {
      get { return this.value; }
      set
      {
        this.value = value;
        this.value.Parent = this;
      }
    }
    private NodeModel value;

  }
}