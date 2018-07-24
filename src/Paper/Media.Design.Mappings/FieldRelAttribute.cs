using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Collections;

namespace Paper.Media.Design.Mappings
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class FieldRelAttribute : Attribute
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
  }
}