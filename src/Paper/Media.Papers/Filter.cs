using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Papers
{
  public class Filter : EntityAction, IFilter
  {
    public Filter()
    {
      this.Name = "Filtro";
      this.Method = MethodNames.Get;
      this.Href = ".";
    }
  }
}
