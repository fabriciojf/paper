using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using Media.Service;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Toolset;

namespace Paper.WebApp.Server
{
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        WebHost.CreateDefaultBuilder(args)
          .UsePaperWebAppSettings()
          .UsePaperSettings()
          .UseStartup<Startup>()
          .Build()
          .Run();
      }
      catch (Exception ex)
      {
        ex.Trace();
      }
    }
  }
}