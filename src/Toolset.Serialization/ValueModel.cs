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
  public class ValueModel : NodeModel
  {
    private readonly NodeType nodeType = NodeType.Value;
    private object nodeValue;

    #region Construtores...

    public ValueModel()
    {
    }

    public ValueModel(object value)
    {
      this.Value = value;
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
      return Enumerable.Empty<NodeModel>();
    }

    public object Value
    {
      get { return nodeValue; }
      set { nodeValue = value; }
    }

    public Type ValueType
    {
      get { return (Value != null) ? Value.GetType() : typeof(object); }
    }

  }
}