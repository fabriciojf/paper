using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class FlatTableTransformWriter : TransformWriter
  {
    public FlatTableTransformWriter(Writer writer)
      : base(writer, new FlatTableTransform())
    {
    }

    public FlatTableTransformWriter(Writer writer, Func<string, bool> fieldFilter)
      : base(writer, new FlatTableTransform(fieldFilter))
    {
    }

    public FlatTableTransformWriter(Writer writer, string[] fields)
      : base(writer, new FlatTableTransform(fields))
    {
    }

    public FlatTableTransformWriter(Writer writer, SerializationSettings settings)
      : base(writer, new FlatTableTransform(), settings)
    {
    }

    public FlatTableTransformWriter(Writer writer, SerializationSettings settings, Func<string, bool> fieldFilter)
      : base(writer, new FlatTableTransform(fieldFilter), settings)
    {
    }

    public FlatTableTransformWriter(Writer writer, SerializationSettings settings, string[] fields)
      : base(writer, new FlatTableTransform(fields), settings)
    {
    }
  }
}