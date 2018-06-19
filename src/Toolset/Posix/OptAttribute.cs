using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Toolset.Posix
{
  /// <summary>
  /// Atributo opcional para personalização de opção.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class OptAttribute : Attribute
  {
    private volatile static int orderer = 0;

    public readonly int Order;
    public readonly char? ShortName;
    public readonly string LongName;
    public string Category;

    public OptAttribute()
    {
      this.Order = ++orderer;
    }

    public OptAttribute(char shortName, string longName)
    {
      this.Order = ++orderer;
      ShortName = shortName;
      LongName = longName;
    }

    public OptAttribute(char shortName)
    {
      this.Order = ++orderer;
      ShortName = shortName;
    }

    public OptAttribute(string longName)
    {
      this.Order = ++orderer;
      LongName = longName;
    }
  }
}