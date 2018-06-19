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

namespace Toolset.Serialization
{
  public class Node : ICloneable
  {
    private NodeType RawTypeMask =
      NodeType.Document | NodeType.Collection | NodeType.Object | NodeType.Property | NodeType.Value;

    public NodeType RawType { get { return Type & RawTypeMask; } }
    public NodeType Type { get; set; }
    public object Value { get; set; }

    public Node Clone()
    {
      return new Node
      {
        Type = Type,
        Value = Value
      };
    }

    object ICloneable.Clone()
    {
      return this.Clone();
    }

    public override string ToString()
    {
      return (Value != null) ? (Type + " '" + Value + "'") : Type.ToString();
    }
  }
}