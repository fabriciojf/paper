using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Toolset.Serialization.Json
{
  public sealed class JsonWriter : Writer
  {
    private readonly TextWriter writer;

    private readonly Stack<Children> stack;
    
    #region Construtores extras ...

    public JsonWriter(TextWriter textWriter)
      : this(textWriter, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }
    
    public JsonWriter(Stream textStream)
      : this(new StreamWriter(textStream), (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }
    
    public JsonWriter(Stream textStream, SerializationSettings settings)
      : this(new StreamWriter(textStream), settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public JsonWriter(string filename)
      : this(File.OpenWrite(filename), (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public JsonWriter(string filename, SerializationSettings settings)
      : this(File.OpenWrite(filename), settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }
    
    #endregion

    public JsonWriter(TextWriter writer, SerializationSettings settings)
      : base(settings)
    {
      this.writer = writer;
      this.stack = new Stack<Children>();
      base.IsValid = true;
    }

    public new JsonSerializationSettings Settings
    {
      get { return base.Settings.As<JsonSerializationSettings>(); }
    }

    private void Indent()
    {
      if (Settings.Indent)
      {
        writer.WriteLine();

        var depth = stack.Count;
        while (depth-- > 0)
        {
          writer.Write(Settings.IndentChars);
        }
      }
    }

    protected override void DoWrite(Node node)
    {
      switch (node.Type)
      {
        case NodeType.DocumentStart:
          {
            writer.Write("{");
            stack.Push(new Children { Parent = NodeType.Document });
            break;
          }

        case NodeType.DocumentEnd:
          {
            stack.Pop();
            Indent();
            writer.Write("}");
            break;
          }

        case NodeType.ObjectStart:
          {
            if (stack.Any())
            {
              var children = stack.Peek();
              if (children.Parent == NodeType.Document)
              {
                EmitProperty((node.Value ?? "root").ToString());
              }
              else if (children.Parent == NodeType.Collection)
              {
                if (children.Count > 0)
                {
                  writer.Write(",");
                }
                children.Count++;

                Indent();
              }
            }

            writer.Write("{");
            stack.Push(new Children { Parent = NodeType.Object });
            return;
          }

        case NodeType.ObjectEnd:
          {
            var children = stack.Pop();
            if (children.Count > 0)
            {
              Indent();
            }
            writer.Write("}");
            return;
          }

        case NodeType.CollectionStart:
          {
            if (stack.Any())
            {
              var children = stack.Peek();
              if (children.Parent == NodeType.Document)
              {
                EmitProperty((node.Value ?? "root").ToString());
              }
              else if (children.Parent == NodeType.Collection)
              {
                if (children.Count > 0)
                {
                  writer.Write(",");
                }
                children.Count++;

                Indent();
              }
            }

            writer.Write("[");
            stack.Push(new Children { Parent = NodeType.Collection });
            return;
          }

        case NodeType.CollectionEnd:
          {
            var children = stack.Pop();
            if (children.Count > 0)
            {
              Indent();
            }
            writer.Write("]");
            return;
          }

        case NodeType.PropertyStart:
          {
            var children = stack.Peek();
            if (children.Count > 0)
            {
              writer.Write(",");
            }
            children.Count++;

            EmitProperty((node.Value ?? "property").ToString());

            return;
          }

        case NodeType.PropertyEnd:
          {
            return;
          }

        case NodeType.Value:
          {
            var children = stack.Peek();
            if (children.Parent == NodeType.Collection)
            {
              if (children.Count > 0)
              {
                writer.Write(",");
              }
              children.Count++;

              Indent();
            }

            var text = ValueConventions.CreateQuotedText(node.Value, Settings, Toolset.Json.Escape);
            writer.Write(text);

            return;
          }
      }
    }

    private void EmitProperty(string name)
    {
      var propertyName = ValueConventions.CreateName(name, Settings, TextCase.CamelCase);

      Indent();
      writer.Write("\"");
      writer.Write(propertyName);
      writer.Write("\":");
      if (Settings.Indent)
      {
        writer.Write(" ");
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

    private class Children
    {
      public NodeType Parent { get; set; }
      public int Count { get; set; }
    }

  }
}
