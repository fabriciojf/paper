using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class ForkTransformReader : TransformReader
  {
    public ForkTransformReader(Reader reader, IEnumerable<Writer> writers)
      : base(reader, new ForkTransform(writers))
    {
    }

    public ForkTransformReader(Reader reader, Writer writer, params Writer[] others)
      : base(reader, new ForkTransform(writer, others))
    {
    }
  }
}