using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  public interface IProvider
  {
    ICatalogCollection GetCatalogCollection();

    IRenderer GetRenderer(IPaper paper);
  }
}