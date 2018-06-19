using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Paper.Media
{
  /// <summary>
  /// Coleção de ações.
  /// </summary>
  [CollectionDataContract(Namespace = Namespaces.Default, Name = "Actions")]
  public class EntityActionCollection : List<EntityAction>
  {
    public EntityActionCollection()
    {
    }

    public EntityActionCollection(IEnumerable<EntityAction> items)
    : base(items)
    {
    }

    public EntityActionCollection(params EntityAction[] items)
    : base(items)
    {
    }
  }
}