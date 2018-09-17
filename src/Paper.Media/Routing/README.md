//
// Revisão da API do Paper
//
// URI patterns:
//
//    Publish Catalog
//
//      /MODULE/Api/REVISION/Paper/Catalog
//
//    List Catalogs
//
//      All Catalogs  -> GET /Paper/Api/1/Catalogs
//      All Versions  -> GET /Paper/Api/1/Catalogs/MODULE
//                       GET /Paper/Api/1/Catalogs/MODULE/Api
//      Get Info      -> GET /Paper/Api/1/Catalogs/MODULE/Api/REVISION
//      Results are all entities
//
//    Register Catalog
//
//      GET /Paper/Api/1/Catalogs/MODULE/Api/REVISION?do=set&uri=http://host.com/MODULE/Api/REVISION
//      POST|PUT /Paper/Api/1/Catalogs/MODULE/Api/REVISION
//        <Uri>http://host.com/MODULE/Api/REVISION</Uri>
//        { "uri": "http://host.com/MODULE/Api/REVISION" }
//
//    Unregister Catalog
//      
//      GET /Paper/Api/1/Catalogs/MODULE/Api/REVISION?do=unset
//      DELETE /Paper/Api/1/Catalogs/MODULE/Api/REVISION

IProvider provider = Provider.GetDefaultProvider();

//
// Catalog
//
ICatalogCollection catalogs = provider.GetCatalogCollection();
ICatalog tradeCatalog = catalogs.SearchCatalog("/Trade/Api/2")
catalogs.AddDefaultCatalog(tradeCatalog);
ICatalog defaultCatalog = catalogs.GetDefaultCatalog();

//
// Entity
//
Route requestUri = "http://host.com/Trade/Api/2/Documents/One.xml"

IPaper paper = defaultCatalog.GetPaper(requestUri.Path);
IRenderer renderer = provider.GetRenderer(paper);

IObjectFactory factory = new DefaultObjectFactory();
factory.Add(new DirectoryContext());
factory.Add(new MailerContext());

IContext context = new DefaultContext();
context.Provider = provider;
context.Catalog = defaultCatalog;

IArgs args = new DefaultArgs(paper.UriTemplate, requestUri.PathAndQueryString);

Entity = renderer.RenderEntity(paper, args, context, factory);

//
// PaperUtility
//
string route = "http://host.com/Trade/Api/2/Documents/One.xml";

Entity entity = PaperUtility.GetEntity(route);
Entity entity = PaperUtility.GetEntity(route, "One.xml");

IArgs args = PaperUtility.GetEntityArgs(route);
args["Id"] = "One.xml";
Entity entity = PaperUtility.GetEntity(route, args);





