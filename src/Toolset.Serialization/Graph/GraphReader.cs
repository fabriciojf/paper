using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Xml.Linq;
using Toolset.Serialization.Xml;

namespace Toolset.Serialization.Graph
{
  public sealed class GraphReader : Reader
  {
    private readonly Stack<object> stack;
    private readonly Func<object, bool> IgnoreValueMethod;

    private Node currentNode;
    private bool done;

    public GraphReader(object graph)
      : this(graph, new SerializationSettings())
    {
    }

    public GraphReader(object graph, SerializationSettings settings)
      : base(settings)
    {
      this.stack = new Stack<object>();
      this.stack.Push(graph);

      this.IgnoreValueMethod =
        x => IgnoreValue(
                x
              , this.Settings.IgnoreFalseProperties
              , this.Settings.IgnoreDefaultProperties
              , this.Settings.IgnoreEmptyProperties
              , this.Settings.IgnoreNullProperties
              );

      base.IsValid = true;
    }

    public new GraphSerializationSettings Settings
    {
      get { return base.Settings.As<GraphSerializationSettings>(); }
    }

    public override Node Current
    {
      get { return currentNode; }
    }

    protected override bool DoRead()
    {
      if (done)
        return false;

      if (currentNode == null)
      {
        currentNode = new Node { Type = Serialization.NodeType.DocumentStart };
      }
      else if (!stack.Any())
      {
        done = true;
        currentNode = new Node { Type = Serialization.NodeType.DocumentEnd };
      }
      else
      {
        currentNode = CreateNode();
      }

      return true;
    }

    public override void Close()
    {
      // nada a fazer...
    }

    private Node CreateNode()
    {
      var item = stack.Pop();

      if (item is Node)
      {
        return (Node)item;
      }

      #region Valores simples...

      else if (item == null)
      {
        return new Node { Type = NodeType.Value };
      }
      else if (IsSimpleValue(item))
      {
        return new Node { Type = NodeType.Value, Value = item };
      }

      #endregion

      #region Xmls

      else if (item is XContainer)
      {
        // var xml = (XContainer)item;
        // var text = xml.ToString(SaveOptions.DisableFormatting);
        // return new Node { Type = NodeType.Value, Value = text };
        return new Node { Type = NodeType.Value, Value = item };
      }

      #endregion

      #region Propriedades...

      else if (item is PropertiesHolder)
      {
        var properties = (PropertiesHolder)item;
        var graph = properties.Graph;
        var enumerator = properties.Enumerator;

        var ready = enumerator.MoveNext();
        if (!ready)
          return CreateNode();

        stack.Push(properties);

        var graphInfo = new GraphInfo(graph.GetType());

        var property = (PropertyInfo)enumerator.Current;
        var propertyValue = property.GetValue(graph, null);

        var ignore = IgnoreValueMethod.Invoke(propertyValue);
        if (ignore)
          return CreateNode();

        var name = graphInfo.GetPropertyLabel(property);
        var propertyName = ValueConventions.CreateName(name, Settings, TextCase.PascalCase);

        stack.Push(new Node { Type = NodeType.PropertyEnd });
        stack.Push(propertyValue);
        return new Node { Type = NodeType.PropertyStart, Value = propertyName };
      }

      #endregion

      #region Dicionarios, tratados como objetos...

      else if (Collections.IsDictionary(item))
      {
        var dictionary = (IDictionary)item;
        var enumerator = dictionary.GetEnumerator();

        var name = item.GetType().Name;
        var propertyName = ValueConventions.CreateName(name, Settings, TextCase.PascalCase);

        stack.Push(new Node { Type = NodeType.ObjectEnd });
        stack.Push(enumerator);
        return new Node { Type = NodeType.ObjectStart, Value = propertyName };
      }
      else if (item is DictionaryEntry)
      {
        var entry = (DictionaryEntry)item;
        stack.Push(new Node { Type = NodeType.PropertyEnd });
        stack.Push(entry.Value);

        var name = entry.Key.ToString();
        var propertyName = ValueConventions.CreateName(name, Settings, TextCase.PascalCase);

        return new Node { Type = NodeType.PropertyStart, Value = name };
      }
        
      #endregion
        
      #region Listas...

      else if (item is IEnumerable)
      {
        var collection = (IEnumerable)item;
        var enumerator = collection.GetEnumerator();

        var collectionType = collection.GetType();
        var collectionInfo = new GraphInfo(collectionType);

        var name = collectionInfo.GetLabel();
        var collectionName = ValueConventions.CreateName(name, Settings, TextCase.PascalCase);

        stack.Push(new Node { Type = NodeType.CollectionEnd });
        stack.Push(enumerator);
        return new Node { Type = NodeType.CollectionStart, Value = collectionName };
      }
      else if (item is IEnumerator)
      {
        var enumerator = (IEnumerator)item;

        var ready = enumerator.MoveNext();
        if (ready)
        {
          stack.Push(enumerator);
          stack.Push(enumerator.Current);
        }

        return CreateNode();
      }

      #endregion

      #region Objetos...

      else
      {
        var graph = item;
        var graphType = graph.GetType();
        var graphInfo = new GraphInfo(graphType);

        var properties = graphInfo.GetProperties();
        var enumerator = properties.GetEnumerator();
        var holder = new PropertiesHolder { Graph = graph, Enumerator = enumerator };

        var name = graphInfo.GetLabel();
        var propertyName = ValueConventions.CreateName(name, Settings, TextCase.PascalCase);

        stack.Push(new Node { Type = NodeType.ObjectEnd });
        stack.Push(holder);
        return new Node { Type = NodeType.ObjectStart, Value = propertyName };
      }

      #endregion
    }

    private bool IsSimpleValue(object value)
    {
      var isPrimitive = 
        value.GetType().IsPrimitive
        || value is string
        || value is Enum
        || value is DateTime
        || value is TimeSpan;
      return isPrimitive;
    }

    private bool IgnoreValue(object value, bool caseFalse, bool caseDefault, bool caseEmpty, bool caseNull)
    {
      if (caseFalse && IsFalseValue(value)) return true;
      if (caseDefault && IsDefaultValue(value)) return true;
      if (caseEmpty && IsEmptyValue(value)) return true;
      if (caseNull && IsNullValue(value)) return true;
      return false;
    }

    private bool IsFalseValue(object value)
    {
      if (value == null)
        return true;

      return (value is bool) && ((bool)value) == false;
    }

    private bool IsDefaultValue(object value)
    {
      if (value == null)
        return true;

      if (value.GetType().IsValueType)
      {
        var emptyValue = Activator.CreateInstance(value.GetType());
        return value.Equals(emptyValue);
      }
      return false;
    }

    private bool IsEmptyValue(object value)
    {
      if (value == null)
        return true;

      return (value is string) && string.IsNullOrEmpty((string)value);
    }

    private bool IsNullValue(object value)
    {
      return value == null;
    }

    private struct PropertiesHolder
    {
      public object Graph;
      public IEnumerator<PropertyInfo> Enumerator;
    }
  }

}
