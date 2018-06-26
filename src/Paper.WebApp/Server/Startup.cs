using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Media.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Paper.WebApp.Server
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
      services.AddPaperWebAppServices();
      services.AddPaperServices();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
        app.UseDeveloperExceptionPage();

      app.UseDefaultFiles();
      app.UseStaticFiles();
      app.UseDirectoryBrowser();
      app.UsePaperWebAppMiddlewares();
      app.UsePaperMiddlewares();
    }
  }
}