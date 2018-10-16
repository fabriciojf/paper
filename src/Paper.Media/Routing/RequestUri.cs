using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Representação de uma rota do Paper.
  /// </summary>
  public class RequestUri
  {
    public RequestUri(string uriPrefix, string requestUri)
    {
      this.Uri = requestUri;
      this.Prefix = uriPrefix;
      this.Path = new Route(requestUri).MakeRelative(uriPrefix).UnsetAllArgs();

      var tokens = requestUri.Split('?');
      var queryString = string.Join("?", tokens.Skip(1));

      this.Query = new Query(queryString);
      this.QueryString = queryString;
    }

    /// <summary>
    /// Uri exatamente como requisitada pelo cliente do aplicativo.
    /// </summary>
    public string Uri { get; }

    /// <summary>
    /// Prefixo de URI.
    /// Geralmente formado pelo contexto da aplicação mais o número de API.
    /// Como em: /Meu/Site/Api/1
    /// </summary>
    public string Prefix { get; }

    /// <summary>
    /// Caminho da URI relativo ao prefixo. Como em: /Minha/Pagina
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Argumentos da URI interpretados.
    /// </summary>
    public Query Query { get; }

    /// <summary>
    /// Argumentos da URI.
    /// </summary>
    public string QueryString { get; }

    /// <summary>
    /// Conversão para string.
    /// </summary>
    public override string ToString()
    {
      return Uri;
    }

    /// <summary>
    /// Conversão implícita para string.
    /// </summary>
    public static implicit operator string(RequestUri uri)
    {
      return uri.ToString();
    }
  }
}