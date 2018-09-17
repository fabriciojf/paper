using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Paper.Media.Rendering;
using Toolset.Collections;

namespace Paper.Media.Design.Mappings
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class FieldPatternAttribute : FieldAttribute
  {
    public string Value { get; }

    public FieldPatternAttribute(string text)
    {
      Value = text;
    }

    internal override void RenderField(Field field, PropertyInfo property, object host, PaperContext ctx)
    {
      field.AddPattern(Value);
    }
  }
}