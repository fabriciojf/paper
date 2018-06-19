using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Sequel
{
  public class SequelException : Exception
  {
    public SequelException(string message)
      : base(message)
    {
    }

    public SequelException(string message, Exception cause)
      : base(message, cause)
    {
    }
  }
}
