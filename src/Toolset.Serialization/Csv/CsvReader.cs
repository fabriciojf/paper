using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Toolset.Serialization.Transformations;

namespace Toolset.Serialization.Csv
{
  public sealed class CsvReader : Reader
  {
    private Reader reader;
    private bool keepOpen;

    #region Construtores ...

    #region Construtores TextReader ...

    public CsvReader(TextReader textReader)
    {
      Initialize(textReader, true);
    }

    public CsvReader(TextReader textReader, Func<string, bool> fieldFilter)
    {
      Initialize(textReader, true, fieldFilter);
    }

    public CsvReader(TextReader textReader, string[] fields)
    {
      Initialize(textReader, true, fields);
    }

    public CsvReader(TextReader textReader, SerializationSettings settings)
      : base(settings)
    {
      Initialize(textReader, true);
    }

    public CsvReader(TextReader textReader, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(settings)
    {
      Initialize(textReader, true, fieldFilter);
    }

    public CsvReader(TextReader textReader, SerializationSettings settings, string[] fields)
      : base(settings)
    {
      Initialize(textReader, true, fields);
    }

    #endregion

    #region Construtores Stream ...

    public CsvReader(Stream textStream)
    {
      var reader = new StreamReader(textStream, Encoding.UTF8, false, 1024, true);
      Initialize(reader, true);
    }

    public CsvReader(Stream textStream, Func<string, bool> fieldFilter)
    {
      var reader = new StreamReader(textStream, Encoding.UTF8, false, 1024, true);
      Initialize(reader, true, fieldFilter);
    }

    public CsvReader(Stream textStream, string[] fields)
    {
      var reader = new StreamReader(textStream, Encoding.UTF8, false, 1024, true);
      Initialize(reader, true, fields);
    }

    public CsvReader(Stream textStream, SerializationSettings settings)
      : base(settings)
    {
      var reader = new StreamReader(textStream, Encoding.UTF8, false, 1024, true);
      Initialize(reader, true);
    }

    public CsvReader(Stream textStream, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(settings)
    {
      var reader = new StreamReader(textStream, Encoding.UTF8, false, 1024, true);
      Initialize(reader, true, fieldFilter);
    }

    public CsvReader(Stream textStream, SerializationSettings settings, string[] fields)
      : base(settings)
    {
      var reader = new StreamReader(textStream, Encoding.UTF8, false, 1024, true);
      Initialize(reader, true, fields);
    }

    #endregion

    #region Construtores Arquivo ...

    public CsvReader(string filename)
    {
      Initialize(new StreamReader(filename), false);
    }

    public CsvReader(string filename, Func<string, bool> fieldFilter)
    {
      Initialize(new StreamReader(filename), false, fieldFilter);
    }

    public CsvReader(string filename, string[] fields)
    {
      Initialize(new StreamReader(filename), false, fields);
    }

    public CsvReader(string filename, SerializationSettings settings)
      : base(settings)
    {
      Initialize(new StreamReader(filename), false);
    }

    public CsvReader(string filename, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(settings)
    {
      Initialize(new StreamReader(filename), false, fieldFilter);
    }

    public CsvReader(string filename, SerializationSettings settings, string[] fields)
      : base(settings)
    {
      Initialize(new StreamReader(filename), false, fields);
    }

    #endregion

    #endregion
    
    private void Initialize(TextReader textReader, bool keepOpen)
    {
      var csvReader = new BasicCsvReader(textReader, base.Settings);
      this.reader = new FlatTableTransformReader(csvReader, base.Settings);
      this.keepOpen = keepOpen;
    }

    private void Initialize(TextReader textReader, bool keepOpen, Func<string, bool> fieldFilter)
    {
      var csvReader = new BasicCsvReader(textReader, base.Settings);
      this.reader = new FlatTableTransformReader(csvReader, base.Settings, fieldFilter);
      this.keepOpen = keepOpen;
    }

    private void Initialize(TextReader textReader, bool keepOpen, string[] fields)
    {
      var csvReader = new BasicCsvReader(textReader, base.Settings);
      this.reader = new FlatTableTransformReader(csvReader, base.Settings, fields);
      this.keepOpen = keepOpen;
    }

    public override Node Current
    {
      get { return reader.Current; }
    }

    protected override bool DoRead()
    {
      return reader.Read();
    }

    public override void Close()
    {
      reader.Close();
    }

    public override void Dispose()
    {
      base.Dispose();
      if (!keepOpen && reader != null)
      {
        reader.Dispose();
      }
    }
  }
}