using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Media.Service.Proxies;

namespace Paper.WebApp.Server.Proxies
{
  interface IProxyRegistry
  {
    string[] Paths { get; }

    void Add(Proxy proxy);

    void Remove(string path);

    Proxy FindExact(string path);

    Proxy FindByPrefix(string path);
  }
}