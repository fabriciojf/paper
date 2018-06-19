using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Toolset.Reflection;

namespace Paper.Media.Design.Widgets.Mapping
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class IgnoreAttribute : Attribute
  {
  }
}