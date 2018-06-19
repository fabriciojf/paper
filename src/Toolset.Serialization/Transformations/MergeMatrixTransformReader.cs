using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class MergeMatrixTransformReader : TransformReader
  {
    public MergeMatrixTransformReader(Reader reader)
      : base(reader, new MergeMatrixTransform())
    {
    }

    public MergeMatrixTransformReader(Reader reader, SerializationSettings settings)
      : base(reader, new MergeMatrixTransform(), settings)
    {
    }
  }
}