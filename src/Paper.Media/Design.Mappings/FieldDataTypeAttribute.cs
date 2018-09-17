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
  public class FieldDataTypeAttribute : FieldAttribute
  {
    public string Value { get; }

    public FieldDataTypeAttribute(DataType dataType)
    {
      this.Value = dataType.GetName();
    }

    public FieldDataTypeAttribute(string dataType)
    {
      this.Value = dataType;
    }

    internal override void RenderField(Field field, PropertyInfo property, object host, PaperContext ctx)
    {
      field.AddDataType(Value);
    }
  }
}