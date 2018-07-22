using Media.Service.Papers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Paper.Media.Rendering;
using Paper.Media.Service;
using Paper.WebApp.Server.Proxies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using static System.Environment;

namespace Paper.WebApp.Server
{
  public static class AspNetCoreExtensions
  {
    #region IWebHostBuilder

    public static IWebHostBuilder UsePaperWebAppSettings(this IWebHostBuilder builder, params string[] args)
    {
      return builder;
    }

    #endregion

    #region IServiceCollection

    public static IServiceCollection AddPaperWebAppServices(this IServiceCollection services)
    {
      services.AddSingleton<IProxyRegistry, ProxyRegistry>();
      return services;
    }

    #endregion

    #region IApplicationBuilder

    public static IApplicationBuilder UsePaperWebAppMiddlewares(this IApplicationBuilder app)
    {
      app.Map("/Api/1/Proxies", ProxyPipeline);
      return app;
    }

    private static void ProxyPipeline(IApplicationBuilder app)
    {
      app.UseMiddleware<ProxyRegistryMiddleware>();
      app.UseMiddleware<ProxyMiddleware>();
    }

    #endregion
  }
}