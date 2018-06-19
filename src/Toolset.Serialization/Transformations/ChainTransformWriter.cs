using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public class ChainTransformWriter : TransformWriter
  {
    public ChainTransformWriter(Writer writer, IEnumerable<ITransform> transforms)
      : base(writer, new ChainTransform(transforms))
    {
    }

    public ChainTransformWriter(Writer writer, ITransform transform, params ITransform[] others)
      : base(writer, new ChainTransform(transform, others))
    {
    }
  }
}
