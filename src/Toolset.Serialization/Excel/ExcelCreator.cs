using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Toolset.Serialization.Transformations;

namespace Toolset.Serialization.Excel
{
  public class ExcelCreator : TableCreator
  {
    public ExcelCreator(Writer writer)
      : base(writer)
    {
    }

    public ExcelCreator(ExcelWriter writer)
      : base(writer)
    {
    }

    #region Construtores Stream ...

    public ExcelCreator(Stream textStream)
      : base(new ExcelWriter(textStream))
    {
    }

    public ExcelCreator(Stream textStream, Func<string, bool> fieldFilter)
      : base(new ExcelWriter(textStream, fieldFilter))
    {
    }

    public ExcelCreator(Stream textStream, string[] fields)
      : base(new ExcelWriter(textStream, fields))
    {
    }

    public ExcelCreator(Stream textStream, SerializationSettings settings)
      : base(new ExcelWriter(textStream, settings))
    {
    }

    public ExcelCreator(Stream textStream, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(new ExcelWriter(textStream, settings, fieldFilter))
    {
    }

    public ExcelCreator(Stream textStream, SerializationSettings settings, string[] fields)
      : base(new ExcelWriter(textStream, settings, fields))
    {
    }

    #endregion

    #region Construtores string ...

    public ExcelCreator(string filename)
      : base(new ExcelWriter(filename))
    {
    }

    public ExcelCreator(string filename, Func<string, bool> fieldFilter)
      : base(new ExcelWriter(filename, fieldFilter))
    {
    }

    public ExcelCreator(string filename, string[] fields)
      : base(new ExcelWriter(filename, fields))
    {
    }

    public ExcelCreator(string filename, SerializationSettings settings)
      : base(new ExcelWriter(filename, settings))
    {
    }

    public ExcelCreator(string filename, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(new ExcelWriter(filename, settings, fieldFilter))
    {
    }

    public ExcelCreator(string filename, SerializationSettings settings, string[] fields)
      : base(new ExcelWriter(filename, settings, fields))
    {
    }

    #endregion

  }
}
