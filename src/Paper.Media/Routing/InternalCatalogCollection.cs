using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  internal class InternalCatalogCollection : ICatalogCollection
  {
    private AggregateCatalog defaultCatalog;

    public InternalCatalogCollection()
    {
      defaultCatalog = new AggregateCatalog();
    }

    public ICatalog GetDefaultCatalog()
    {
      return defaultCatalog;
    }

    public ICatalog SearchCatalog(string catalogName)
    {
      return new InternalCatalog();
    }

    public void AddDefaultCatalog(ICatalog catalog)
    {
      defaultCatalog.Add(catalog);
    }
  }
}