using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class FlatTableTransformReader : TransformReader
  {
    public FlatTableTransformReader(Reader reader)
      : base(reader, new FlatTableTransform())
    {
    }

    public FlatTableTransformReader(Reader reader, Func<string, bool> fieldFilter)
      : base(reader, new FlatTableTransform(fieldFilter))
    {
    }

    public FlatTableTransformReader(Reader reader, string[] fields)
      : base(reader, new FlatTableTransform(fields))
    {
    }

    public FlatTableTransformReader(Reader reader, SerializationSettings settings)
      : base(reader, new FlatTableTransform(), settings)
    {
    }

    public FlatTableTransformReader(Reader reader, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(reader, new FlatTableTransform(fieldFilter), settings)
    {
    }

    public FlatTableTransformReader(Reader reader, SerializationSettings settings, string[] fields)
      : base(reader, new FlatTableTransform(fields), settings)
    {
    }

    public FlatTableTransformReader(Reader reader, MatrixSettings settings)
      : base(reader, new FlatTableTransform(), settings)
    {
    }

    public FlatTableTransformReader(Reader reader, MatrixSettings settings, Func<string, bool> fieldFilter)
      : base(reader, new FlatTableTransform(fieldFilter), settings)
    {
    }

    public FlatTableTransformReader(Reader reader, MatrixSettings settings, string[] fields)
      : base(reader, new FlatTableTransform(fields), settings)
    {
    }
  }
}