using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Toolset.Collections;

namespace Paper.Media.Papers.Rendering
{
  public class PaperContext
  {
    private EntryCollection _cache;
    private ArgCollection _queryArgs;
    private ArgCollection _pathArgs;

    public PaperContext(IPaper paper, IList<Type> knownPapers, string requestUri)
    {
      this.Paper = paper;
      this.KnownPapers = knownPapers ?? new Type[0];
      this.RequestUri = requestUri;
    }

    public IPaper Paper { get; }

    public IList<Type> KnownPapers { get; }

    public string UriTemplate => PaperAttribute.GetFrom(Paper)?.UriTemplate ?? "/";

    public string RequestUri { get; }

    public EntryCollection Cache => _cache ?? (_cache = new EntryCollection());

    /// <summary>
    /// Argumentos coletados do caminho da URI pela aplicação do template de URI.
    /// </summary>
    public ArgCollection PathArgs
    {
      get
      {
        if (_pathArgs == null)
        {
          _pathArgs = CollectPathArgs(this.RequestUri, this.UriTemplate);
        }
        return _pathArgs;
      }
    }

    /// <summary>
    /// Argumentos extraídos da URI.
    /// </summary>
    public ArgCollection QueryArgs
    {
      get
      {
        if (_queryArgs == null)
        {
          _queryArgs = CollectQueryArgs(this.RequestUri);
        }
        return _queryArgs;
      }
    }

    /// <summary>
    /// Aplica o template de URI e extrai os argumentos correspondentes
    /// encontrados na URI de requisição.
    /// </summary>
    /// <returns>Os argumentos extraídos da URI com a aplicação do template.</returns>
    private static ArgCollection CollectPathArgs(string requestUri, string uriTemplate)
    {
      var args = new ArgCollection();

      requestUri = requestUri.Split('?').First();

      var keyPattern = new Regex(@"\{([^{}]+)\}");
      var uriPattern = new Regex(keyPattern.Replace(uriTemplate, @"([^/]+)"));

      var keyMatches = keyPattern.Matches(uriTemplate);
      var uriMatches = uriPattern.Matches(requestUri);
      for (var i = 0; i < keyMatches.Count; i++)
      {
        Match keyMatch = keyMatches[i];
        Match uriMatch = (i < uriMatches.Count) ? uriMatches[i] : null;

        var key = keyMatch.Groups[1].Value;
        var value = uriMatch?.Groups[1].Value;

        args.Set(key, value);
      }

      return args;
    }

    /// <summary>
    /// Extrai os parâmetros da coleção de argumentos da URL.
    /// </summary>
    /// <returns>Os argumentos extraídos da URL.</returns>
    private static ArgCollection CollectQueryArgs(string requestUri)
    {
      var queryString = requestUri.Split('?').LastOrDefault() ?? "";
      var entries =
        from arg in queryString.Split('&')
        where arg.Contains("=")
        let tokens = arg.Split('=')
        let key = tokens.First()
        let value = string.Join('=', tokens.Skip(1))
        select KeyValuePair.Create(key, value);
      var args = new ArgCollection(entries);
      return args;
    }
  }
}