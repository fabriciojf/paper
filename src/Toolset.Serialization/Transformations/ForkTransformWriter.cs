using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class ForkTransformWriter : TransformWriter
  {
    public ForkTransformWriter(IEnumerable<Writer> writers)
      : base(writers.First(), new ForkTransform(writers.Skip(1)))
    {
    }

    public ForkTransformWriter(Writer writer, params Writer[] others)
      : base(writer, new ForkTransform(others))
    {
    }
  }
}