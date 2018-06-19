using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.IO;
using Toolset.Serialization.Xml;
using Toolset.Serialization.Json;
using Toolset.Serialization.Csv;

namespace Toolset.Serialization
{
  public abstract class Writer : IDisposable
  {
    public static readonly Writer Null = new NullWriter();

    protected abstract void DoWrite(Node node);
    protected abstract void DoWriteComplete();
    protected abstract void DoClose();
    protected abstract void DoFlush();

    private bool complete;
    private Validator validator;
    private Action<Node> forward;

    public Writer()
    {
      this.Settings = new SerializationSettings();
      this.forward = DoWrite;
    }

    public Writer(SerializationSettings settings)
    {
      this.Settings = settings ?? new SerializationSettings();
      this.forward = DoWrite;
    }

    public SerializationSettings Settings
    {
      get;
      private set;
    }

    public virtual bool IsValid
    {
      get { return forward == DoValidWrite; }
      set
      {
        if (value)
        {
          if (validator == null)
            validator = new Validator();
          forward = DoValidWrite;
        }
        else
        {
          forward = DoWrite;
        }
      }
    }

    public void Write(Node node)
    {
      forward.Invoke(node);
    }

    private void DoValidWrite(Node node)
    {
      validator.AcceptNode(node);
      DoWrite(node);
    }

    #region Escritas extras...

    public void Write(NodeType type)
    {
      Write(new Node { Type = type });
    }

    public void Write(NodeType type, object value)
    {
      Write(new Node { Type = type, Value = value });
    }

    public void WriteDocumentStart(string documentName)
    {
      Write(new Node { Type = NodeType.DocumentStart, Value = documentName });
    }

    public void WriteDocumentStart()
    {
      Write(new Node { Type = NodeType.DocumentStart });
    }

    public void WriteDocumentEnd()
    {
      Write(new Node { Type = NodeType.DocumentEnd });
    }

    public void WriteObjectStart(string objectName)
    {
      Write(new Node { Type = NodeType.ObjectStart, Value = objectName });
    }

    public void WriteObjectStart()
    {
      Write(new Node { Type = NodeType.ObjectStart });
    }

    public void WriteObjectEnd()
    {
      Write(new Node { Type = NodeType.ObjectEnd });
    }

    public void WriteCollectionStart()
    {
      Write(new Node { Type = NodeType.CollectionStart });
    }

    public void WriteCollectionStart(string collectionName)
    {
      Write(new Node { Type = NodeType.CollectionStart, Value = collectionName });
    }

    public void WriteCollectionEnd()
    {
      Write(new Node { Type = NodeType.CollectionEnd });
    }

    public void WritePropertyStart(string propertyName)
    {
      Write(new Node { Type = NodeType.PropertyStart, Value = propertyName });
    }

    public void WritePropertyEnd()
    {
      Write(new Node { Type = NodeType.PropertyEnd });
    }

    public void WriteProperty(string propertyName, object value)
    {
      Write(new Node { Type = NodeType.PropertyStart, Value = propertyName });
      Write(new Node { Type = NodeType.Value, Value = value });
      Write(new Node { Type = NodeType.PropertyEnd });
    }

    public void WriteValue(object value)
    {
      Write(new Node { Type = NodeType.Value, Value = value });
    }

    public void WriteComplete()
    {
      if (!complete)
      {
        complete = true;
        DoWriteComplete();
      }
    }

    public virtual void Flush()
    {
      DoFlush();
    }

    public virtual void Close()
    {
      WriteComplete();
      Flush();
      DoClose();
    }

    public virtual void Dispose()
    {
      Close();
    }

    #endregion

    private class NullWriter : Writer
    {
      public NullWriter()
        : base(null)
      {
      }

      protected override void DoWrite(Node node)
      {
        // nada a fazer
      }

      protected override void DoWriteComplete()
      {
        // nada a fazer
      }

      protected override void DoFlush()
      {
        // nada a fazer
      }

      protected override void DoClose()
      {
        // nada a fazer
      }
    }

  }

}
