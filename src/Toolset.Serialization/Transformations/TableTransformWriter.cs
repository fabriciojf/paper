using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class TableTransformWriter : TransformWriter
  {
    public TableTransformWriter(Writer writer)
      : base(writer, new TableTransform())
    {
    }

    public TableTransformWriter(Writer writer, SerializationSettings settings)
      : base(writer, new TableTransform(), settings)
    {
    }
  }
}