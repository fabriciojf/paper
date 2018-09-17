using Microsoft.AspNetCore.Http;
using Paper.Media.Serialization;
using System;
using System.Threading.Tasks;
using Paper.Media;
using Toolset;
using Paper.Media.Rendering;
using Paper.Media.Rendering.Papers;

namespace Paper.Core
{
  public class PaperMiddleware
  {
    private readonly RequestDelegate next;
    private readonly IPaperCatalog catalog;

    public PaperMiddleware(RequestDelegate next, IPaperCatalog catalog)
    {
      this.next = next;
      this.catalog = catalog;
    }

    public async Task Invoke(HttpContext httpContext, IServiceProvider serviceProvider)
    {
      var path = httpContext.Request.Path.Value;

      var paparInfo = catalog.FindPaper(path);
      var paperType = paparInfo?.Type;
      if (paperType == null)
      {
        await next(httpContext);
        return;
      }
      
      var req = httpContext.Request;
      var res = httpContext.Response;

      var injector = new Injector(serviceProvider);

      var ret = RenderEntity(httpContext, injector, paperType);
      var entity = ret.Data ?? HttpEntity.CreateFromRet(req.GetRequestUri(), ret);

      var contentType = HttpNegotiation.SelectContentType(req);
      var encoding = HttpNegotiation.SelectEncoding(req);

      var serializer = new MediaSerializer(contentType);
      var data = serializer.Serialize(entity);

      res.StatusCode = ret.Status;
      res.ContentType = $"{contentType}; charset={encoding.HeaderName}";

      await res.WriteAsync(data, encoding);
    }

    private Ret<Entity> RenderEntity(HttpContext httpContext, IInjector injector, Type paperType)
    {
      object paper = null;
      try
      {
        paper = injector.CreateInstance(paperType);

        var requestUri = httpContext.Request.GetRequestUri();
        var paperContext = new PaperContext(injector, paper, catalog, requestUri);
        var paperRenderer = new PaperRenderer(injector);

        var ret = paperRenderer.RenderEntity(paperContext);
        return ret;
      }
      catch (Exception ex)
      {
        return ex;
      }
      finally
      {
        (paper as IDisposable)?.Dispose();
      }
    }
  }
}