
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Paper.Media.Rendering;
using Toolset.Collections;

namespace Paper.Media.Design.Mappings
{
  public abstract class FieldAttribute : Attribute
  {
    internal abstract void RenderField(Field field, PropertyInfo property, object host, PaperContext ctx);
  }
}