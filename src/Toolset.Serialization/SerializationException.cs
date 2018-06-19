using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization
{
  public class SerializationException : Exception
  {
    public SerializationException()
    {
    }

    public SerializationException(string message)
      : base(message)
    {
    }

    public SerializationException(string message, Exception cause)
      : base(message, cause)
    {
    }
  }
}
