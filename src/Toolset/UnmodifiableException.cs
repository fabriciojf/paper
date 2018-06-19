using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolset
{
  public class UnmodifiableException : Exception
  {
    public UnmodifiableException(string message)
      : base(message)
    {
    }

    public UnmodifiableException(string message, Exception cause)
      : base(message, cause)
    {
    }
  }
}
