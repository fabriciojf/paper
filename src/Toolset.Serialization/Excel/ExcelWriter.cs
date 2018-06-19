using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Toolset.Serialization.Transformations;

namespace Toolset.Serialization.Excel
{
  public class ExcelWriter : Writer
  {
    internal const TextCase DefaultTextCase = TextCase.KeepOriginal;

    private readonly Writer writer;

    #region Construtores extras ...

    #region Construtores Stream ...

    public ExcelWriter(Stream textStream)
      : this(textStream, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public ExcelWriter(Stream textStream, Func<string, bool> fieldFilter)
      : this(textStream, (SerializationSettings)null, fieldFilter)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public ExcelWriter(Stream textStream, string[] fields)
      : this(textStream, (SerializationSettings)null, fields)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    #endregion

    #region Construtores string ...

    public ExcelWriter(string filename)
      : this(new FileStream(filename, FileMode.Create), (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public ExcelWriter(string filename, Func<string, bool> fieldFilter)
      : this(new FileStream(filename, FileMode.Create), (SerializationSettings)null, fieldFilter)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public ExcelWriter(string filename, string[] fields)
      : this(new FileStream(filename, FileMode.Create), (SerializationSettings)null, fields)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public ExcelWriter(string filename, SerializationSettings settings)
      : this(new FileStream(filename, FileMode.Create), settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public ExcelWriter(string filename, SerializationSettings settings, Func<string, bool> fieldFilter)
      : this(new FileStream(filename, FileMode.Create), settings, fieldFilter)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public ExcelWriter(string filename, SerializationSettings settings, string[] fields)
      : this(new FileStream(filename, FileMode.Create), settings, fields)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    #endregion

    #endregion

    public ExcelWriter(Stream streamWriter, SerializationSettings settings)
      : base(settings)
    {
      var basicExcelWriter = new BasicExcelWriter(streamWriter, base.Settings);
      writer = new FlatMatrixTransformWriter(basicExcelWriter, base.Settings);
    }

    public ExcelWriter(Stream streamWriter, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(settings)
    {
      var basicExcelWriter = new BasicExcelWriter(streamWriter, base.Settings);
      writer = new FlatMatrixTransformWriter(basicExcelWriter, base.Settings, fieldFilter);
    }

    public ExcelWriter(Stream streamWriter, SerializationSettings settings, string[] fields)
      : base(settings)
    {
      var basicExcelWriter = new BasicExcelWriter(streamWriter, base.Settings);
      writer = new FlatMatrixTransformWriter(basicExcelWriter, base.Settings, fields);
    }

    public new ExcelSerializationSettings Settings
    {
      get { return base.Settings.As<ExcelSerializationSettings>(); }
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

  }
}
