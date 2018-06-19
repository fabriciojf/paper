using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Toolset.Collections;

namespace Paper.Media.Rendering
{
  public static class AspNetCoreExtensions
  {
    public static T CreateInstance<T>(this IServiceProvider provider, params object[] args)
    {
      return ActivatorUtilities.CreateInstance<T>(provider, args);
    }

    public static object CreateInstance(this IServiceProvider provider, Type intanceType, params object[] args)
    {
      return ActivatorUtilities.CreateInstance(provider, intanceType, args);
    }

    public static string GetRequestUri(this HttpRequest request)
    {
      string uri = "";

      if (request.Scheme != null)
        uri = string.Concat(uri, request.Scheme, "://");

      if (request.Host.HasValue)
        uri = string.Concat(uri, request.Host.ToUriComponent());

      uri = string.Concat(
        uri,
        request.PathBase.ToUriComponent(),
        request.Path.ToUriComponent(),
        request.QueryString.ToUriComponent()
      );

      return uri;
    }

    public static IDictionary<object, object> GetCache(this HttpContext httpContext)
    {
      return httpContext.Items ?? (httpContext.Items = new Map<object, object>());
    }
  }
}