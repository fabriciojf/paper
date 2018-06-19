using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paper.Media.Design.Fluid
{
  public static class EntityExtensions
  {
    public static Entity AddClass(this Entity target, string entityClass)
    {
      if (target.Class == null)
        target.Class = new NameCollection();

      if (!target.Class.Contains(entityClass))
        target.Class.Add(entityClass);

      return target;
    }

    public static Link AddClass(this Link target, string entityClass)
    {
      if (target.Class == null)
        target.Class = new NameCollection();

      if (!target.Class.Contains(entityClass))
        target.Class.Add(entityClass);

      return target;
    }

    public static Entity AddRel(this Entity target, string rel)
    {
      if (target.Rel == null)
        target.Rel = new NameCollection();

      if (!target.Rel.Contains(rel))
        target.Rel.Add(rel);

      return target;
    }

    public static Link AddRel(this Link target, string rel)
    {
      if (target.Rel == null)
        target.Rel = new NameCollection();

      if (!target.Rel.Contains(rel))
        target.Rel.Add(rel);

      return target;
    }
  }
}