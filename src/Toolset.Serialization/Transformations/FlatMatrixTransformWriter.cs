using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class FlatMatrixTransformWriter : TransformWriter
  {
    public FlatMatrixTransformWriter(Writer writer)
      : base(writer, new FlatMatrixTransform())
    {
    }

    public FlatMatrixTransformWriter(Writer writer, Func<string, bool> fieldFilter)
      : base(writer, new FlatMatrixTransform(fieldFilter))
    {
    }

    public FlatMatrixTransformWriter(Writer writer, string[] fields)
      : base(writer, new FlatMatrixTransform(fields))
    {
    }

    public FlatMatrixTransformWriter(Writer writer, SerializationSettings settings)
      : base(writer, new FlatMatrixTransform(), settings)
    {
    }

    public FlatMatrixTransformWriter(Writer writer, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(writer, new FlatMatrixTransform(fieldFilter), settings)
    {
    }

    public FlatMatrixTransformWriter(Writer writer, SerializationSettings settings, string[] fields)
      : base(writer, new FlatMatrixTransform(fields), settings)
    {
    }
  }
}