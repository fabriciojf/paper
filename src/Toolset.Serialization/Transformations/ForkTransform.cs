using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class ForkTransform : ITransform
  {
    private readonly Writer[] writers;

    public ForkTransform(IEnumerable<Writer> writers)
    {
      this.writers = writers.ToArray();
    }

    public ForkTransform(Writer writer, params Writer[] others)
    {
      this.writers = (new[] { writer }.Union(others)).ToArray();
    }

    public SerializationSettings Settings
    {
      get;
      set;
    }

    public IEnumerable<Node> TransformNode(Node node)
    {
      foreach (var writer in this.writers)
      {
        writer.Write(node);
      }
      yield return node;
    }

    public IEnumerable<Node> Complete()
    {
      return Enumerable.Empty<Node>();
    }
  }
}
