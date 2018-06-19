using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Toolset.Posix
{
  /// <summary>
  /// Atributo opcional para determinar o nome da ferramenta.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class HelpTitleAttribute : HelpAttribute
  {
    public HelpTitleAttribute(string name) : base(name) { }
  }
}
