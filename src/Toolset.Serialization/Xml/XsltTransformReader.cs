using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace Toolset.Serialization.Xml
{
  public sealed class XsltTransformReader : Reader
  {
    private readonly XsltArgumentList arguments;
    private readonly Func<Reader> transformation;
    private readonly Action closeAction;

    private Reader bufferReader;

    #region Construtores extras ...

    #region Construtores XmlReader ...

    public XsltTransformReader(XmlReader reader, XmlReader xsltReader, XsltArgumentList arguments)
      : this(reader, xsltReader, arguments, (XmlSerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformReader(XmlReader reader, XmlReader xsltReader, SerializationSettings settings)
      : this(reader, xsltReader, null, new XmlSerializationSettings(settings))
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformReader(XmlReader reader, XmlReader xsltReader)
      : this(reader, xsltReader, null, (XmlSerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    #endregion

    #region Construtores TextReader ...

    public XsltTransformReader(TextReader reader, XmlReader xsltReader, XsltArgumentList arguments, SerializationSettings settings)
      : this(XmlReader.Create(reader), xsltReader, arguments, (XmlSerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformReader(TextReader reader, XmlReader xsltReader, XsltArgumentList arguments)
      : this(XmlReader.Create(reader), xsltReader, arguments, (XmlSerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformReader(TextReader reader, XmlReader xsltReader, SerializationSettings settings)
      : this(XmlReader.Create(reader), xsltReader, null, new XmlSerializationSettings(settings))
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformReader(TextReader reader, XmlReader xsltReader)
      : this(XmlReader.Create(reader), xsltReader, null, (XmlSerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    #endregion

    #region Construtores XmlDocumentReader ...

    public XsltTransformReader(XmlDocumentReader reader, XmlReader xsltReader, XsltArgumentList arguments)
      : this(reader, xsltReader, arguments, (XmlSerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformReader(XmlDocumentReader reader, XmlReader xsltReader, SerializationSettings settings)
      : this(reader, xsltReader, null, new XmlSerializationSettings(settings))
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformReader(XmlDocumentReader reader, XmlReader xsltReader)
      : this(reader, xsltReader, null, (XmlSerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    #endregion

    #region Construtores Stream ...

    public XsltTransformReader(Stream stream, XmlReader xsltReader, XsltArgumentList arguments, SerializationSettings settings)
      : this(new StreamReader(stream), xsltReader, arguments, settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformReader(Stream stream, XmlReader xsltReader, XsltArgumentList arguments)
      : this(new StreamReader(stream), xsltReader, arguments, (XmlSerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformReader(Stream stream, XmlReader xsltReader, SerializationSettings settings)
      : this(new StreamReader(stream), xsltReader, null, new XmlSerializationSettings(settings))
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformReader(Stream stream, XmlReader xsltReader)
      : this(new StreamReader(stream), xsltReader, null, (XmlSerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    #endregion

    #region Construtores Arquivo ...

    public XsltTransformReader(string filename, XmlReader xsltReader, XsltArgumentList arguments, SerializationSettings settings)
      : this(File.OpenRead(filename), xsltReader, arguments, settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformReader(string filename, XmlReader xsltReader, XsltArgumentList arguments)
      : this(File.OpenRead(filename), xsltReader, arguments, (XmlSerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformReader(string filename, XmlReader xsltReader, SerializationSettings settings)
      : this(File.OpenRead(filename), xsltReader, null, new XmlSerializationSettings(settings))
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public XsltTransformReader(string filename, XmlReader xsltReader)
      : this(File.OpenRead(filename), xsltReader, null, (XmlSerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    #endregion

    #endregion

    public XsltTransformReader(XmlReader reader, XmlReader xsltReader, XsltArgumentList arguments, SerializationSettings settings)
      : base(new XmlSerializationSettings(settings))
    {
      this.arguments = arguments;
      this.transformation = () => Transform(reader, xsltReader);
      this.closeAction = () =>
      {
        if (!this.Settings.KeepOpen)
        {
          reader.Close();
        }
      };
    }

    public XsltTransformReader(XmlDocumentReader reader, XmlReader xsltReader, XsltArgumentList arguments, SerializationSettings settings)
      : base(new XmlSerializationSettings(settings))
    {
      this.arguments = arguments;
      this.transformation = () => Transform(reader, xsltReader);
      this.closeAction = reader.Close;
    }

    public new XmlSerializationSettings Settings
    {
      get { return base.Settings.As<XmlSerializationSettings>(); }
    }

    public override Node Current
    {
      get { return bufferReader.Current; }
    }

    protected override bool DoRead()
    {
      if (bufferReader == null)
      {
        bufferReader = this.transformation.Invoke();
      }
      return bufferReader.Read();
    }

    public override void Close()
    {
      this.closeAction.Invoke();
    }

    private Reader Transform(XmlDocumentReader documentReader, XmlReader xsltReader)
    {
      var memory = new MemoryStream();
      var writer = new StreamWriter(memory) { AutoFlush = true };
      var reader = new StreamReader(memory);

      var xmlWriter = new XmlDocumentWriter(writer, null);
      documentReader.CopyTo(xmlWriter);
      memory.Position = 0;

      var xmlReader = XmlReader.Create(reader);
      return Transform(xmlReader, xsltReader);
    }

    private Reader Transform(XmlReader xmlReader, XmlReader xsltReader)
    {
      var xslt = new XslCompiledTransform();
      xslt.Load(xsltReader);

      var stringWriter = new StringWriter();
      var writer = XmlWriter.Create(stringWriter,
        new XmlWriterSettings
        {
          Indent = this.Settings.Indent,
          IndentChars = this.Settings.IndentChars,
          Encoding = Encoding.UTF8,
          OmitXmlDeclaration = true
        }
      );
      xslt.Transform(xmlReader, arguments, writer);

      var stringReader = new StringReader(stringWriter.ToString());
      return new XmlDocumentReader(stringReader, Settings);
    }
  }
}