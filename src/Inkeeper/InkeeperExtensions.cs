using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Inkeeper
{
  public static class InkeeperExtensions
  {
    public static IWebHostBuilder UseInkeeper(this IWebHostBuilder builder, Action<InkeeperOptions> configurator = null)
    {
      var options = new InkeeperOptions();
      configurator?.Invoke(options);
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        builder.UseHttpSys(opts =>
        {
          opts.UrlPrefixes.Add($"http://localhost:{options.Port}");
        });
      }
      return builder;
    }
  }
}