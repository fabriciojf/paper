using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Sequel
{
  public class NoConnectionException : SequelException
  {
    public NoConnectionException(string message)
      : base(message)
    {
    }

    public NoConnectionException(string message, Exception cause)
      : base(message, cause)
    {
    }
  }
}
