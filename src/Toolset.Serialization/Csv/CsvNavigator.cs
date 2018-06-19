using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Toolset.Serialization.Transformations;

namespace Toolset.Serialization.Csv
{
  public class CsvNavigator : TableNavigator
  {
    public CsvNavigator(Reader reader)
      : base(reader)
    {
    }

    public CsvNavigator(CsvReader reader)
      : base(reader)
    {
    }

    #region Construtores TextReader ...

    public CsvNavigator(TextReader textReader)
      : base(new CsvReader(textReader))
    {
    }

    public CsvNavigator(TextReader textReader, Func<string, bool> fieldFilter)
      : base(new CsvReader(textReader, fieldFilter))
    {
    }

    public CsvNavigator(TextReader textReader, string[] fields)
      : base(new CsvReader(textReader, fields))
    {
    }

    public CsvNavigator(TextReader textReader, SerializationSettings settings)
      : base(new CsvReader(textReader, settings))
    {
    }

    public CsvNavigator(TextReader textReader, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(new CsvReader(textReader, settings, fieldFilter))
    {
    }

    public CsvNavigator(TextReader textReader, SerializationSettings settings, string[] fields)
      : base(new CsvReader(textReader, settings, fields))
    {
    }

    #endregion

    #region Construtores Stream ...

    public CsvNavigator(Stream textStream)
      : base(new CsvReader(textStream))
    {
    }

    public CsvNavigator(Stream textStream, Func<string, bool> fieldFilter)
      : base(new CsvReader(textStream, fieldFilter))
    {
    }

    public CsvNavigator(Stream textStream, string[] fields)
      : base(new CsvReader(textStream, fields))
    {
    }

    public CsvNavigator(Stream textStream, SerializationSettings settings)
      : base(new CsvReader(textStream, settings))
    {
    }

    public CsvNavigator(Stream textStream, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(new CsvReader(textStream, settings, fieldFilter))
    {
    }

    public CsvNavigator(Stream textStream, SerializationSettings settings, string[] fields)
      : base(new CsvReader(textStream, settings,fields))
    {
    }

    #endregion

    #region Construtores string ...

    public CsvNavigator(string text)
      : base(new CsvReader(text))
    {
    }

    public CsvNavigator(string text, Func<string, bool> fieldFilter)
      : base(new CsvReader(text, fieldFilter))
    {
    }

    public CsvNavigator(string text, string[] fields)
      : base(new CsvReader(text, fields))
    {
    }

    public CsvNavigator(string text, SerializationSettings settings)
      : base(new CsvReader(text, settings))
    {
    }

    public CsvNavigator(string text, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(new CsvReader(text, settings, fieldFilter))
    {
    }

    public CsvNavigator(string text, SerializationSettings settings, string[] fields)
      : base(new CsvReader(text, settings, fields))
    {
    }

    #endregion
  }
}
