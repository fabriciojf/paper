using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset;
using System.Diagnostics;

namespace Toolset.Text.Template
{
  static class ExceptionExtensions
  {
    public static void Report(this Exception ex, string message)
    {
      if (ex != null)
        ex.TraceWarning("[TEXT-TEMPLATE]: " + message);
      else
        Trace.TraceWarning("[TEXT-TEMPLATE]: " + message);
    }
  }
}
