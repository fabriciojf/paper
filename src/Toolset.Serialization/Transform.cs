using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization
{
  public static class Transform
  {
    public static readonly ITransform Null = new NullTransform();

    private class NullTransform : ITransform
    {
      public SerializationSettings Settings
      {
        get;
        set;
      }

      public IEnumerable<Node> TransformNode(Node node)
      {
        yield return node;
      }

      public IEnumerable<Node> Complete()
      {
        return Enumerable.Empty<Node>();
      }
    }
  }
}