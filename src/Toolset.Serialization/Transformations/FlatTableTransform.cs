using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public class FlatTableTransform : ITransform
  {
    private readonly TableTransform tableTransform;
    private readonly Func<string, bool> fieldFilter;

    private string[] fieldNames;
    private readonly Dictionary<string, Queue<Node>> fields;
    private Queue<Node> currentField;
    
    public FlatTableTransform()
    {
      this.tableTransform = new TableTransform();
      this.fieldFilter = field => true;

      this.fieldNames = null;
      this.fields = new Dictionary<string, Queue<Node>>();
    }

    public FlatTableTransform(Func<string, bool> fieldFilter)
    {
      this.tableTransform = new TableTransform();
      this.fieldFilter = fieldFilter;

      this.fieldNames = null;
      this.fields = new Dictionary<string, Queue<Node>>();
    }

    public FlatTableTransform(string[] fields)
    {
      this.tableTransform = new TableTransform();
      this.fieldFilter = fields.Contains;

      this.fieldNames = fields;
      this.fields = new Dictionary<string, Queue<Node>>();
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
              fields.Clear();
              yield return emittedNode;
              break;
            }

          case NodeType.ObjectEnd:
            {
              if (fieldNames == null)
              {
                fieldNames = fields.Keys.Where(fieldFilter).ToArray();
              }

              foreach (var fieldName in fieldNames)
              {
                var field = (
                  from item in fields
                  where item.Key.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase)
                  select item.Value
                  ).FirstOrDefault();
                if (field != null)
                {
                  foreach (var queuedNode in field)
                  {
                    yield return queuedNode;
                  }
                }
                else
                {
                  yield return new Node { Type = NodeType.PropertyStart, Value = fieldName };
                  yield return new Node { Type = NodeType.Value, Value = null };
                  yield return new Node { Type = NodeType.PropertyEnd };
                }
              }

              yield return emittedNode;
              break;
            }

          case NodeType.PropertyStart:
            {
              currentField = new Queue<Node>();
              currentField.Enqueue(emittedNode);
              
              var fieldName = (string)emittedNode.Value;
              fields[fieldName] = currentField;
              break;
            }

          case NodeType.PropertyEnd:
          case NodeType.Value:
            {
              currentField.Enqueue(emittedNode);
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
