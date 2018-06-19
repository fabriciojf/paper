using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Toolset.Serialization.Transformations;

namespace Toolset.Serialization
{
  public sealed class TraceWriter : TransformWriter
  {
    public TraceWriter()
      : base(Writer.Null, new TraceTransform())
    {
    }

    public TraceWriter(Stream stream)
      : base(Writer.Null, new TraceTransform(stream))
    {
    }

    public TraceWriter(TextWriter textWriter)
      : base(Writer.Null, new TraceTransform(textWriter))
    {
    }

    public TraceWriter(Writer writer)
      : base(writer, new TraceTransform())
    {
    }

    public TraceWriter(Writer writer, Stream stream)
      : base(writer, new TraceTransform(stream))
    {
    }

    public TraceWriter(Writer writer, TextWriter textWriter)
      : base(writer, new TraceTransform(textWriter))
    {
    }
  }
}