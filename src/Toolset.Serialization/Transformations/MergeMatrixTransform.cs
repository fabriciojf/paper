using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class MergeMatrixTransform : ITransform
  {
    private readonly MatrixTransform tableTransform;
    private bool initialized;
    private int collectionDepth;

    public MergeMatrixTransform()
    {
      this.tableTransform = new MatrixTransform();
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
      foreach (var transformedNode in transformedNodes)
      {
        switch (transformedNode.Type)
        {
          case NodeType.CollectionStart:
            {
              collectionDepth++;
              if (collectionDepth == 2)
              {
                yield return transformedNode;
              }
              break;
            }

          case NodeType.CollectionEnd:
            {
              if (collectionDepth == 2)
              {
                yield return transformedNode;
              }
              collectionDepth--;
              break;
            }

          case NodeType.Value:
            {
              yield return transformedNode;
              break;
            }
        }
      }
    }

    public IEnumerable<Node> Complete()
    {
      yield return new Node { Type = NodeType.CollectionEnd };
      yield return new Node { Type = NodeType.DocumentEnd };
    }
  }
}