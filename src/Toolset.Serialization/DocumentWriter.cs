using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization
{
  public sealed class DocumentWriter : Writer
  {
    private Stack<NodeModel> stack;
    private Stack<Stack<NodeModel>> cache;

    public DocumentWriter()
      : base(new SerializationSettings())
    {
      this.stack = new Stack<NodeModel>();
      this.cache = new Stack<Stack<NodeModel>>();
    }

    public DocumentWriter(SerializationSettings settings)
      : base(settings ?? new SerializationSettings())
    {
      this.stack = new Stack<NodeModel>();
      this.cache = new Stack<Stack<NodeModel>>();
    }

    public DocumentModel TargetDocument
    {
      get;
      set;
    }

    protected override void DoWrite(Node node)
    {
      switch (node.Type)
      {
        case NodeType.DocumentStart:
          {
            var obj = new DocumentModel { SerializationValue = node.Value };
            stack.Push(obj);
            cache.Push(stack);
            stack = new Stack<NodeModel>();
            break;
          }

        case NodeType.DocumentEnd:
          {
            var root = stack.SingleOrDefault<NodeModel>();
            stack = cache.Pop();

            var document = (DocumentModel)stack.Pop();
            document.Root = root;

            TargetDocument = document;
            break;
          }

        case NodeType.ObjectStart:
          {
            var obj = new ObjectModel { SerializationValue = node.Value };
            stack.Push(obj);
            cache.Push(stack);
            stack = new Stack<NodeModel>();
            break;
          }

        case NodeType.ObjectEnd:
          {
            var properties = stack.Cast<PropertyModel>();
            stack = cache.Pop();

            var obj = (ObjectModel)stack.Peek();
            obj.AddPropertyRange(properties.Reverse());

            if (!cache.Any() && (stack.Count <= 1))
            {
              var document = new DocumentModel();
              document.Root = obj;

              TargetDocument = document;
            }

            break;
          }

        case NodeType.CollectionStart:
          {
            var collection = new CollectionModel { SerializationValue = node.Value };
            stack.Push(collection);
            cache.Push(stack);
            stack = new Stack<NodeModel>();
            break;
          }

        case NodeType.CollectionEnd:
          {
            var items = stack;
            stack = cache.Pop();

            var obj = (CollectionModel)stack.Peek();
            obj.AddRange(items.Reverse());

            break;
          }

        case NodeType.PropertyStart:
          {
            var property = new PropertyModel { SerializationValue = node.Value };
            stack.Push(property);
            cache.Push(stack);
            stack = new Stack<NodeModel>();
            break;
          }

        case NodeType.PropertyEnd:
          {
            NodeModel value = null;
            if (stack.Count == 0)
              value = new ValueModel { Value = null };
            else if (stack.Count == 1)
              value = stack.Pop();
            else
              throw new Exception("Property cannot have more than one value.");

            stack = cache.Pop();
            var property = (PropertyModel)stack.Peek();
            property.Value = value;

            break;
          }

        case NodeType.Value:
          {
            var value = new ValueModel { Value = node.Value };
            stack.Push(value);
            break;
          }
      }
    }

    protected override void DoWriteComplete()
    {
      // nada a fazer...
    }

    protected override void DoFlush()
    {
      // nada a fazer...
    }

    protected override void DoClose()
    {
      // nada a fazer...
    }
  }
}
