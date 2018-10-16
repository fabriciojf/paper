using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media.Design;

namespace Paper.Media.Routing
{
  static class Sampling
  {
    public static void Main()
    {
      IProvider provider = Provider.Current;

      //
      // Catalog
      //
      ICatalogSearcher catalogs = provider.GetCatalogCollection();
      ICatalog tradeCatalog = catalogs.SearchCatalog("/Trade/Api/2");

      //
      // Entity
      //
      RequestUri requestUri = new RequestUri("/Trade/Api/2", "http://host.com/Trade/Api/2/Documents/One.xml");

      PaperBlueprint blueprint = tradeCatalog.GetPaperBlueprint(requestUri.Path);
      IRenderer renderer = provider.GetRenderer(blueprint);

      ICache cache = new DefaultCache();
      IObjectFactory factory = new DefaultObjectFactory();

      IArgs args = new DefaultArgs(blueprint.UriTemplate, requestUri);

      IContext context = new DefaultContext
      {
        RequestArgs = args,
        RequestUri = requestUri,
        Provider = provider,
        Catalog = tradeCatalog,
        Factory = factory,
        Cache = cache
      };

      IPaper paper = blueprint.Paper;
      Entity entity = renderer.RenderEntity(paper, context);

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