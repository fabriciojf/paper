using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public class FlatMatrixTransform : ITransform
  {
    private readonly FlatTableTransform tableTransform;

    private readonly Queue<Node> headers;
    private readonly Queue<Node> queue;

    private bool initialized;
    private bool collectingHeaders;

    public FlatMatrixTransform()
    {
      this.tableTransform = new FlatTableTransform();

      this.headers = new Queue<Node>();
      this.queue = new Queue<Node>();
    }

    public FlatMatrixTransform(Func<string, bool> fieldFilter)
    {
      this.tableTransform = new FlatTableTransform(fieldFilter);

      this.headers = new Queue<Node>();
      this.queue = new Queue<Node>();
    }

    public FlatMatrixTransform(string[] fields)
    {
      this.tableTransform = new FlatTableTransform(fields);

      this.headers = new Queue<Node>();
      this.queue = new Queue<Node>();
    }

    public SerializationSettings Settings
    {
      get { return this.tableTransform.Settings; }
      set { this.tableTransform.Settings = value; }
    }

    public IEnumerable<Node> TransformNode(Node node)
    {
      if (!initialized)
      {
        collectingHeaders = Settings.Get<bool>("HasHeaders");
        initialized = true;
      }

      var emittedNodes = tableTransform.TransformNode(node);
      foreach (var emittedNode in emittedNodes)
      {
        switch (emittedNode.Type)
        {
          case NodeType.ObjectStart:
            {
              queue.Clear();
              queue.Enqueue(new Node { Type = NodeType.CollectionStart, Value = emittedNode.Value });
              break;
            }

          case NodeType.ObjectEnd:
            {
              queue.Enqueue(new Node { Type = NodeType.CollectionEnd, Value = emittedNode.Value });

              if (collectingHeaders)
              {
                var name = ValueConventions.CreateName("Header", Settings, TableTransform.DefaultCase);
                yield return new Node { Type = NodeType.CollectionStart, Value = name };
                foreach (var header in headers)
                {
                  yield return new Node { Type = NodeType.Value, Value = header.Value };
                }
                yield return new Node { Type = NodeType.CollectionEnd };
                collectingHeaders = false;
              }

              foreach (var queueNode in queue)
              {
                yield return queueNode;
              }

              break;
            }

          case NodeType.PropertyStart:
            {
              if (collectingHeaders)
                headers.Enqueue(emittedNode);

              // omitindo...
              break;
            }

          case NodeType.PropertyEnd:
            {
              // omitindo...
              break;
            }

          case NodeType.Value:
            {
              queue.Enqueue(emittedNode);
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
