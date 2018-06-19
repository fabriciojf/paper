using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class MergeTableTransformWriter : TransformWriter
  {
    public MergeTableTransformWriter(Writer writer)
      : base(writer, new MergeTableTransform())
    {
    }

    public MergeTableTransformWriter(Writer writer, SerializationSettings settings)
      : base(writer, new MergeTableTransform(), settings)
    {
    }
  }
}