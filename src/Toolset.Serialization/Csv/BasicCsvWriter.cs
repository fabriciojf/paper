using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Serialization.Transformations;
using System.IO;

namespace Toolset.Serialization.Csv
{
  internal class BasicCsvWriter : Writer
  {
    private readonly TextWriter writer;

    private int collectionDepth;
    private int cols;

    public BasicCsvWriter(TextWriter writer)
      : base(null)
    {
      this.writer = writer;
    }

    public BasicCsvWriter(TextWriter writer, SerializationSettings settings)
      : base(settings)
    {
      this.writer = writer;
    }

    public new CsvSerializationSettings Settings
    {
      get { return base.Settings.As<CsvSerializationSettings>(); }
    }

    protected override void DoWrite(Node node)
    {
      switch (node.Type)
      {
        case NodeType.CollectionStart:
          {
            collectionDepth++;
            if (collectionDepth == 2)
            {
              cols = 0;
            }
            break;
          }

        case NodeType.CollectionEnd:
          {
            if (collectionDepth == 2)
            {
              writer.WriteLine();
            }
            collectionDepth--;
            break;
          }

        case NodeType.Value:
          {
            cols++;
            if (cols > 1)
            {
              writer.Write(Settings.FieldDelimiter);
            }

            if (node.Value != null)
            {
              var text = ValueConventions.CreateQuotedText(node.Value, Settings);
              writer.Write(text);
            }
            break;
          }
      }
    }

    protected override void DoWriteComplete()
    {
      // nada a fazer
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
