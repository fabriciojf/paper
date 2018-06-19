using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Paper.Media
{
  [CollectionDataContract(Namespace = Namespaces.Default, ItemName = "Item")]
  [KnownType(typeof(PropertyCollection))]
  [KnownType(typeof(CaseVariantString))]
  public class Many : List<object>
  {
    public Many()
    {
    }

    public Many(IEnumerable<object> items)
    : base(items)
    {
    }
  }
}