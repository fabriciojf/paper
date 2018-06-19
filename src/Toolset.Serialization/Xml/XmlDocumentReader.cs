using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Toolset.Serialization.Xml
{
  public sealed class XmlDocumentReader : Reader
  {
    private readonly System.Xml.XmlReader reader;
    private readonly Stack<NodeType> stack;
    private readonly IEnumerator<Node> enumerator;

    private Node currentNode;

    #region Construtores extras ...

    public XmlDocumentReader(TextReader textReader)
      : this(textReader, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XmlDocumentReader(System.Xml.XmlReader xmlReader)
      : this(xmlReader, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XmlDocumentReader(Stream textStream)
      : this(new StreamReader(textStream), (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XmlDocumentReader(Stream textStream, SerializationSettings settings)
      : this(new StreamReader(textStream), settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XmlDocumentReader(string filename)
      : this(File.OpenRead(filename), (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XmlDocumentReader(string filename, SerializationSettings settings)
      : this(File.OpenRead(filename), settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    #endregion 

    public XmlDocumentReader(TextReader reader, SerializationSettings settings)
      : base(settings)
    {
      this.reader = System.Xml.XmlReader.Create(reader);
      this.stack = new Stack<NodeType>();
      this.enumerator = EnumerateNodes().GetEnumerator();
      base.IsValid = true;
    }

    public XmlDocumentReader(System.Xml.XmlReader reader, SerializationSettings settings)
      : base(settings)
    {
      this.reader = reader;
      this.stack = new Stack<NodeType>();
      this.enumerator = EnumerateNodes().GetEnumerator();
      base.IsValid = true;
    }

    public new XmlSerializationSettings Settings
    {
      get { return base.Settings.As<XmlSerializationSettings>(); }
    }

    public override Node Current
    {
      get { return currentNode; }
    }

    protected override bool DoRead()
    {
      var ready = this.enumerator.MoveNext();
      currentNode = ready ? this.enumerator.Current : null;
      return ready;
    }

    private IEnumerable<Node> EnumerateNodes()
    {
      if (!Settings.IsFragment)
      {
        yield return new Node { Type = NodeType.DocumentStart };
      }

      var nodes = EmitNodes(reader);
      foreach (var node in nodes)
      {
        yield return node;
      }

      if (!Settings.IsFragment)
      {
        yield return new Node { Type = NodeType.DocumentEnd };
      }
    }

    private IEnumerable<Node> EmitNodes(XmlReader reader)
    {
      // Propriedade sem EndElement é considerada nula: <tag/>
      // Propriedade com EndElement e sem conteúdo é considerada vazia: <tag></tag>
      // Tag nula é identificada por IsEmptyElement.
      // Tag vazia é identificada por esta variável, que estoca o último tipo de nodo encontrdo.
      // Se este último nodo foi um abre propriedade e estamos fechando esta propriedade então
      // não vimos valor algum.
      Node emitted = null;

      while (reader.Read())
      {

        var nodeType = reader.NodeType;
        var nodeName = reader.Name;
        var nodeValue = reader.Value;
        var isEmptyElement = reader.IsEmptyElement;

        switch (nodeType)
        {
          case XmlNodeType.Element:
            {
              var isCollection = IsCollection();

              var parentKind = stack.FirstOrDefault();
              var parentIsProperty = (parentKind == NodeType.Property);
              if (parentIsProperty)
              {
                var name = "Object";
                var conventionName = ValueConventions.CreateName(name, Settings, TextCase.KeepOriginal);
                stack.Push(NodeType.Object);
                yield return emitted = new Node { Type = NodeType.ObjectStart, Value = conventionName };

                parentKind = stack.FirstOrDefault();
              }

              var isProperty = (parentKind == NodeType.Object);
              if (isProperty)
              {
                var name = nodeName ?? "Property";
                var conventionName = ValueConventions.CreateName(name, Settings, TextCase.KeepOriginal);

                yield return emitted = new Node { Type = NodeType.PropertyStart, Value = conventionName };

                if (reader.IsEmptyElement)
                {
                  // tags vazias não tem um EndElement correspondente, como em: <tag/>
                  // por isto vamos emitir um aqui e com um valor padrao.
                  if (isCollection)
                  {
                    yield return emitted = new Node { Type = NodeType.CollectionStart, Value = "Collection" };
                    yield return emitted = new Node { Type = NodeType.CollectionEnd };

                    // Como estamos fechando a coleção para forçar uma coleção vazia como
                    // valor padrão vamos também abortar a interpretação da coleção na continuidade.
                    // A propriedade foi detectada em uma tag vazia como <Xml IsArray="true"/> e
                    // portanto não há realmente coleção para interpretar
                    isCollection = false;
                  }
                  else
                  {
                    yield return emitted = new Node { Type = NodeType.Value, Value = null };
                  }

                  yield return emitted = new Node { Type = NodeType.PropertyEnd };
                }
                else
                {
                  stack.Push(NodeType.Property);
                }
              }

              if (isCollection)
              {
                var name = nodeName ?? "Collection";
                var conventionName = ValueConventions.CreateName(name, Settings, TextCase.KeepOriginal);
                stack.Push(NodeType.Collection);
                yield return emitted = new Node { Type = NodeType.CollectionStart, Value = conventionName };
                if (reader.IsEmptyElement)
                {
                  yield return emitted = new Node { Type = NodeType.CollectionEnd };
                }
              }

              else if (!isProperty && !isCollection)
              {
                var name = nodeName ?? "Object";
                var conventionName = ValueConventions.CreateName(name, Settings, TextCase.KeepOriginal);
                stack.Push(NodeType.Object);
                yield return emitted = new Node { Type = NodeType.ObjectStart, Value = conventionName };
                if (reader.IsEmptyElement)
                {
                  yield return emitted = new Node { Type = NodeType.ObjectEnd };
                }
              }

              break;
            }

          case XmlNodeType.EndElement:
            {
              var kind = stack.Pop();
              var parentKind = stack.FirstOrDefault();

              if (kind == NodeType.Property)
              {
                // se o nodo emitido anteriormente tiver sido abre propriedade então não vimos
                // um valor e devemos tratar a propriedade como vazia
                if (emitted.Type == NodeType.PropertyStart)
                  yield return emitted = new Node { Type = NodeType.Value, Value = string.Empty };
              }

              yield return emitted = new Node { Type = kind | NodeType.End };

              if (kind == NodeType.Collection)
              {
                var isProperty = (parentKind == NodeType.Property);
                if (isProperty)
                {
                  kind = stack.Pop();
                  yield return emitted = new Node { Type = kind | NodeType.End };
                }
              }
              else if (parentKind == NodeType.Property)
              {
                kind = stack.Pop();
                yield return emitted = new Node { Type = kind | NodeType.End };
              }

              break;
            }

          case XmlNodeType.CDATA:
          case XmlNodeType.Text:
            {
              var value = ValueConventions.CreateValue(nodeValue, Settings);
              yield return emitted = new Node { Type = NodeType.Value, Value = value };
              break;
            }
        }

      } // while (reader.Read())
    }

    private bool IsCollection()
    {
      var isCollection = false;

      if (reader.HasAttributes)
      {
        var ok = reader.MoveToFirstAttribute();
        while (ok)
        {
          var name = reader.Name;
          var attName = name.ChangeCase(TextCase.PascalCase);
          if (attName.Equals("IsArray"))
          {
            isCollection = (reader.Value == "true") || (reader.Value == "1");
            break;
          }
          ok = reader.MoveToNextAttribute();
        }
      }

      reader.MoveToElement();
      return isCollection;
    }

    public override void Close()
    {
      if (!Settings.KeepOpen)
      {
        if (reader != null)
        {
          reader.Close();
        }
      }
    }
  }
}
