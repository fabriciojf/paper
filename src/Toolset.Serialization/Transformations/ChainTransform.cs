using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public class ChainTransform : ITransform
  {
    private LinkedList<ITransform> chain;

    public ChainTransform(IEnumerable<ITransform> transforms)
    {
      chain = new LinkedList<ITransform>(transforms);
    }

    public ChainTransform(ITransform transform, params ITransform[] others)
    {
      chain = new LinkedList<ITransform>(new[] { transform }.Union(others).ToArray());
    }

    public SerializationSettings Settings
    {
      get;
      set;
    }

    public IEnumerable<Node> TransformNode(Node node)
    {
      var first = chain.First;
      return ProcessTransformationChain(node, first.Value, first.Next);
    }

    public IEnumerable<Node> Complete()
    {
      var item = chain.First;
      while (item != null)
      {
        var transform = item.Value;

        var nodes = transform.Complete();
        foreach (var node in nodes)
        {
          var transformedNodes = ProcessTransformationChain(node, transform, item.Next);
          foreach (var transformedNode in transformedNodes)
          {
            yield return transformedNode;
          }
        }

        item = item.Next;
      }
    }

    private IEnumerable<Node> ProcessTransformationChain(Node node, ITransform transform, LinkedListNode<ITransform> next)
    {
      var transformedNodes = transform.TransformNode(node);
      foreach (var transformedNode in transformedNodes)
      {
        if (next != null)
        {
          var others = ProcessTransformationChain(transformedNode, next.Value, next.Next);
          foreach (var other in others)
            yield return other;
        }
        else
        {
          yield return transformedNode;
        }
      }
    }

  }
}
