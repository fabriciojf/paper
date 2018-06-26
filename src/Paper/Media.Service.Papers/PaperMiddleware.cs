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
using Paper.Media.Rendering;
using Media.Service.Utilities;

namespace Media.Service.Papers
{
  public class PaperMiddleware
  {
    private readonly RequestDelegate next;
    private readonly IPaperRendererRegistry registry;

    public PaperMiddleware(RequestDelegate next, IPaperRendererRegistry registry)
    {
      this.next = next;
      this.registry = registry;
    }

    public async Task Invoke(HttpContext httpContext, IServiceProvider injector)
    {
      var path = httpContext.Request.Path.Value;

      var renderer = registry.FindPaperRenderer(path);
      if (renderer == null)
      {
        await next(httpContext);
        return;
      }

      var req = httpContext.Request;
      var res = httpContext.Response;
      
      var ret = RenderEntity(httpContext, injector, renderer);
      var entity = ret.Data ?? HttpEntity.CreateFromRet(req.GetRequestUri(), ret);

      var contentType = HttpNegotiation.SelectContentType(req);
      var encoding = HttpNegotiation.SelectEncoding(req);

      var serializer = new MediaSerializer(contentType);
      var data = serializer.Serialize(entity);

      res.StatusCode = ret.Status;
      res.ContentType = $"{contentType}; charset={encoding.HeaderName}";

      await res.WriteAsync(data, encoding);
    }

    private Ret<Entity> RenderEntity(HttpContext httpContext, IServiceProvider injector, PaperRendererInfo rendererType)
    {
      try
      {
        var path = httpContext.Request.Path.Value;

        var renderer = (IPaperRenderer)injector.CreateInstance(rendererType.PaperRendererType);
        renderer.PaperType = rendererType.PaperType;
        renderer.PathTemplate = rendererType.PathTemplate;

        var ret = renderer.RenderEntity(httpContext, path);

        return ret;
      }
      catch (Exception ex)
      {
        return ex;
      }
    }
  }
}