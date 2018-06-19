using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class FlatMatrixTransformReader : TransformReader
  {
    public FlatMatrixTransformReader(Reader reader)
      : base(reader, new FlatMatrixTransform())
    {
    }

    public FlatMatrixTransformReader(Reader reader, Func<string, bool> fieldFilter)
      : base(reader, new FlatMatrixTransform(fieldFilter))
    {
    }

    public FlatMatrixTransformReader(Reader reader, string[] fields)
      : base(reader, new FlatMatrixTransform(fields))
    {
    }

    public FlatMatrixTransformReader(Reader reader, SerializationSettings settings)
      : base(reader, new FlatMatrixTransform(), settings)
    {
    }

    public FlatMatrixTransformReader(Reader reader, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(reader, new FlatMatrixTransform(fieldFilter), settings)
    {
    }

    public FlatMatrixTransformReader(Reader reader, SerializationSettings settings, string[] fields)
      : base(reader, new FlatMatrixTransform(fields), settings)
    {
    }

    public FlatMatrixTransformReader(Reader reader, MatrixSettings settings)
      : base(reader, new FlatMatrixTransform(), settings)
    {
    }

    public FlatMatrixTransformReader(Reader reader, MatrixSettings settings, Func<string, bool> fieldFilter)
      : base(reader, new FlatMatrixTransform(fieldFilter), settings)
    {
    }

    public FlatMatrixTransformReader(Reader reader, MatrixSettings settings, string[] fields)
      : base(reader, new FlatMatrixTransform(fields), settings)
    {
    }
  }
}