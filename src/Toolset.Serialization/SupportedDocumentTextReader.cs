using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Toolset.Serialization.Xml;

namespace Toolset.Serialization
{
  public class SupportedDocumentTextReader : TextReaderComposite
  {
    public const string XmlFormat = "xml";
    public const string JsonFormat = "json";
    public const string CsvFormat = "csv";
    public const string UnknownFormat = "unknown";

    private SupportedDocumentTextReader(string format, params TextReader[] readers)
      : base(readers)
    {
      this.DocumentFormat = format;
    }

    public string DocumentFormat
    {
      get;
      private set;
    }

    public static SupportedDocumentTextReader Create(Stream stream)
    {
      var reader = new StreamReader(stream, true);
      return Create(reader);
    }

    public static SupportedDocumentTextReader Create(TextReader reader)
    {
      var format = SupportedDocumentTextReader.UnknownFormat;

      var memory = new MemoryStream();
      var writer = new StreamWriter(memory);

      while (reader.Peek() > -1)
      {
        var ch = (char)reader.Read();

        writer.Write(ch);
        writer.Flush();

        if (!char.IsWhiteSpace(ch))
        {
          if (ch == '<')
            format = SupportedDocumentTextReader.XmlFormat;
          else if (ch == '{' || ch == '[')
            format = SupportedDocumentTextReader.JsonFormat;
          else
            format = SupportedDocumentTextReader.CsvFormat;

          break;
        }
      }

      memory.Position = 0;
      var memoryReader = new StreamReader(memory);

      var compositeReader = new SupportedDocumentTextReader(format, memoryReader, reader);
      return compositeReader;
    }
  }
}
