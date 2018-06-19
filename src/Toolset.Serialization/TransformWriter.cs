using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization
{
  public class TransformWriter : Writer
  {
    private readonly Writer writer;
    private readonly ITransform transform;

    public TransformWriter(Writer writer, ITransform transform)
      : base(writer.Settings)
    {
      this.writer = writer;
      this.transform = transform;
      this.transform.Settings = this.Settings;
    }

    public TransformWriter(Writer writer, ITransform transform, SerializationSettings settings)
      : base(settings)
    {
      this.writer = writer;
      this.transform = transform;
      this.transform.Settings = this.Settings;
    }

    protected override void DoWrite(Node node)
    {
      var emittedNodes = transform.TransformNode(node);
      foreach (var emittedNode in emittedNodes)
      {
        writer.Write(emittedNode);
      }
    }

    protected override void DoWriteComplete()
    {
      var nodes = this.transform.Complete();
      foreach (var node in nodes)
      {
        this.writer.Write(node);
      }
      writer.WriteComplete();
    }

    protected override void DoFlush()
    {
      writer.Flush();
    }

    protected override void DoClose()
    {
      this.writer.Close();
    }
  }
}
