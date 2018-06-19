using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class MergeTableTransform : ITransform
  {
    private readonly TableTransform tableTransform;
    private bool initialized;

    public MergeTableTransform()
    {
      this.tableTransform = new TableTransform();
    }

    public SerializationSettings Settings
    {
      get { return tableTransform.Settings; }
      set { tableTransform.Settings = value; }
    }

    public IEnumerable<Node> TransformNode(Node node)
    {
      if (!initialized)
      {
        initialized = true;
        yield return new Node { Type = NodeType.DocumentStart };
        yield return new Node { Type = NodeType.CollectionStart };
      }

      var transformedNodes = tableTransform.TransformNode(node);
      var rows =
        from n in transformedNodes
        where n.RawType == NodeType.Object
           || n.RawType == NodeType.Property
           || n.RawType == NodeType.Value
        select n;
      foreach (var row in rows)
      {
        yield return row;
      }
    }

    public IEnumerable<Node> Complete()
    {
      yield return new Node { Type = NodeType.CollectionEnd };
      yield return new Node { Type = NodeType.DocumentEnd };
    }
  }
}