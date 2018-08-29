using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Toolset.Serialization;

namespace Toolset.Serialization
{
  public class CollectionModel : NodeModel
  {
    private readonly List<NodeModel> items = new List<NodeModel>();
    private readonly NodeType nodeType = NodeType.Collection;
    private object nodeValue;

    #region Construtores...

    public CollectionModel()
    {
    }

    public CollectionModel(IEnumerable<NodeModel> nodes)
    {
      items.AddRange(nodes);
    }

    public CollectionModel(NodeModel item, params NodeModel[] nodes)
    {
      items.Add(item);
      items.AddRange(nodes);
    }

    public CollectionModel(IEnumerable<object> nodes)
    {
      var range =
        from x in nodes
        select
          (x is NodeModel)
            ? (NodeModel)x : new ValueModel { Value = x };
      items.AddRange(range);
    }

    public CollectionModel(object item, params object[] nodes)
    {
      var node = (item is NodeModel) ? (NodeModel)item : new ValueModel { Value = item };
      var range =
        from x in nodes
        select
          (x is NodeModel)
            ? (NodeModel)x : new ValueModel { Value = x };
      items.Add(node);
      items.AddRange(range);
    }

    #endregion

    public override NodeModel Parent
    {
      get;
      internal set;
    }

    public override NodeType SerializationType
    {
      get { return nodeType; }
    }

    public override object SerializationValue
    {
      get { return nodeValue; }
      set { nodeValue = value; }
    }

    public override IEnumerable<NodeModel> Children()
    {
      return items;
    }

    public new int Count
    {
      get { return items.Count; }
    }

    public new NodeModel this[int index]
    {
      get { return items.ElementAtOrDefault(index); }
    }

    public void Add(NodeModel node)
    {
      Adopt(node);
      items.Add(node);
    }

    public void Add(object value)
    {
      var node = new ValueModel(value);
      Adopt(node);
      items.Add(node);
    }

    public void AddRange(IEnumerable<NodeModel> range)
    {
      Adopt(range);
      items.AddRange(range);
    }

    public void AddRange(IEnumerable<object> range)
    {
      AddRange(range.Select(x => (x is NodeModel) ? (NodeModel)x : new ValueModel(x)));
    }

    public void Insert(int index, NodeModel node)
    {
      Adopt(node);
      items.Insert(index, node);
    }

    public void Insert(int index, object value)
    {
      var node = new ValueModel(value);
      Adopt(node);
      items.Insert(index, node);
    }

    public void InsertRange(int index, IEnumerable<NodeModel> range)
    {
      Adopt(range);
      items.InsertRange(index, range);
    }

    public void InsertRange(int index, IEnumerable<object> range)
    {
      InsertRange(index, range.Select(x => (x is NodeModel) ? (NodeModel)x : new ValueModel(x)));
    }

    public void Remove(NodeModel node)
    {
      Reject(node);
      items.Remove(node);
    }

    public void Remove(object value)
    {
      RemoveRange(items.OfType<ValueModel>().Where(x => x.Value == value));
    }

    public void RemoveAt(int index)
    {
      var node = items[index];
      Reject(node);
      items.RemoveAt(index);
    }

    public void RemoveRange(IEnumerable<NodeModel> range)
    {
      Reject(range);
      items.RemoveAll(p => range.Contains(p));
    }

    public void RemoveAll(Func<NodeModel, bool> match)
    {
      var range = items.Where(match);
      Reject(range);
      items.RemoveAll(p => match.Invoke(p));
    }

    public void RemoveAll()
    {
      Reject(items);
      items.Clear();
    }

    private void Adopt(IEnumerable<NodeModel> nodes)
    {
      foreach (var node in nodes)
      {
        node.Parent = this;
      }
    }

    private void Adopt(NodeModel node)
    {
      node.Parent = this;
    }

    private void Reject(IEnumerable<NodeModel> nodes)
    {
      foreach (var node in nodes)
      {
        if (node.Parent == this)
        {
          node.Parent = null;
        }
      }
    }

    private void Reject(NodeModel node)
    {
      if (node.Parent == this)
      {
        node.Parent = null;
      }
    }

  }
}