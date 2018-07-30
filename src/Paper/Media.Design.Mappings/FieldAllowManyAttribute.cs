using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Collections;
using Paper.Media.Design.Papers;
using Paper.Media.Design.Papers.Rendering;
using System.Reflection;

namespace Paper.Media.Design.Mappings
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class FieldAllowManyAttribute : FieldAttribute
  {
    public bool Allow { get; }

    public FieldAllowManyAttribute(bool allow = true)
    {
      Allow = allow;
    }

    internal override void RenderField(Field field, PropertyInfo property, object host, PaperContext ctx)
    {
      field.AddAllowMany(Allow);
    }
  }
}