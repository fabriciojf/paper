
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Paper.Media.Rendering_Obsolete;
using Paper.Media.Routing;
using Toolset.Collections;

namespace Paper.Media.Design.Mappings
{
  public abstract class FieldAttribute : Attribute
  {
    [Obsolete("Será removido em breve")]
    internal abstract void RenderField(Field field, PropertyInfo property, object host, PaperContext ctx);

    internal abstract void RenderField(Field field, PropertyInfo property, object host);
  }
}