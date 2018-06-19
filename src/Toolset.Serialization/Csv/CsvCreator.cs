using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Toolset.Serialization.Transformations;

namespace Toolset.Serialization.Csv
{
  public class CsvCreator : TableCreator
  {
    public CsvCreator(Writer writer)
      : base(writer)
    {
    }

    public CsvCreator(CsvWriter writer)
      : base(writer)
    {
    }

    #region Construtores TextWriter ...

    public CsvCreator(TextWriter textWriter)
      : base(new CsvWriter(textWriter))
    {
    }

    public CsvCreator(TextWriter textWriter, Func<string, bool> fieldFilter)
      : base(new CsvWriter(textWriter, fieldFilter))
    {
    }

    public CsvCreator(TextWriter textWriter, string[] fields)
      : base(new CsvWriter(textWriter, fields))
    {
    }

    public CsvCreator(TextWriter textWriter, SerializationSettings settings)
      : base(new CsvWriter(textWriter, settings))
    {
    }

    public CsvCreator(TextWriter textWriter, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(new CsvWriter(textWriter, settings, fieldFilter))
    {
    }

    public CsvCreator(TextWriter textWriter, SerializationSettings settings, string[] fields)
      : base(new CsvWriter(textWriter, settings, fields))
    {
    }

    #endregion

    #region Construtores Stream ...

    public CsvCreator(Stream textStream)
      : base(new CsvWriter(textStream))
    {
    }

    public CsvCreator(Stream textStream, Func<string, bool> fieldFilter)
      : base(new CsvWriter(textStream, fieldFilter))
    {
    }

    public CsvCreator(Stream textStream, string[] fields)
      : base(new CsvWriter(textStream, fields))
    {
    }

    public CsvCreator(Stream textStream, SerializationSettings settings)
      : base(new CsvWriter(textStream, settings))
    {
    }

    public CsvCreator(Stream textStream, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(new CsvWriter(textStream, settings, fieldFilter))
    {
    }

    public CsvCreator(Stream textStream, SerializationSettings settings, string[] fields)
      : base(new CsvWriter(textStream, settings,fields))
    {
    }

    #endregion

  }
}
