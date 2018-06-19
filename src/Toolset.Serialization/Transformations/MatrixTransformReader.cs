using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class MatrixTransformReader : TransformReader
  {
    public MatrixTransformReader(Reader reader)
      : base(reader, new MatrixTransform())
    {
    }

    public MatrixTransformReader(Reader reader, SerializationSettings settings)
      : base(reader, new MatrixTransform(), settings)
    {
    }
  }
}