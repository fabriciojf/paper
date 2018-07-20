using System;
using System.Collections.Generic;
using System.Text;

namespace Media.Design.Extensions.Papers
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
