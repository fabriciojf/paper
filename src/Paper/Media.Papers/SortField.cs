
using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Papers
{
  public class SortField
  {
    public SortField()
    {
    }

    public SortField(string name)
    {
    }

    public SortField(string name, SortOrder order)
    {
      this.Name = name;
      this.Order = order;
    }

    public string Name { get; set; }

    public SortOrder Order { get; set; }

    public override string ToString()
    {
      if (Order == SortOrder.Ascending) return $"{Name} ASC";
      if (Order == SortOrder.Descending) return $"{Name} DESC";
      return Name ?? "";
    }
  }
}