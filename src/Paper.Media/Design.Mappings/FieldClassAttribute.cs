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
  public class FieldClassAttribute : FieldAttribute
  {
    public string[] Classes { get; }

    public FieldClassAttribute(Class @class, params Class[] otherClasses)
    {
      Classes = @class.AsSingle().Union(otherClasses).Select(x => x.GetName()).ToArray();
    }

    public FieldClassAttribute(string @class, params string[] otherClasses)
    {
      Classes = @class.AsSingle().Union(otherClasses).ToArray();
    }

    internal override void RenderField(Field field, PropertyInfo property, object host, PaperContext ctx)
    {
      field.AddClass(Classes);
    }
  }
}