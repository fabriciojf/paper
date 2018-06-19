using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Toolset.Posix
{
  /// <summary>
  /// Exceção lançada pelos algoritmos de manipulação do prompt de comando.
  /// </summary>
  public class PromptException : Exception
  {
    public PromptException(string msg) : base(msg) { }
    public PromptException(string msg, Exception ex) : base(msg, ex) { }
  }
}
