using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Toolset.Serialization.Graph;
using Toolset.Serialization.Json;

namespace Toolset.Serialization
{
  public class DocumentModel : NodeModel
  {
    #region Construtores...

    public DocumentModel()
    {
    }

    public DocumentModel(ObjectModel root)
    {
      this.Root = root;
    }

    public DocumentModel(CollectionModel root)
    {
      this.Root = root;
    }

    #endregion

    public NodeModel Root
    {
      get { return root ?? (root = new ObjectModel()); }
      set { root = value; }
    }
    private NodeModel root;

    public override NodeModel Parent
    {
      get;
      internal set;
    }

    public override NodeType SerializationType
    {
      get { return NodeType.Document; }
    }

    public override object SerializationValue
    {
      get;
      set;
    }

    public override IEnumerable<NodeModel> Children()
    {
      return new[] { Root };
    }

    public DocumentReader CreateReader()
    {
      return new DocumentReader(this, null);
    }

    public static DocumentModel Read(Reader reader)
    {
      using (var writer = new DocumentWriter(null))
      {
        reader.CopyTo(writer);
        return writer.TargetDocument;
      }
    }
    
    public void Write(Writer writer)
    {
      if (Root != null)
      {
        using (var reader = CreateReader())
        {
          reader.CopyTo(writer);
        }
      }
    }

    public override string ToString()
    {
      try
      {
        var buffer = new StringWriter();
        using (var reader = CreateReader())
        {
          using (var writer = new JsonWriter(buffer, new JsonSerializationSettings { Indent = true }))
          {
            reader.CopyTo(writer);
          }
        }
        return buffer.ToString();
      }
      catch
      {
        return base.ToString();
      }
    }
      
  }
}
