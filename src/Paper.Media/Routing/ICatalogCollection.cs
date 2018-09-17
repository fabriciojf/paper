using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  public interface ICatalogCollection
  {
    ICatalog GetDefaultCatalog();

    void AddDefaultCatalog(ICatalog catalog);

    ICatalog SearchCatalog(string catalogName);
  }
}