using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class MergeMatrixTransformWriter : TransformWriter
  {
    public MergeMatrixTransformWriter(Writer writer)
      : base(writer, new MergeMatrixTransform())
    {
    }

    public MergeMatrixTransformWriter(Writer writer, SerializationSettings settings)
      : base(writer, new MergeMatrixTransform(), settings)
    {
    }
  }
}