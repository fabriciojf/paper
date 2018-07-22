
using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design
{
  public struct SortedField
  {
    public SortedField(string name, SortOrder order)
    {
      this.Name = name;
      this.Order = order;
    }

    public string Name { get; set; }

    public SortOrder Order { get; set; }

    public override string ToString()
    {
      return (Order == SortOrder.Descending) ? $"{Name} desc" : $"{Name} asc";
    }
  }
}