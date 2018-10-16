using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Collections;
using System.Reflection;
using Paper.Media.Rendering_Obsolete;
using Paper.Media.Routing;

namespace Paper.Media.Design.Mappings
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class FieldMultilineAttribute : FieldAttribute
  {
    public bool Allow { get; }

    public FieldMultilineAttribute(bool allow = true)
    {
      Allow = allow;
    }

    internal override void RenderField(Field field, PropertyInfo property, object host, PaperContext ctx)
    {
      field.AddMultiline(Allow);
    }

    internal override void RenderField(Field field, PropertyInfo property, object host)
    {
      field.AddMultiline(Allow);
    }
  }
}