using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Paper.Media.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static System.Environment;

namespace Paper.Client.Mvc
{
  public static class AspNetCoreExtensions
  {
    #region IMvcBuilder

    public static IMvcBuilder AddPaperMvcParts(this IMvcBuilder mvc)
    {
      var assembly = typeof(AspNetCoreExtensions).Assembly;
      mvc.AddApplicationPart(assembly);
      mvc.Services.Configure<RazorViewEngineOptions>(options =>
        options.FileProviders.Add(new EmbeddedFileProvider(assembly))
      );
      return mvc;
    }

    #endregion
  }
}