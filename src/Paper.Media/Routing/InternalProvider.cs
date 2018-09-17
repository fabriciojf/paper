
using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  internal class InternalProvider : IProvider
  {
    private InternalCatalogCollection catalogCollection;

    public ICatalogCollection GetCatalogCollection()
    {
      return catalogCollection ?? (catalogCollection = new InternalCatalogCollection());
    }

    public IRenderer GetRenderer(IPaper paper)
    {
      throw new NotImplementedException();
    }
  }
}