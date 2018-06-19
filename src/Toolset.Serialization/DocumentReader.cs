using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization
{
  public sealed class DocumentReader : Reader
  {
    private readonly IEnumerator<Node> enumerator;
    private Node currentNode;

    public DocumentReader(DocumentModel document)
      : this((NodeModel)document, new SerializationSettings())
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public DocumentReader(DocumentModel document, SerializationSettings settings)
      : this((NodeModel)document, settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public DocumentReader(NodeModel document, SerializationSettings settings)
      : base(settings)
    {
      this.enumerator = EnumerateNodes(document).GetEnumerator();
    }

    public override Node Current
    {
      get { return currentNode; }
    }

    protected override bool DoRead()
    {
      var ok = enumerator.MoveNext();
      if (ok)
      {
        currentNode = enumerator.Current;
      }
      return ok;
    }

    public override void Close()
    {
      // nada a fazer...
    }

    private IEnumerable<Node> EnumerateNodes(NodeModel node)
    {
      if (node.SerializationType == NodeType.Value)
      {
        yield return new Node { Type = node.SerializationType, Value = node.SerializationValue };
      }
      else
      {
        yield return new Node { Type = node.SerializationType | NodeType.Start, Value = node.SerializationValue };
        foreach (var child in node.Children())
        {
          var enumerator = EnumerateNodes(child);
          foreach (var item in enumerator)
          {
            yield return item;
          }
        }
        yield return new Node { Type = node.SerializationType | NodeType.End };
      }
    }

  }
}
