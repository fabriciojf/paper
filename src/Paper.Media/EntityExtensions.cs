using System;
using System.Linq;
using System.Collections.Generic;

namespace Paper.Media
{
  public static class EntityExtensions
  {
    public static IEnumerable<Entity> EntitiesAndSelf(this Entity entity)
    {
      return new[] { entity }.Concat(entity.Entities);
    }
  }
}