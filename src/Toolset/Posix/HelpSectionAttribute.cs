using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Toolset.Posix
{
  /// <summary>
  /// Atributo opcional para texto para criação de seção de ajuda adicional.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
  public class HelpSectionAttribute : Attribute
  {
    public string Name;
    public string Text;
    public SectionOrder Order;
    public HelpSectionAttribute(string sectionName, params string[] lines)
    {
      Name = sectionName;
      Text = lines.JoinLines();
    }
    public HelpSectionAttribute(string sectionName, SectionOrder order, params string[] lines)
    {
      Name = sectionName;
      Order = order;
      Text = lines.JoinLines();
    }
  }
}
