using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Paper.Core;
using Paper.Core.Proxies;
using Paper.Media;
using Paper.Media.Serialization;
using Toolset;

namespace Paper.WebApp.Server.Proxies
{
  class ProxyMiddleware
  {
    private readonly RequestDelegate next;
    private readonly IProxyRegistry registry;

    private static string[] ContentHeaderNames { get; }

    static ProxyMiddleware()
    {
      ContentHeaderNames =
        typeof(HttpContentHeaders)
          .GetProperties()
          .Select(x => x.Name)
          .ToArray();
    }

    static bool IsContentHeader(string key)
    {
      return ContentHeaderNames.Contains(key.ChangeCase(TextCase.PascalCase));
    }

    public ProxyMiddleware(RequestDelegate next, IProxyRegistry registry)
    {
      this.next = next;
      this.registry = registry;
    }

    public async Task Invoke(HttpContext httpContext)
    {
      var path = httpContext.Request.Path.Value;

      var proxy = registry.FindByPrefix(path);
      if (proxy == null)
      {
        await next(httpContext);
        return;
      }

      await RedirectRequest(httpContext, proxy);
    }

    private async Task RedirectRequest(HttpContext httpContext, Proxy proxy)
    {
      Ret ret;
      using (var httpClient = new HttpClient())
      {
        try
        {
          var req = httpContext.Request;
          var res = httpContext.Response;

          var uri = CreateTargetUri(req, proxy);
          if (uri.IsFault())
          {
            await SendStatusAsync(httpContext, uri);
            return;
          }

          var message = CreateMessage(req, uri);
          if (message.IsFault())
          {
            await SendStatusAsync(httpContext, message);
            return;
          }

          ret = CopyRequestToMessage(req, message);
          if (ret.IsFault())
          {
            await SendStatusAsync(httpContext, ret);
            return;
          }

          var result = await httpClient.SendAsync(message);

          ret = CopyResultToResponse(result, res);
          if (ret.IsFault())
          {
            await SendStatusAsync(httpContext, ret);
            return;
          }

          await result.Content.CopyToAsync(res.Body);
        }
        catch (Exception ex)
        {
          await SendStatusAsync(httpContext, Ret.Fail(ex));
        }
      }
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

    private Ret<string> CreateTargetUri(HttpRequest req, Proxy proxy)
    {
      if (!req.Path.StartsWithSegments(proxy.Path))
      {
        return Ret.Fail(HttpStatusCode.BadGateway,
          $"As configurações de proxy para esta roda não estão corretas: {req.Path.Value}");
      }

      var path = req.Path.Value.Substring(proxy.Path.Value.Length);

      var currentRoute = new Route(req.GetRequestUri()).Combine("/");
      var reverseRoute = new Route(proxy.ReverseUri);

      var uri = currentRoute.Combine(reverseRoute).Append(path).ToString();

      return uri;
    }

    private Ret<HttpRequestMessage> CreateMessage(HttpRequest req, string targetUri)
    {
      var method = new HttpMethod(req.Method);
      var message = new HttpRequestMessage(method, targetUri);
      return message;
    }

    private Ret CopyRequestToMessage(HttpRequest req, HttpRequestMessage message)
    {
      // copiando headers
      var exceptions = new string[] { };
      foreach (var entry in req.Headers)
      {
        if (exceptions.Contains(entry.Key))
          continue;

        Debug.WriteLine($"{entry.Key} = {entry.Value}");
        if (IsContentHeader(entry.Key))
          message.Content.Headers.Add(entry.Key, (string)entry.Value);
        else
          message.Headers.Add(entry.Key, (string)entry.Value);
      }

      // copiando dados
      if (req.Method == "POST" || req.Method == "PUT" || req.Method == "PATCH")
      {
        message.Content = new StreamContent(req.Body);
      }

      return true;
    }

    private Ret CopyResultToResponse(HttpResponseMessage message, HttpResponse res)
    {
      // copiando headers
      var exceptions = new string[] { };
      var headers =
        message.Headers
          .Concat(message.Content.Headers)
          .Where(x => x.Key != "Transfer-Encoding");
      foreach (var entry in headers)
      {
        var value = string.Join(",", entry.Value);
        res.Headers.Add(entry.Key, value);
      }

      // copiando status
      res.StatusCode = (int)message.StatusCode;

      return true;
    }
  }
}