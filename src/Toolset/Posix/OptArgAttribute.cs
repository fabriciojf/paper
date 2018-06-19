using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Toolset.Posix
{
  /// <summary>
  /// Atributo opcional para personalização de valor de opção.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class OptArgAttribute : Attribute
  {
    public readonly string Name;
    public OptArgAttribute(string name, params string[] others)
    {
      Name = string.Join(",", name.AsSingle().Concat(others));
    }
  }
}