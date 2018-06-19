using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Xml.Linq;

namespace Toolset.Serialization.Graph
{
  public class GraphWriter<T> : GraphWriter
  {
    public GraphWriter()
      : base(typeof(T))
    {
    }

    public GraphWriter(SerializationSettings settings)
      : base(typeof(T), settings)
    {
    }

    public GraphWriter(IEnumerable<Type> knownTypes, SerializationSettings settings)
      : base(typeof(T), knownTypes, settings)
    {
    }

    public new T TargetObject
    {
      get { return (base.TargetObject is T) ? (T)base.TargetObject : default(T); }
    }
  }

  public class GraphWriter : Writer
  {
    private Type type;
    private IEnumerable<Type> knownTypes;

    private Stack<IHolder> stack;
    private Stack<Stack<IHolder>> cache;
    private IHolder typeHolder;

    // Quando Settings.IsLenient é marcado uma propriedade sem correspondencia
    // no objeto é lida com sucesso do Reader mas não é atribuída ao objeto.
    // Por padrão, uma exceção seria lançada, mas quando leniente é marcado a
    // propriedade é simplesmente ignorada se não existir.
    private readonly NodeSkipper nodeSkipper = new NodeSkipper();

    public GraphWriter(Type type)
      : this(type, null, new SerializationSettings())
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public GraphWriter(Type type, SerializationSettings settings)
      : this(type, null, settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public GraphWriter(Type type, IEnumerable<Type> knownTypes, SerializationSettings settings)
      : base(settings)
    {
      this.type = type;
      this.knownTypes = new KnownTypes(type, knownTypes);

      this.stack = new Stack<IHolder>();
      this.cache = new Stack<Stack<IHolder>>();
      this.typeHolder = new ValueHolder { HintType = type };

      base.IsValid = true;
    }

    public new GraphSerializationSettings Settings
    {
      get { return base.Settings.As<GraphSerializationSettings>(); }
    }

    public object TargetObject
    {
      get;
      private set;
    }

    private IHolder GetHolderParent()
    {
      var stackedStack = cache.FirstOrDefault();
      return (stackedStack != null) ? stackedStack.FirstOrDefault() : typeHolder;
    }

    protected override void DoWrite(Node node)
    {
      if (nodeSkipper.ShouldSkip(node))
        return;

      switch (node.Type)
      {
        case NodeType.ObjectStart:
          {
            Type type = null;

            var name = (node.Value ?? "").ToString();
            var className = name.ChangeCase(TextCase.PascalCase);
            
            var parent = GetHolderParent();
            if (parent != null)
              type = parent.HintType;
            if (type == null)
              type = knownTypes.First(t => (t.Name.Equals(className)) || (t.FullName.Equals(name)));

            var instance = Activator.CreateInstance(type);

            var holder = new ObjectHolder { Value = instance };
            stack.Push(holder);
            cache.Push(stack);
            stack = new Stack<IHolder>();
            break;
          }

        case NodeType.ObjectEnd:
          {
            stack = cache.Pop();
            break;
          }

        case NodeType.CollectionStart:
          {
            Type type = null;

            var name = (node.Value ?? "").ToString();
            var className = name.ChangeCase(TextCase.PascalCase);

            var parent = GetHolderParent();
            if (parent != null)
              type = parent.HintType;
            if (type == null)
              type = knownTypes.First(t => (t.Name.Equals(className)) || (t.FullName.Equals(name)));

            var holder = new CollectionHolder { CollectionType = type };
            stack.Push(holder);
            cache.Push(stack);
            stack = new Stack<IHolder>();
            break;
          }

        case NodeType.CollectionEnd:
          {
            var items = stack.Reverse().Select(h => h.Value);
            stack = cache.Pop();
            var holder = (CollectionHolder)stack.Peek();
            holder.AddItems(items);
            break;
          }

        case NodeType.PropertyStart:
          {
            var parent = (ObjectHolder)GetHolderParent();
            var host = parent.Value;

            IHolder holder = null;
            if (Collections.IsDictionary(host))
            {
              var dictionary = (IDictionary)host;

              var name = (node.Value ?? "").ToString();
              var propertyName = ValueConventions.CreateName(name, Settings, TextCase.KeepOriginal);

              var hintType = 
                Collections.GetGenericArguments(host).Skip(1).FirstOrDefault()
                ?? typeof(object);

              holder = new EntryHolder { Host = dictionary, HintType = hintType, Key = propertyName };
            }
            else
            {
              var name = node.Value.ToString();
              var propertyName = name.ChangeCase(TextCase.PascalCase);

              var properties = host.GetType().GetProperties();
              var property = properties.FirstOrDefault(
                p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

              if (property == null)
              {
                if (Settings.IsLenient)
                {
                  nodeSkipper.Activate();
                  nodeSkipper.ShouldSkip(node); // inicia o descarte desta propriedade
                  break;
                }
                else
                {
                  throw new Exception(
                    "Propriedade não encontrada: " + propertyName + " em " + host.GetType().FullName);
                }
              }

              holder = new PropertyHolder { Host = host, Property = property };
            }

            stack.Push(holder);
            cache.Push(stack);
            stack = new Stack<IHolder>();
            break;
          }

        case NodeType.PropertyEnd:
          {
            var valueHolder = stack.SingleOrDefault();
            var value = (valueHolder != null) ? valueHolder.Value : null;

            stack = cache.Pop();
            var holder = stack.Peek();
            if (holder is EntryHolder)
            {
              var entryHoder = (EntryHolder)holder;
              var host = entryHoder.Host;
              var key = entryHoder.Key;
              var type = entryHoder.HintType;
              var changedValue = Castings.Cast(value, type);
              host.Add(key, changedValue);
            }
            else
            {
              var propertyHoder = (PropertyHolder)holder;
              var host = propertyHoder.Host;
              var property = propertyHoder.Property;
              var changedValue = Castings.Cast(value, property.PropertyType);
              property.SetValue(host, changedValue, null);
            }

            break;
          }

        case NodeType.Value:
          {
            var parent = GetHolderParent();

            if (parent.HintType == typeof(XElement))
            {
              var text = (string)node.Value;
              var xml = XElement.Parse(text);
              stack.Push(new ValueHolder { Value = xml });
            }
            else if (parent.HintType == typeof(XDocument))
            {
              var text = (string)node.Value;
              var xml = XDocument.Parse(text);
              stack.Push(new ValueHolder { Value = xml });
            }
            else
            
            {
              var type = parent.HintType ?? typeof(object);
              var changedValue = Castings.Cast(node.Value, type);
              stack.Push(new ValueHolder { Value = changedValue });
            }

            break;
          }
      }
    }

    protected override void DoWriteComplete()
    {
      var rootStack = cache.LastOrDefault() ?? stack;
      var rootHolder = (rootStack != null) ? rootStack.LastOrDefault() : null;
      TargetObject = (rootHolder != null) ? rootHolder.Value : null;
    }

    protected override void DoFlush()
    {
      // nada a fazer...
    }

    protected override void DoClose()
    {
      // nada a fazer...
    }

    #region Classes auxiliares...

    private interface IHolder
    {
      Type HintType { get; }
      object Value { get; }
    }

    private struct ValueHolder : IHolder
    {
      public Type HintType { get; set; }
      public object Value { get; set; }
    }

    private struct ObjectHolder : IHolder
    {
      public Type HintType { get { throw new NotImplementedException(); } }
      public Object Value { get; set; }
    }

    private class PropertyHolder : IHolder
    {
      public Type HintType { get { return Property.PropertyType; } }
      public Object Value { get { return Property.GetValue(Host, null); } }
      public object Host { get; set; }
      public PropertyInfo Property { get; set; }
    }

    private class EntryHolder : IHolder
    {
      public Type HintType { get; set; }
      public IDictionary Host { get; set; }
      public string Key { get; set; }
      public Object Value { get { return Host.Contains(Key) ? Host[Key] : null; } }
    }

    private class CollectionHolder : IHolder
    {
      public object Value { get; private set; }

      public Type HintType
      {
        get
        {
          return 
            CollectionType.GetElementType()
            ?? Collections.GetGenericArguments(CollectionType).FirstOrDefault()
            ?? CollectionType;
        }
      }

      public Type CollectionType
      {
        get;
        set;
      }

      public void AddItems(IEnumerable items)
      {
        var collection = ConvertToCollection(items.Cast<object>(), CollectionType);
        Value = collection;
      }

      private object ConvertToCollection(IEnumerable<object> items, Type collectionType)
      {
        if (collectionType.IsArray)
        {
          var itemType = collectionType.GetElementType();
          var targetArray = Array.CreateInstance(itemType, items.Count());
          var sourceArray = items.Select(x => Castings.Cast(x, itemType)).ToArray();
          Array.Copy(sourceArray, targetArray, targetArray.Length);
          return targetArray;
        }
        else
        {
          var targetCollection = (ICollection)Activator.CreateInstance(collectionType);
          
          var itemType = Collections.GetGenericArguments(collectionType).FirstOrDefault() ?? typeof(object);
          var targetAppender = collectionType.GetMethod("Add", new[] { itemType });

          foreach (var item in items)
          {
            var itemValue = Castings.Cast(item, itemType);
            targetAppender.Invoke(targetCollection, new[] { itemValue });
          }
                               
          return targetCollection;
        }
      }
    }

    #endregion

  }
}
