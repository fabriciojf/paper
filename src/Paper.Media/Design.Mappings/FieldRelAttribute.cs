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
  public class FieldRelAttribute : FieldAttribute
  {
    public string[] Rels { get; }

    public FieldRelAttribute(Rel rel, params Rel[] otherRels)
    {
      Rels = rel.AsSingle().Union(otherRels).Select(x => x.GetName()).ToArray();
    }

    public FieldRelAttribute(string rel, params string[] otherRels)
    {
      Rels = rel.AsSingle().Union(otherRels).ToArray();
    }

    internal override void RenderField(Field field, PropertyInfo property, object host, PaperContext ctx)
    {
      field.AddRel(Rels);
    }
  }
}