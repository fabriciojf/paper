using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Paper.Core.Proxies;
using Paper.Core.Utilities;
using Paper.Media.Utilities;
using Toolset;

namespace Paper.WebApp.Server.Proxies
{
  class ProxyRegistry : IProxyRegistry
  {
    private readonly PathIndex<Proxy> index = new PathIndex<Proxy>();

    public ProxyRegistry(IConfiguration configuration)
    {
      LoadPredefinedProxies(configuration);
    }

    private void LoadPredefinedProxies(IConfiguration configuration)
    {
      var sections = configuration.GetSection("Paper:Proxies").GetChildren();
      foreach (var section in sections)
      {
        try
        {
          var proxy = Proxy.Create(section["Path"], section["ReverseUri"]);
          this.Add(proxy);
        }
        catch (Exception ex)
        {
          ex.Trace($"[PAPER]Falha mapeando o proxy {section.Key}: {ex.Message}");
          continue;
        }
      }
    }

    public string[] Paths => index.Paths;

    public void Add(Proxy proxy) => index.Add(proxy.Path, proxy);

    public void Remove(string path) => index.Remove(path);

    public Proxy FindExact(string path) => index.FindExact(path);

    public Proxy FindByPrefix(string path) => index.FindByPrefix(path);
  }
}