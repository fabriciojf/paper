using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Toolset.Serialization.Xml
{
  public sealed class XmlDocumentWriter : Writer
  {
    private readonly System.Xml.XmlWriter writer;
    private readonly Stack<NodeType> stack;
    
    #region Construtores extras ...

    public XmlDocumentWriter(TextWriter textWriter)
      : this(textWriter, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XmlDocumentWriter(System.Xml.XmlWriter xmlWriter)
      : this(xmlWriter, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XmlDocumentWriter(Stream textStream)
      : this(new StreamWriter(textStream), (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XmlDocumentWriter(Stream textStream, SerializationSettings settings)
      : this(new StreamWriter(textStream), settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XmlDocumentWriter(string filename)
      : this(File.OpenWrite(filename), (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XmlDocumentWriter(string filename, SerializationSettings settings)
      : this(File.OpenWrite(filename), settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    #endregion 

    public XmlDocumentWriter(TextWriter writer, SerializationSettings settings)
      : base(settings)
    {
      this.writer = System.Xml.XmlWriter.Create(writer,
        new XmlWriterSettings
        {
          Indent = this.Settings.Indent,
          IndentChars = this.Settings.IndentChars,
          Encoding = Encoding.UTF8,
          OmitXmlDeclaration = true
        }
      );
      this.stack = new Stack<NodeType>();
      base.IsValid = true;
    }

    public XmlDocumentWriter(System.Xml.XmlWriter writer, SerializationSettings settings)
      : base(settings)
    {
      this.writer = writer;
      this.stack = new Stack<NodeType>();
      base.IsValid = true;
    }

    public new XmlSerializationSettings Settings
    {
      get { return base.Settings.As<XmlSerializationSettings>(); }
    }

    protected override void DoWrite(Node node)
    {
      switch (node.Type)
      {
        case NodeType.DocumentStart:
        case NodeType.DocumentEnd:
          // nada a fazer
          break;

        case NodeType.ObjectStart:
          {
            var parentKind = stack.FirstOrDefault();
            var parentIsProperty = parentKind == NodeType.Property;

            if (!parentIsProperty)
            {
              var name = (node.Value ?? "Element").ToString();
              var tagName = ValueConventions.CreateName(name, Settings, TextCase.PascalCase);

              writer.WriteStartElement(tagName);
            }

            stack.Push(NodeType.Object);
            break;
          }

        case NodeType.ObjectEnd:
          {
            stack.Pop();

            var parentKind = stack.FirstOrDefault();
            var parentIsProperty = parentKind == NodeType.Property;

            if (!parentIsProperty)
            {
              writer.WriteEndElement();
            }
            break;
          }

        case NodeType.CollectionStart:
          {
            var parentKind = stack.FirstOrDefault();
            var parentIsProperty = parentKind == NodeType.Property;

            if (!parentIsProperty)
            {
              var name = (node.Value ?? "Array").ToString();
              var tagName = ValueConventions.CreateName(name, Settings, TextCase.PascalCase);
              writer.WriteStartElement(tagName);
            }

            var attName = ValueConventions.CreateName("IsArray", Settings, TextCase.PascalCase);
            writer.WriteAttributeString(attName, "true");
            
            stack.Push(NodeType.Collection);
            break;
          }

        case NodeType.CollectionEnd:
          {
            stack.Pop();

            var parentKind = stack.FirstOrDefault();
            var parentIsProperty = parentKind == NodeType.Property;

            if (!parentIsProperty)
            {
              writer.WriteEndElement();
            }

            break;
          }

        case NodeType.PropertyStart:
          {
            var name = (node.Value ?? "Property").ToString();
            var tagName = ValueConventions.CreateName(name, Settings, TextCase.PascalCase);
            writer.WriteStartElement(tagName);

            stack.Push(NodeType.Property);
            break;
          }

        case NodeType.PropertyEnd:
          {
            writer.WriteEndElement();
            stack.Pop();
            break;
          }

        case NodeType.Value:
          {
            if (node.Value == null)
            {
              writer.WriteValue(null);
            }
            else if (node.Value is XContainer)
            {
              var xml = (XContainer)node.Value;
              var cdata = xml.ToString(SaveOptions.DisableFormatting);
              writer.WriteCData(cdata);
            }
            else
            {
              var text = ValueConventions.CreateText(node.Value, Settings);
              writer.WriteValue(text);
            }
            break;
          }

        default:
          throw new SerializationException("Token não esperado: " + node);
      }

      if (Settings.AutoFlush)
      {
        DoFlush();
      }

    }

    protected override void DoWriteComplete()
    {
      writer.Flush();
    }

    protected override void DoFlush()
    {
      writer.Flush();
    }

    protected override void DoClose()
    {
      writer.Flush();
      if (!Settings.KeepOpen)
      {
        writer.Close();
      }
    }
  }
}
