using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Media.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Toolset.Collections;

namespace Paper.WebApp.Host
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddPaperHostServices();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider provider)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      var fileProvider = new CompositeFileProvider(
        env.ContentRootFileProvider,
        new PaperHostStaticFileProvider()
      );

      app.UsePaperHostMiddlewares();

      app.UseDefaultFiles();
      app.UseStaticFiles(new StaticFileOptions { FileProvider = fileProvider });
      app.UseDirectoryBrowser(new DirectoryBrowserOptions { FileProvider = fileProvider });
    }
  }
}