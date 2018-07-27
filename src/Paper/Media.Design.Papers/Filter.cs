using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Papers
{
  public class Filter : EntityAction, IFilter
  {
    public Filter()
    {
      base.Name = "Filtro";
      base.Method = MethodNames.Get;
      base.Href = ".";
    }
  }
}