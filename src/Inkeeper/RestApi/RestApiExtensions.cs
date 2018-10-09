using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Toolset;

namespace Inkeeper.RestApi
{
  public static class RestApiExtensions
  {
    public static IApplicationBuilder UseInkeeperRestApi(this IApplicationBuilder app)
    {
      var services =
        from type in ExposedTypes.GetTypes()
        from attr in type.GetCustomAttributes(true).OfType<RestAttribute>()
        select new { type, attr };

      foreach (var service in services)
      {
        try
        {
          var route = service.attr.Route;
          if (route == null)
          {
            var words = service.type.FullName.EnumerateWords();
            route = string.Join("/", words);
          }

          if (route.EndsWith("/"))
          {
            route = route.Substring(0, route.Length - 1);
          }

          if (!route.StartsWith("/"))
          {
            route = "/" + route;
          }

          app.Map(route, map => map.UseMiddleware<RestMiddleware>(route, service.type));
        }
        catch (Exception ex)
        {
          ex.Trace();
        }
      }
      
      return app;
    }
  }
}