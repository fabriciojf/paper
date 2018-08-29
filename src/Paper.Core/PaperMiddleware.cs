using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Paper.Media.Serialization;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Paper.Media;
using Toolset;
using Paper.Media.Design.Rendering;
using Paper.Media.Routing;
using Paper.Media.Design.Rendering.Papers;

namespace Paper.Core.Service
{
  public class PaperMiddleware
  {
    private readonly RequestDelegate next;
    private readonly IPaperRegistry registry;

    public PaperMiddleware(RequestDelegate next, IPaperRegistry registry)
    {
      this.next = next;
      this.registry = registry;
    }

    public async Task Invoke(HttpContext httpContext, IServiceProvider serviceProvider)
    {
      var path = httpContext.Request.Path.Value;

      var paparInfo = registry.FindPaper(path);
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
        var paperContext = new PaperContext(injector, paper, registry, requestUri);
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