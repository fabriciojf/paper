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
  public class FieldMaxLengthAttribute : FieldAttribute
  {
    public int Value { get; }

    public FieldMaxLengthAttribute(int minLength)
    {
      Value = minLength;
    }

    internal override void RenderField(Field field, PropertyInfo property, object host, PaperContext ctx)
    {
      field.AddMaxLength(Value);
    }

    internal override void RenderField(Field field, PropertyInfo property, object host)
    {
      field.AddMaxLength(Value);
    }
  }
}