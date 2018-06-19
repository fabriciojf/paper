using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Toolset.Serialization.Csv;
using Toolset.Serialization.Json;
using Toolset.Serialization.Transformations;
using Toolset.Serialization.Xml;

namespace Toolset.Serialization
{
  public abstract class Reader : IDisposable
  {
    public static readonly Reader Null = new NullReader();

    protected abstract bool DoRead();
    public abstract void Close();
    public abstract Node Current { get; }

    private Validator validator;
    private Func<bool> readMethod;

    public Reader()
    {
      this.Settings = new SerializationSettings();
      this.readMethod = DoRead;
    }

    public Reader(SerializationSettings settings)
    {
      this.Settings = settings ?? new SerializationSettings();
      this.readMethod = DoRead;
    }

    public SerializationSettings Settings
    {
      get;
      private set;
    }

    public virtual bool IsValid
    {
      get { return readMethod == DoValidRead; }
      set
      {
        if (value)
        {
          if (validator == null)
            validator = new Validator();
          readMethod = DoValidRead;
        }
        else
        {
          readMethod = DoRead;
        }
      }
    }

    public NodeType NodeType
    {
      get { return (Current != null) ? Current.Type : NodeType.Unknown; }
    }

    public object NodeValue
    {
      get { return (Current != null) ? Current.Value : null; }
    }

    public bool Read()
    {
      return readMethod.Invoke();
    }

    private bool DoValidRead()
    {
      var ready = this.DoRead();
      if (ready)
      {
        validator.AcceptNode(this.Current);
      }
      return ready;
    }

    public Node ReadNode()
    {
      var ready = Read();
      return ready ? Current : null;
    }

    public Node ReadNode(NodeType type)
    {
      var ready = Read();
      if (!ready)
        return null;
      if (NodeType != type)
        throw new SerializationException("Era esperado um token " + type + " mas foi detectado: " + Current);
      return Current;
    }

    public Node ReadDocumentStart() { return ReadNode(NodeType.DocumentStart); }
    public Node ReadDocumentEnd() { return ReadNode(NodeType.DocumentEnd); }
    public Node ReadObjectStart() { return ReadNode(NodeType.ObjectStart); }
    public Node ReadObjectEnd() { return ReadNode(NodeType.ObjectEnd); }
    public Node ReadCollectionStart() { return ReadNode(NodeType.CollectionStart); }
    public Node ReadCollectionEnd() { return ReadNode(NodeType.CollectionEnd); }
    public Node ReadPropertyStart() { return ReadNode(NodeType.PropertyStart); }
    public Node ReadPropertyEnd() { return ReadNode(NodeType.PropertyEnd); }
    public Node ReadValueNode() { return ReadNode(NodeType.Value); }

    public object ReadValue()
    {
      var value = ReadValueNode();
      return value.Value;
    }
    
    public virtual void Dispose()
    {
      Close();
    }

    public virtual void CopyTo(Writer writer)
    {
      while (Read())
      {
        writer.Write(Current);
      }
      writer.WriteComplete();
    }

    #region CreateReader
    
    public static Reader CreateReader(TextReader reader, SerializationSettings settings)
    {
      var documentReader = SupportedDocumentTextReader.Create(reader);
      
      if (documentReader.DocumentFormat == SupportedDocumentTextReader.XmlFormat)
        return new XmlDocumentReader(documentReader, settings);
      
      if (documentReader.DocumentFormat == SupportedDocumentTextReader.JsonFormat)
        return new JsonReader(documentReader, settings);

      return new CsvReader(documentReader, settings);
    }

    #region Fábricas extras...

    public static Reader CreateReader(TextReader reader)
    {
      return CreateReader(reader, new SerializationSettings());
    }
    
    public static Reader CreateReader(string filename, SerializationSettings settings)
    {
      var reader = new StreamReader(filename);
      return CreateReader(reader, settings);
    }

    public static Reader CreateReader(string filename)
    {
      return CreateReader(filename, new SerializationSettings());
    }

    #endregion

    #endregion

    private class NullReader : Reader
    {
      public NullReader()
        : base(null)
      {
      }

      public override Node Current
      {
        get { return null; }
      }

      protected override bool DoRead()
      {
        return false;
      }

      public override void Close()
      {
        // nada a fazer
      }
    }

  }

}
