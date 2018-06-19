using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public class MatrixTransform : ITransform
  {
    private readonly TableTransform tableTransform;
    
    public MatrixTransform()
    {
      this.tableTransform = new TableTransform();
    }

    public SerializationSettings Settings
    {
      get { return this.tableTransform.Settings; }
      set { this.tableTransform.Settings = value; }
    }

    public IEnumerable<Node> TransformNode(Node node)
    {
      var emittedNodes = tableTransform.TransformNode(node);
      foreach (var emittedNode in emittedNodes)
      {
        switch (emittedNode.Type)
        {
          case NodeType.ObjectStart:
            {
              yield return new Node { Type = NodeType.CollectionStart, Value = emittedNode.Value };
              break;
            }

          case NodeType.ObjectEnd:
            {
              yield return new Node { Type = NodeType.CollectionEnd, Value = emittedNode.Value };
              break;
            }

          case NodeType.PropertyStart:
          case NodeType.PropertyEnd:
            {
              // omitindo...
              break;
            }

          default:
            {
              yield return emittedNode;
              break;
            }
        }
      }
    }

    public IEnumerable<Node> Complete()
    {
      return Enumerable.Empty<Node>();
    }

  }
}
