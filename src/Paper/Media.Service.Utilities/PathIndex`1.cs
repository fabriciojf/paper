using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolset;
using Toolset.Collections;

namespace Media.Service.Utilities
{
  public class PathIndex<TValue>
    where TValue : class
  {
    private readonly Node<TValue> entries = new Node<TValue>(null, null);

    public string[] Paths => EnumeratePaths().ToArray();

    private IEnumerable<string> EnumeratePaths()
    {
      var stack = new Stack<Node<TValue>>();
      stack.Push(entries);

      while (stack.Count > 0)
      {
        var node = stack.Pop();

        if (node.Path != null)
          yield return node.Path;

        node.Values.ForEach(n => stack.Push(n));
      }
    }

    public void Add(string path, TValue value)
    {
      Node<TValue> node = entries;
      foreach (var token in path.ToLower().Split('/').NonEmpty())
      {
        var key = token.Contains("{") ? "*" : token;
        if (!node.ContainsKey(key))
        {
          node[key] = new Node<TValue>(node, key);
        }
        node = node[key];
      }
      node.Path = path;
      node.Value = value;
    }

    public void Remove(string path)
    {
      var node = FindNodeExact(path);
      if (node != null)
      {
        node.Path = null;
        node.Value = null;
      }
    }

    public TValue FindExact(string path)
    {
      var node = FindNodeExact(path);
      return node?.Value;
    }

    public TValue FindByPrefix(string path)
    {
      var node = FindNodeByPrefix(path);
      return node?.Value;
    }

    private Node<TValue> FindNodeExact(string path)
    {
      Node<TValue> index = entries;
      foreach (var token in path.ToLower().Split('/').NonEmpty())
      {
        var key = index.ContainsKey(token) ? token : index.ContainsKey("*") ? "*" : null;
        if (key == null)
          return null;
        index = index[key];
      }
      return index;
    }

    private Node<TValue> FindNodeByPrefix(string path)
    {
      var tokens = path.ToLower().Split('/').NonEmpty().GetEnumerator();

      tokens.MoveNext();

      var node = entries[tokens.Current];
      if (node == null)
        return null;

      while (tokens.MoveNext())
      {
        if (!node.ContainsKey(tokens.Current))
          break;

        node = node[tokens.Current];
      }

      return node;
    }

    public class Node<T> : Map<string, Node<T>>
      where T : class
    {
      public Node(Node<T> parent, string key)
      {
        this.Key= key;
      }

      public Node<T> Parent { get; }

      public string Key { get; }

      public bool IsRoot => (Parent == null);

      public string Path { get; set; }

      public T Value { get; set; }
    }
  }
}