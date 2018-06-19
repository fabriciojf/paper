using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Serialization.Transformations;

namespace Toolset.Serialization
{
  internal sealed class Validator
  {
    private const NodeType RawTypeMask =
      (NodeType.Document | NodeType.Object | NodeType.Collection | NodeType.Property | NodeType.Value);

    private struct Rule
    {
      public NodeType[] ExpectedTypes;
      public int MinChildren;
      public int MaxChildren;
      public bool ValueRequired;
    }

    private class Item
    {
      public Node Node { get; set; }
      public NodeType Type { get { return Node.Type; } }
      public NodeType RawType { get { return Node.Type & RawTypeMask; } }
      public int Children { get; set; }
    }

    private readonly Stack<Item> stack;

    #region Regras...

    private static readonly Dictionary<NodeType, Rule> Rules = new Dictionary<NodeType, Rule>
    {
      {
        NodeType.Unknown,
        new Rule
        {
          ExpectedTypes = new [] { NodeType.DocumentStart, NodeType.ObjectStart, NodeType.CollectionStart }
        , MinChildren = 1
        , MaxChildren = 1
        }
      },
      {
        NodeType.Document,
        new Rule
        {
          ExpectedTypes = new [] { NodeType.DocumentEnd, NodeType.ObjectStart, NodeType.CollectionStart }
        , MinChildren = 0
        , MaxChildren = 1
        }
      },
      {
        NodeType.Object,
        new Rule
        {
          ExpectedTypes = new [] { NodeType.ObjectEnd, NodeType.PropertyStart }
        , MinChildren = 0
        , MaxChildren = int.MaxValue
        }
      },
      {
        NodeType.Collection,
        new Rule
        {
          ExpectedTypes = new [] { NodeType.CollectionEnd, NodeType.CollectionStart, NodeType.ObjectStart, NodeType.Value }
        , MinChildren = 0
        , MaxChildren = int.MaxValue
        }
      },
      {
        NodeType.Property,
        new Rule
        {
          ExpectedTypes = new [] { NodeType.PropertyEnd, NodeType.ObjectStart, NodeType.CollectionStart, NodeType.Value }
        , ValueRequired = true
        , MinChildren = 1
        , MaxChildren = 1
        }
      },
      {
        NodeType.Value,
        new Rule
        {
          ExpectedTypes = new NodeType[0]
        , MinChildren = 0
        , MaxChildren = 0
        }
      }
    };

    #endregion

    public Validator()
    {
      this.stack = new Stack<Item>();
      
      // Cadastrando o nodo inicial representando a raiz do documento
      this.stack.Push(new Item { Node = new Node() });
    }

    public void AcceptNode(Node node)
    {
      var currentNode = node;
      var currentType = (currentNode != null) ? currentNode.Type : NodeType.Unknown;
      var currentRawType = currentType & RawTypeMask;

      var parentItem = stack.FirstOrDefault() ?? new Item();
      var parentNode = parentItem.Node;
      var parentType = parentItem.Type;
      var parentRawType = parentItem.RawType;

      //
      // Aplicando validacoes...
      //
      var parentRule = Rules[parentRawType];

      if (!Rules.ContainsKey(currentRawType)
      ||  !parentRule.ExpectedTypes.Contains(currentType))
        throw new NotExpectedException(parentNode, currentNode, parentRule.ExpectedTypes);

      var currentRule = Rules[currentRawType];

      var shouldHaveValue = !currentType.HasFlag(NodeType.End) && currentRule.ValueRequired && node.Value == null;
      if (shouldHaveValue)
        throw new RequiredValueException(parentNode, currentNode);

      if (currentType.HasFlag(NodeType.End))
      {
        if (parentItem.Children < parentRule.MinChildren)
          throw new ChildCountException(parentNode, currentNode, parentRule.MinChildren, parentRule.MaxChildren, parentItem.Children);
        stack.Pop();
      }
      else
      {
        parentItem.Children++;
        if (parentItem.Children > parentRule.MaxChildren)
          throw new ChildCountException(parentNode, currentNode, parentRule.MinChildren, parentRule.MaxChildren, parentItem.Children);

        if (currentType.HasFlag(NodeType.Start))
          stack.Push(new Item { Node = currentNode });
      }
    }

    public void Validate()
    {
      var parentItem = stack.FirstOrDefault() ?? new Item();
      var parentNode = parentItem.Node;
      var parentType = parentItem.Type;
      var parentRawType = parentItem.RawType;

      var rule = Rules[parentRawType];

      if (stack.Count != 1)
        throw new EndOfStreamException(parentNode, rule.ExpectedTypes);

      if (parentItem.Children < rule.MinChildren || parentItem.Children > rule.MaxChildren)
        throw new ChildCountException(parentNode, null, rule.MinChildren, rule.MaxChildren, parentItem.Children);
    }
  }
}
