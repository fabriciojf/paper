using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Inkeeper.RestApi
{
  public class WebMethodAttribute : Attribute
  {
    internal string[] Methods { get; }
    public string Route { get; set; }

    public WebMethodAttribute(string method, string route = null)
    {
      string[] methods;

      if (method == "*")
      {
        methods = new[] { "GET", "POST", "PUT", "DELETE" };
      }
      else
      {
        methods = method
          .ToUpper()
          .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
      }

      this.Methods = methods;
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