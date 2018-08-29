using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Toolset;

namespace Paper.Media
{
  [CollectionDataContract(Namespace = Namespaces.Default, Name = "Fields")]
  public class FieldCollection : List<Field>
  {
    public FieldCollection()
    {
    }

    public FieldCollection(IEnumerable<Field> items)
    : base(items)
    {
    }

    public Field this[string fieldName]
    {
      get => this.FirstOrDefault(x => x.Name.EqualsIgnoreCase(fieldName));
      set => this.Add(value);
    }
  }
}