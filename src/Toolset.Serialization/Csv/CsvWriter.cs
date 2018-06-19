using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Toolset.Serialization.Transformations;

namespace Toolset.Serialization.Csv
{
  public class CsvWriter : Writer
  {
    private Writer writer;
    private bool keepOpen;

    #region Construtores ...

    #region Construtores TextWriter ...

    public CsvWriter(TextWriter textWriter)
    {
      Initialize(textWriter, true);
    }

    public CsvWriter(TextWriter textWriter, Func<string, bool> fieldFilter)
    {
      Initialize(textWriter, true, fieldFilter);
    }

    public CsvWriter(TextWriter textWriter, string[] fields)
    {
      Initialize(textWriter, true, fields);
    }

    public CsvWriter(TextWriter textWriter, SerializationSettings settings)
      : base(settings)
    {
      Initialize(textWriter, true);
    }

    public CsvWriter(TextWriter textWriter, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(settings)
    {
      Initialize(textWriter, true, fieldFilter);
    }

    public CsvWriter(TextWriter textWriter, SerializationSettings settings, string[] fields)
      : base(settings)
    {
      Initialize(textWriter, true, fields);
    }

    #endregion

    #region Construtores Stream ...

    public CsvWriter(Stream textStream)
    {
      var writer = new StreamWriter(textStream, Encoding.UTF8, 1024, true);
      Initialize(writer, true);
    }

    public CsvWriter(Stream textStream, Func<string, bool> fieldFilter)
    {
      var writer = new StreamWriter(textStream, Encoding.UTF8, 1024, true);
      Initialize(writer, true, fieldFilter);
    }

    public CsvWriter(Stream textStream, string[] fields)
    {
      var writer = new StreamWriter(textStream, Encoding.UTF8, 1024, true);
      Initialize(writer, true, fields);
    }

    public CsvWriter(Stream textStream, SerializationSettings settings)
      : base(settings)
    {
      var writer = new StreamWriter(textStream, Encoding.UTF8, 1024, true);
      Initialize(writer, true);
    }

    public CsvWriter(Stream textStream, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(settings)
    {
      var writer = new StreamWriter(textStream, Encoding.UTF8, 1024, true);
      Initialize(writer, true, fieldFilter);
    }

    public CsvWriter(Stream textStream, SerializationSettings settings, string[] fields)
      : base(settings)
    {
      var writer = new StreamWriter(textStream, Encoding.UTF8, 1024, true);
      Initialize(writer, true, fields);
    }

    #endregion

    #region Construtores Arquivo ...

    public CsvWriter(string filename)
    {
      Initialize(new StreamWriter(filename), false);
    }

    public CsvWriter(string filename, Func<string, bool> fieldFilter)
    {
      Initialize(new StreamWriter(filename), false, fieldFilter);
    }

    public CsvWriter(string filename, string[] fields)
    {
      Initialize(new StreamWriter(filename), false, fields);
    }

    public CsvWriter(string filename, SerializationSettings settings)
      : base(settings)
    {
      Initialize(new StreamWriter(filename), false);
    }

    public CsvWriter(string filename, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(settings)
    {
      Initialize(new StreamWriter(filename), false, fieldFilter);
    }

    public CsvWriter(string filename, SerializationSettings settings, string[] fields)
      : base(settings)
    {
      Initialize(new StreamWriter(filename), false, fields);
    }

    #endregion

    #endregion

    public void Initialize(TextWriter textWriter, bool keepOpen)
    {
      var basicCsvWriter = new BasicCsvWriter(textWriter, base.Settings);
      this.writer = new FlatMatrixTransformWriter(basicCsvWriter, base.Settings);
      this.keepOpen = keepOpen;
    }

    public void Initialize(TextWriter textWriter, bool keepOpen, Func<string, bool> fieldFilter)
    {
      var basicCsvWriter = new BasicCsvWriter(textWriter, base.Settings);
      this.writer = new FlatMatrixTransformWriter(basicCsvWriter, base.Settings, fieldFilter);
      this.keepOpen = keepOpen;
    }

    public void Initialize(TextWriter textWriter, bool keepOpen, string[] fields)
    {
      var basicCsvWriter = new BasicCsvWriter(textWriter, base.Settings);
      this.writer = new FlatMatrixTransformWriter(basicCsvWriter, base.Settings, fields);
      this.keepOpen = keepOpen;
    }

    protected override void DoWrite(Node node)
    {
      writer.Write(node);
    }

    protected override void DoWriteComplete()
    {
      writer.WriteComplete();
    }

    protected override void DoFlush()
    {
      writer.Flush();
    }

    protected override void DoClose()
    {
      writer.Close();
    }

    public override void Dispose()
    {
      base.Dispose();
      if (writer != null)
      {
        writer.Flush();
        if (!keepOpen)
        {
          writer.Dispose();
        }
      }
    }
  }
}
