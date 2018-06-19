using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class MatrixTransformWriter : TransformWriter
  {
    public MatrixTransformWriter(Writer writer)
      : base(writer, new MatrixTransform())
    {
    }

    public MatrixTransformWriter(Writer writer, SerializationSettings settings)
      : base(writer, new MatrixTransform(), settings)
    {
    }
  }
}