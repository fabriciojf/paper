using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Toolset;

namespace Paper.Core.Proxies
{
  public class Proxy
  {
    private static readonly TimeSpan MaxDelay = TimeSpan.FromMinutes(1);

    [DisplayName("Caminho")]
    public PathString Path { get; set; }

    [DisplayName("URI Reversa")]
    public Uri ReverseUri { get; set; }

    [DisplayName("Visto Em")]
    public DateTime LastSeen { get; set; } = DateTime.Now;

    [DisplayName("Habilitado")]
    public bool Enabled { get; set; } = true;

    [DisplayName("Disponível")]
    public bool Available => Enabled && ((DateTime.Now - LastSeen) <= MaxDelay);

    public static Proxy Create(string path, string reverseUri)
    {
      var proxy = new Proxy();

      try
      {
        proxy.Path = path;
      }
      catch (Exception ex)
      {
        throw new Exception(
          $"O proxy pré-configurado não define a propriedade Path corretamente: {path ?? "(null)"}"
          , ex);
      }

      try
      {
        proxy.ReverseUri = new Uri(reverseUri, UriKind.RelativeOrAbsolute);
      }
      catch (Exception ex)
      {
        throw new Exception(
          $"O proxy pré-configurado não define a propriedade ReverseUri corretamente: {reverseUri ?? "(null)"}"
          , ex);
      }

      return proxy;
    }
  }
}