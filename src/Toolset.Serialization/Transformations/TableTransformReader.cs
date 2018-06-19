using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public sealed class TableTransformReader : TransformReader
  {
    public TableTransformReader(Reader reader)
      : base(reader, new TableTransform())
    {
    }

    public TableTransformReader(Reader reader, SerializationSettings settings)
      : base(reader, new TableTransform(), settings)
    {
    }
  }
}