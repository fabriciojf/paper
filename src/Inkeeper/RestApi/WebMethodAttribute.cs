using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Inkeeper.RestApi
{
  public class WebMethodAttribute : Attribute
  {
    public string Method { get; }
    public string Route { get; set; }

    public WebMethodAttribute(string method, string route = null)
    {
      this.Method = method;
      this.Route = route;
    }

    internal string GetRouteForMethod(MethodInfo methodInfo)
    {
      var route = this.Route?.Trim();

      if (string.IsNullOrEmpty(route))
      {
        route = methodInfo.Name;
      }

      if (route.EndsWith("/"))
      {
        route = route.Substring(0, route.Length - 1);
      }

      if (!route.StartsWith("/"))
      {
        route = "/" + route;
      }

      return route;
    }
  }
}