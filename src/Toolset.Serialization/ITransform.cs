using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization
{
  public interface ITransform
  {
    SerializationSettings Settings { get; set; }

    IEnumerable<Node> TransformNode(Node node);

    IEnumerable<Node> Complete();
  }
}
