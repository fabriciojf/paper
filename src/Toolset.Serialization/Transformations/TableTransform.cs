using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class TableTransform : ITransform
  {
    public const TextCase DefaultCase = TextCase.KeepOriginal;

    private readonly Stack<string> fieldStack;

    private int collectionDepth;
    private bool isInsideTable;
    private bool isInsideRow;
    private int rowDepth;
    private int rowCount;
    private int unnamedFieldCount;

    public TableTransform()
    {
      this.fieldStack = new Stack<string>();
    }

    public SerializationSettings Settings
    {
      get;
      set;
    }

    private Node OpenRow()
    {
      rowCount++;
      unnamedFieldCount = 0;
      var name = "Row" + rowCount;
      var rowName = ValueConventions.CreateName(name, Settings, DefaultCase);
      return new Node { Type = NodeType.ObjectStart, Value = rowName };
    }

    private Node CloseRow()
    {
      unnamedFieldCount = 0;
      return new Node { Type = NodeType.ObjectEnd };
    }

    public IEnumerable<Node> TransformNode(Node node)
    {
      switch (node.Type)
      {
        case NodeType.DocumentStart:
          {
            var name = ValueConventions.CreateName(node.Value, Settings, DefaultCase);
            yield return new Node { Type = NodeType.DocumentStart, Value = name };
            break;
          }

        case NodeType.DocumentEnd:
          {
            yield return new Node { Type = NodeType.DocumentEnd };
            break;
          }

        case NodeType.CollectionStart:
          {
            collectionDepth++;

            if (collectionDepth == 1)
            {
              var name = ValueConventions.CreateName(node.Value, Settings, DefaultCase);
              yield return new Node { Type = NodeType.CollectionStart, Value = name };
              isInsideTable = true;
            }
            else if (collectionDepth > 1)
            {
              rowDepth++;
              if (rowDepth == 1)
              {
                isInsideTable = true;
                yield return OpenRow();
              }
              isInsideRow = rowDepth > 0;
            }

            isInsideTable = (collectionDepth == 1) || (rowDepth == 1);
            break;
          }

        case NodeType.CollectionEnd:
          {
            if (collectionDepth > 1)
            {
              if (rowDepth == 1)
              {
                yield return CloseRow();
              }
              rowDepth--;
              isInsideRow = rowDepth > 0;
            }
            else if (collectionDepth == 1)
            {
              yield return new Node { Type = NodeType.CollectionEnd };
            }

            collectionDepth--;
            isInsideTable = (collectionDepth == 1) || (rowDepth == 1);
            break;
          }

        case NodeType.ObjectStart:
          {
            if (isInsideTable)
            {
              rowDepth++;
              if (rowDepth == 1)
              {
                yield return OpenRow();
              }
              isInsideRow = rowDepth > 0;
            }
            break;
          }

        case NodeType.ObjectEnd:
          {
            if (isInsideTable)
            {
              if (rowDepth == 1)
              {
                yield return CloseRow();
              }
              rowDepth--;
              isInsideRow = rowDepth > 0;
            }
            break;
          }

        case NodeType.PropertyStart:
          {
            if (isInsideTable)
            {
              var fieldName = ValueConventions.CreateName(node.Value, Settings, DefaultCase);
              fieldStack.Push(fieldName);
            }
            break;
          }

        case NodeType.PropertyEnd:
          {
            if (isInsideTable)
            {
              fieldStack.Pop();
            }
            break;
          }

        case NodeType.Value:
          {
            if (isInsideTable)
            {
              if (!isInsideRow)
              {
                rowCount++;
                var name = "Row" + rowCount;
                var rowName = ValueConventions.CreateName(name, Settings, DefaultCase);
                yield return new Node { Type = NodeType.ObjectStart, Value = rowName };
              }

              var fieldName = CreateFieldName();
              yield return new Node { Type = NodeType.PropertyStart, Value = fieldName };
              yield return node;
              yield return new Node { Type = NodeType.PropertyEnd };

              if (!isInsideRow)
              {
                yield return new Node { Type = NodeType.ObjectEnd };
              }
            }
            break;
          }
      }
    }

    private string CreateFieldName()
    {
      if (fieldStack.Count == 0)
      {
        unnamedFieldCount++;

        var field = "Field" + unnamedFieldCount;
        var name = ValueConventions.CreateName(field, Settings, DefaultCase);
        return name;
      }
      else
      {
        var name = fieldStack.Peek();
        foreach (var field in fieldStack.Skip(1))
        {
          name = field + "." + name;
        }
        return name;
      }
    }

    public IEnumerable<Node> Complete()
    {
      return Enumerable.Empty<Node>();
    }

  }
}