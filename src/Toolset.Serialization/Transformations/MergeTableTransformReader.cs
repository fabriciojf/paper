using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class MergeTableTransformReader : TransformReader
  {
    public MergeTableTransformReader(Reader reader)
      : base(reader, new MergeTableTransform())
    {
    }

    public MergeTableTransformReader(Reader reader, SerializationSettings settings)
      : base(reader, new MergeTableTransform(), settings)
    {
    }
  }
}