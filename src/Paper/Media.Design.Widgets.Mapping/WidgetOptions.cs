using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Widgets.Mapping
{
  [AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Method,
    AllowMultiple = true,
    Inherited = true
  )]
  public class WidgetOptions : Attribute
  {
    public string For { get; set; }
    public string ValueKey { get; set; }
    public string TitleKey { get; set; }
  }
}
