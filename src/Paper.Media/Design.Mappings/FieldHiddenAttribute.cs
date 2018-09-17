using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Collections;
using System.Reflection;
using Paper.Media.Rendering;

namespace Paper.Media.Design.Mappings
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class FieldHiddenAttribute : FieldAttribute
  {
    public bool Hidden { get; }

    public FieldHiddenAttribute(bool allow = true)
    {
      Hidden = allow;
    }

    internal override void RenderField(Field field, PropertyInfo property, object host, PaperContext ctx)
    {
      field.AddHidden(Hidden);
    }
  }
}