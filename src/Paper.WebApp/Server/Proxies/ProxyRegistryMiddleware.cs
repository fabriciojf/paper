using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Media.Service.Proxies;
using Media.Service.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Paper.Media;
using Paper.Media.Rendering;
using Paper.Media.Serialization;
using Toolset;

namespace Paper.WebApp.Server.Proxies
{
  class ProxyRegistryMiddleware
  {
    private readonly RequestDelegate next;
    private readonly IProxyRegistry registry;

    public ProxyRegistryMiddleware(RequestDelegate next, IProxyRegistry registry)
    {
      this.next = next;
      this.registry = registry;
    }

    public async Task Invoke(HttpContext httpContext)
    {
      try
      {
        var req = httpContext.Request;
        var query = httpContext.Request.Query;

        var path = (string)query["path"];
        var reverseUri = (string)query["reverseUri"];

        var isShowInfoOnly = (req.Method == "GET" && reverseUri == null);
        if (isShowInfoOnly)
        {
          await SendProxyAsync(httpContext);
        }
        else
        {
          await EditProxyAsync(httpContext);
        }
      }
      catch (Exception ex)
      {
        await SendStatusAsync(httpContext, Ret.Fail(ex));
      }
    }

    private async Task SendProxyAsync(HttpContext httpContext)
    {
      var req = httpContext.Request;
      var query = httpContext.Request.Query;

      var path = (string)query["path"];
      if (path != null)
      {
        Proxy proxy = registry.FindExact(path);
        if (proxy == null)
        {
          var ret = HttpEntity.Create(
            route: req.GetRequestUri(),
            status: HttpStatusCode.NotFound,
            message: $"Proxy não registrado: {path}"
          );
          await SendStatusAsync(httpContext, ret);
        }
        else
        {
          var href = req.GetRequestUri();
          var allProxiesHref = new Route(href).UnsetArgs("path");

          var entity = new Entity();
          entity.Title = $"Configuração de proxy: {proxy.Path}";
          entity.Class = KnownClasses.Single;
          entity.Links = new LinkCollection();
          entity.Links.AddSelf(href);
          entity.Links.Add("link", "Todas as configurações de proxy", allProxiesHref);
          entity.Properties = new PropertyCollection();
          entity.Properties.AddFromGraph(proxy);
          entity.Properties.AddDataHeadersFromGraph<Proxy>();

          await SendStatusAsync(httpContext, Ret.Ok(entity));
        }
      }
      else
      {
        var entity = new Entity();
        entity.Class = KnownClasses.Rows;
        entity.Title = "Todas as configurações de proxy";
        entity.Links = new LinkCollection();
        entity.Links.AddSelf(req.GetRequestUri());
        entity.Entities = new EntityCollection();
        entity.Properties = new PropertyCollection();
        entity.Properties.AddRowsHeadersFromGraph<Proxy>();

        foreach (var knownPath in registry.Paths)
        {
          var proxy = registry.FindExact(knownPath);
          var href = 
            new Route(req.GetRequestUri())
              .SetArg("path", proxy.Path)
              .ToString();
          
          var row = new Entity();
          row.Title = $"Configuração de proxy: {proxy.Path}";
          row.Class = KnownClasses.Row;
          row.Rel = KnownRelations.Row;
          row.Links = new LinkCollection();
          row.Links.AddSelf(href);
          row.Properties = new PropertyCollection();
          row.Properties.AddFromGraph(proxy);

          entity.Entities.Add(row);
        }

        await SendStatusAsync(httpContext, Ret.Ok(entity));
      }
    }

    private async Task EditProxyAsync(HttpContext httpContext)
    {
      var req = httpContext.Request;
      var query = httpContext.Request.Query;

      var path = (string)query["path"];
      var reverseUri = (string)query["reverseUri"];

      if (path == null ||reverseUri == null)
      {
        var ret = HttpEntity.Create(
          route: req.GetRequestUri(),
          status: HttpStatusCode.BadRequest,
          message: "Uso incorreto. Os parâmetros `path' e `reverseUri' devem ser indicados."
        );
        await SendStatusAsync(httpContext, ret);
        return;
      }

      bool hasDelete = req.Method.EqualsAny("DELETE", "PUT", "PATCH");
      bool hasEdit = req.Method.EqualsAny("GET", "POST", "PUT", "PATCH");

      Proxy oldProxy = registry.FindExact(path);
      Proxy newProxy;

      try
      {
        newProxy = Proxy.Create(path, reverseUri);
      }
      catch (Exception ex)
      {
        ex.TraceWarning();

        var ret = HttpEntity.Create(
          route: req.GetRequestUri(),
          status: HttpStatusCode.BadRequest,
          message: "Os parâmetros `path' e `reverseUri` não estão corretos."
        );
        await SendStatusAsync(httpContext, ret);
        return;
      }

      if (oldProxy != null && !oldProxy.ReverseUri.Equals(newProxy.ReverseUri))
      {
        var ret = HttpEntity.Create(
          route: req.GetRequestUri(),
          status: HttpStatusCode.BadRequest,
          message: $"Já existe um proxy diferente configurado nesta rota `{oldProxy.Path}'."
        );
        await SendStatusAsync(httpContext, ret);
        return;
      }

      if (hasDelete)
      {
        if (oldProxy != null)
        {
          registry.Remove(path);
        }
      }

      if (hasEdit)
      {
        if (oldProxy != null)
        {
          oldProxy.Enabled = true;
          oldProxy.LastSeen = DateTime.Now;
        }
        else
        {
          registry.Add(newProxy);
        }
      }

      var status = HttpEntity.Create(
        route: req.GetRequestUri(),
        status: HttpStatusCode.OK,
        message: hasEdit ? "Registro de proxy efetuado." : "Registro de proxy removido."
      );

      status.Data.Properties.Add("Path", newProxy.Path.ToString());
      status.Data.Properties.Add("ReverseUri", newProxy.ReverseUri.ToString());

      await SendStatusAsync(httpContext, status);
    }

    private async Task SendStatusAsync(HttpContext httpContext, Ret ret)
    {
      var req = httpContext.Request;
      var res = httpContext.Response;

      var entity =
        (ret.Data as Entity)
        ?? HttpEntity.CreateFromRet(req.GetRequestUri(), ret);

      var contentType = HttpNegotiation.SelectContentType(req);
      var encoding = HttpNegotiation.SelectEncoding(req);

      var serializer = new MediaSerializer(contentType);
      var data = serializer.Serialize(entity);

      res.StatusCode = ret.Status;
      res.ContentType = $"{contentType}; charset={encoding.HeaderName}";

      await res.WriteAsync(data, encoding);
    }
  }
}