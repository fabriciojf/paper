using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace Toolset.Serialization.Xml
{
  public sealed class XsltTransformWriter : Writer
  {
    private readonly XsltArgumentList arguments;
    private readonly Action transformation;
    private readonly Action completeAction;
    private readonly Action closeAction;
    private readonly Action flushAction;

    private readonly MemoryStream buffer;
    private readonly XmlDocumentWriter bufferWriter;

    private int depth;
    
    #region Construtores extras ...

    #region Construtores XmlWriter ...

    public XsltTransformWriter(XmlWriter writer, XmlReader xsltReader, XsltArgumentList arguments)
      : this(writer, xsltReader, arguments, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformWriter(XmlWriter writer, XmlReader xsltReader, SerializationSettings settings)
      : this(writer, xsltReader, null, settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformWriter(XmlWriter writer, XmlReader xsltReader)
      : this(writer, xsltReader, null, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    #endregion

    #region Construtores TextWriter ...

    public XsltTransformWriter(TextWriter writer, XmlReader xsltReader, XsltArgumentList arguments, SerializationSettings settings)
      : this(XmlWriter.Create(writer), xsltReader, arguments, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformWriter(TextWriter writer, XmlReader xsltReader, XsltArgumentList arguments)
      : this(XmlWriter.Create(writer), xsltReader, arguments, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformWriter(TextWriter writer, XmlReader xsltReader, SerializationSettings settings)
      : this(XmlWriter.Create(writer), xsltReader, null, settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformWriter(TextWriter writer, XmlReader xsltReader)
      : this(XmlWriter.Create(writer), xsltReader, null, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    #endregion

    #region Construtores Stream ...

    public XsltTransformWriter(Stream stream, XmlReader xsltReader, XsltArgumentList arguments, SerializationSettings settings)
      : this(new StreamWriter(stream), xsltReader, arguments, settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformWriter(Stream stream, XmlReader xsltReader, XsltArgumentList arguments)
      : this(new StreamWriter(stream), xsltReader, arguments, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformWriter(Stream stream, XmlReader xsltReader, SerializationSettings settings)
      : this(new StreamWriter(stream), xsltReader, null, settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformWriter(Stream stream, XmlReader xsltReader)
      : this(new StreamWriter(stream), xsltReader, null, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    #endregion

    #region Construtores Arquivo ...

    public XsltTransformWriter(string filename, XmlReader xsltReader, XsltArgumentList arguments, SerializationSettings settings)
      : this(File.OpenWrite(filename), xsltReader, arguments, settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformWriter(string filename, XmlReader xsltReader, XsltArgumentList arguments)
      : this(File.OpenWrite(filename), xsltReader, arguments, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformWriter(string filename, XmlReader xsltReader, SerializationSettings settings)
      : this(File.OpenWrite(filename), xsltReader, null, settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformWriter(string filename, XmlReader xsltReader)
      : this(File.OpenWrite(filename), xsltReader, null, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    #endregion

    #region Construtores Writer ...

    public XsltTransformWriter(Writer writer, XmlReader xsltReader, XsltArgumentList arguments)
      : this(writer, xsltReader, arguments, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformWriter(Writer writer, XmlReader xsltReader, SerializationSettings settings)
      : this(writer, xsltReader, null, settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformWriter(Writer writer, XmlReader xsltReader)
      : this(writer, xsltReader, null, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    #endregion

    #endregion

    public XsltTransformWriter(XmlWriter writer, XmlReader xsltReader, XsltArgumentList arguments, SerializationSettings settings)
      : base(new XmlSerializationSettings(settings))
    {
      this.arguments = arguments;

      this.transformation = () => TransformAndWrite(writer, xsltReader);
      this.completeAction = () => { writer.Flush(); };
      this.flushAction = () => { writer.Flush(); };
      this.closeAction = () =>
      {
        writer.Flush();
        if (!this.Settings.KeepOpen)
        {
          writer.Close();
          xsltReader.Close();
        }
      };

      this.buffer = new MemoryStream();
      this.bufferWriter = new XmlDocumentWriter(new StreamWriter(this.buffer) { AutoFlush = true }, settings);
    }

    public XsltTransformWriter(Writer writer, XmlReader xsltReader, XsltArgumentList arguments, SerializationSettings settings)
      : base(new XmlSerializationSettings(settings))
    {
      this.arguments = arguments;

      this.transformation = () => TransformAndWrite(writer, xsltReader);
      this.completeAction = () => { writer.WriteComplete(); };
      this.closeAction = () => { writer.Close(); xsltReader.Close(); };
      this.flushAction= () => { writer.Flush(); };

      this.buffer = new MemoryStream();
      this.bufferWriter = new XmlDocumentWriter(new StreamWriter(this.buffer) { AutoFlush = true }, settings);
    }

    public new XmlSerializationSettings Settings
    {
      get { return base.Settings.As<XmlSerializationSettings>(); }
    }

    protected override void DoWrite(Node node)
    {
      this.bufferWriter.Write(node);

      if (node.Type.HasFlag(NodeType.Start))
        depth++;
      if (node.Type.HasFlag(NodeType.End))
        depth--;

      if (depth == 0)
        this.transformation.Invoke();
    }

    protected override void DoWriteComplete()
    {
      this.completeAction.Invoke();
    }

    protected override void DoFlush()
    {
      this.flushAction.Invoke();
    }

    protected override void DoClose()
    {
      this.closeAction.Invoke();
    }

    private void TransformAndWrite(Writer documentWriter, XmlReader xsltReader)
    {
      var memory = new MemoryStream();
      var writer = new StreamWriter(memory) { AutoFlush = true };
      var reader = new StreamReader(memory);

      var xmlWriter = XmlWriter.Create(writer);
      TransformAndWrite(xmlWriter, xsltReader);
      memory.Position = 0;

      var xmlReader = new XmlDocumentReader(reader, null);
      xmlReader.CopyTo(documentWriter);
    }

    private void TransformAndWrite(XmlWriter xmlWriter, XmlReader xsltReader)
    {
      using (buffer)
      {
        buffer.Position = 0;

        var reader = XmlReader.Create(buffer);
        var writer = XmlWriter.Create(xmlWriter,
          new XmlWriterSettings
          {
            Indent = this.Settings.Indent,
            IndentChars = this.Settings.IndentChars,
            Encoding = Encoding.UTF8,
            OmitXmlDeclaration = true
          }
        );

        var xslt = new XslCompiledTransform();
        xslt.Load(xsltReader);

        xslt.Transform(reader, writer);
      }
    }

  }
}