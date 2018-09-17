using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  static class Sampling
  {
    public static void Main()
    {
      IProvider provider = Provider.GetDefaultProvider();

      //
      // Catalog
      //
      ICatalogCollection catalogs = provider.GetCatalogCollection();
      ICatalog tradeCatalog = catalogs.SearchCatalog("/Trade/Api/2");

      catalogs.AddDefaultCatalog(tradeCatalog);
      ICatalog defaultCatalog = catalogs.GetDefaultCatalog();

      //
      // Entity
      //
      Route requestUri = "http://host.com/Trade/Api/2/Documents/One.xml";

      IPaper paper = defaultCatalog.GetPaper(requestUri.Path);
      IRenderer renderer = provider.GetRenderer(paper);

      IObjectFactory factory = new DefaultObjectFactory();

      IContext context = new DefaultContext
      {
        Provider = provider,
        Catalog = defaultCatalog
      };

      IArgs args = new DefaultArgs(paper.UriTemplate, requestUri.PathAndQueryString);

      Entity entity = renderer.RenderEntity(paper, args, context, factory);

      //
      // PaperUtility
      //
      string route = "http://host.com/Trade/Api/2/Documents/One.xml";

      Entity entity1 = PaperUtility.GetEntity(route);
      Entity entity2 = PaperUtility.GetEntity(route, "One.xml");

      IArgs paperArgs = PaperUtility.GetEntityArgs(route);
      args["Id"] = "One.xml";
      Entity entity3 = PaperUtility.GetEntity(route, args);
    }
  }
}