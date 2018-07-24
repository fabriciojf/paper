using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media.Design.Widgets;

namespace Paper.Media.Design
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