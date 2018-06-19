using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Toolset.Posix
{
  /// <summary>
  /// Atributo opcional para personalização do texto no início da ajuda.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class HelpHeaderAttribute : HelpAttribute
  {
    public HelpHeaderAttribute(params string[] lines) : base(lines) { }
  }
}
