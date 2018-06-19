using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Toolset.Serialization.Transformations;

namespace Toolset.Serialization
{
  public sealed class TraceTransform : ITransform
  {
    private readonly Action<string> printer;
    private readonly TextWriter writer;
    
    private int depth;

    public TraceTransform()
    {
      this.printer = text => Trace.Write(text);
    }

    public TraceTransform(Stream stream)
    {
      this.writer = new StreamWriter(stream) { AutoFlush = true };
      this.printer = text => this.writer.Write(text);
    }

    public TraceTransform(TextWriter writer)
    {
      this.writer = writer;
      this.printer = text => this.writer.Write(text);
    }

    public SerializationSettings Settings
    {
      get;
      set;
    }

    public IEnumerable<Node> TransformNode(Node node)
    {
      if (node.Type.HasFlag(NodeType.Start))
      {
        Indent();
        PrintNode(node);
        PrintLine();
        depth++;
      }
      else if (node.Type.HasFlag(NodeType.End))
      {
        depth--;
        Indent();
        PrintText(NodeType.End);
        PrintLine();
      }
      else
      {
        Indent();
        PrintNode(node);
        PrintLine();
      }
      yield return node;
    }

    public IEnumerable<Node> Complete()
    {
      return Enumerable.Empty<Node>();
    }

    private void Indent()
    {
      for (int i = 0; i < depth; i++)
      {
        printer.Invoke("  ");
      }
    }

    private void PrintNode(Node node)
    {
      printer.Invoke(node.RawType.ToString());
      if (node.Type == NodeType.Value)
      {
        if (node.Value != null)
        {
          printer.Invoke(" ");
          printer.Invoke(node.Value.GetType().Name);
          printer.Invoke(" ");
          printer.Invoke(node.Value.ToString());
        }
        else if (node.Type == NodeType.Value)
        {
          printer.Invoke(" Null");
        }
      }
      else
      {
        if (node.Value != null)
        {
          printer.Invoke(" ");
          printer.Invoke(node.Value.ToString());
        }
      }
    }

    private void PrintText(object text)
    {
      if (text != null)
        printer.Invoke(text.ToString());
    }

    private void PrintLine()
    {
      printer.Invoke("\n");
    }
  }
}
