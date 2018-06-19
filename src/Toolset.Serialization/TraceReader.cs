using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Toolset.Serialization.Transformations;

namespace Toolset.Serialization
{
  public sealed class TraceReader : TransformReader
  {
    public TraceReader(Reader reader)
      : base(reader, new TraceTransform())
    {
    }

    public TraceReader(Reader reader, Stream stream)
      : base(reader, new TraceTransform(stream))
    {
    }

    public TraceReader(Reader reader, TextWriter writer)
      : base(reader, new TraceTransform(writer))
    {
    }
  }
}
