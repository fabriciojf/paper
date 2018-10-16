using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Toolset;
using Toolset.Collections;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Implementação simplificada da coleção de argumentos.
  /// 
  /// A coleção extrai argumentos da URI de requisição com base no 
  /// template de URI.
  /// 
  /// O template de URI tem a forma: /Rota/{parametro}/...?chave={parametro}
  /// Cada parametro na forma "{parametro}" é extraído e estocado na coleção.
  /// </summary>
  public class DefaultArgs : IArgs
  {
    /// <summary>
    /// Aplicador do template de URI para extração de argumentos.
    /// </summary>
    private class Template
    {
      private string[] _allTargets;

      /// <summary>
      /// Regex para extração de argumentos do caminho da URI.
      /// </summary>
      public Regex PathRegex { get; internal set; }

      /// <summary>
      /// Nome dos argumentos extraídos do caminho da URI.
      /// </summary>
      public string[] PathTargets { get; internal set; }

      /// <summary>
      /// Nome dos argumentos obtidos dos argumentos de URI.
      /// A porção depois da interrogação (?).
      /// Exemplo:
      /// -   host.com?arg={parametro}&arg2={parametro2}
      /// </summary>
      public string[] QuerySources { get; internal set; }

      /// <summary>
      /// Nome destino dos argumentos extraídos da URI.
      /// </summary>
      public string[] QueryTargets { get; internal set; }

      /// <summary>
      /// Nome de todos os argumentos na ordem em que foram declarados no template de URI.
      /// </summary>
      public string[] AllTargets
        => _allTargets ?? (_allTargets = PathTargets.Concat(QueryTargets).ToArray());
    }

    private static readonly object synclock = new object();
    private static readonly HashMap<Template> templates;

    private readonly string[] names;
    private readonly HashMap values;
    
    static DefaultArgs()
    {
      templates = new HashMap<Template>();
    }

    /// <summary>
    /// Construtor dos argumentos.
    /// </summary>
    /// <param name="uriTemplate">
    /// Template de URI, na forma: /Rota/{parametro}/...
    /// </param>
    /// <param name="requestUri">
    /// URI de requisição do aplicativo cliente.
    /// </param>
    public DefaultArgs(string uriTemplate, string requestUri)
    {
      Template template;
      lock (synclock)
      {
        template =
          templates[uriTemplate]
          ?? (templates[uriTemplate] = ParseTemplate(uriTemplate));
      }

      names = template.AllTargets;
      values = ParseArguments(template, requestUri);
    }

    /// <summary>
    /// Quantidade de argumentos definidos.
    /// </summary>
    public int Count => names.Length;

    /// <summary>
    /// Nomes dos argumentos definidos.
    /// </summary>
    public ICollection<string> Names => names;

    /// <summary>
    /// Obtém o argumento da posição.
    /// </summary>
    /// <param name="index">A posição do argumento.</param>
    /// <returns>O valor do argumento.</returns>
    public object this[int index]
    {
      get
      {
        var name = names.ElementAtOrDefault(index);
        return (name != null) ? values[name] : null;
      }
      set
      {
        var name = names.ElementAtOrDefault(index);
        if (name == null)
          throw new IndexOutOfRangeException($"O índice não existe: {index}");

        values[name] = value;
      }
    }

    /// <summary>
    /// Obtém o valor do argumento.
    /// </summary>
    /// <param name="name">Nome do argumento.</param>
    /// <returns>O valor do argumento.</returns>
    public object this[string name]
    {
      get => values[name];
      set => values[name] = value;
    }

    /// <summary>
    /// Interpreta o template de URI e constrói o aplicador de template.
    /// 
    /// O template de URI tem a forma: /Rota/{parametro}/...?chave={parametro}
    /// Cada parametro na forma "{parametro}" é extraído e estocado na coleção.
    /// </summary>
    /// <param name="uriTemplate">O template de URI.</param>
    /// <returns>O aplicador de template.</returns>
    private Template ParseTemplate(string uriTemplate)
    {
      MatchCollection matches;

      var tokens = uriTemplate.Split('?');
      var path = tokens.FirstOrDefault();
      var queryString = tokens.Skip(1).FirstOrDefault() ?? "";

      var pattern = Regex.Escape(Regex.Replace(path, @"\{[^{}]*\}", "§"));
      var pathPattern = $"^{pattern.Replace("§", @"([^/?]+)")}$";
      var pathArgNamesPattern = @"\{([^/?]+)\}";

      matches = Regex.Matches(path, pathArgNamesPattern);
      var pathArgNames = (
        from m in matches.Cast<Match>()
        select m.Groups[1].Value
      ).ToArray();

      matches = Regex.Matches(queryString, @"([^?&=]+)=\{([^?&=]+)\}");
      var queryArgNames = (
        from m in matches.Cast<Match>()
        select new { source = m.Groups[1].Value, target = m.Groups[2].Value }
      ).ToArray();

      return new Template
      {
        PathRegex = new Regex(pathPattern),
        PathTargets = pathArgNames,
        QuerySources = queryArgNames.Select(x => x.source).ToArray(),
        QueryTargets = queryArgNames.Select(x => x.target).ToArray()
      };
    }

    /// <summary>
    /// Interpretador dos argumentos da URI de requisição.
    /// O algoritmo aplica o template para extrair os parâmetros do caminho
    /// da URI e da lista de argumentos depois da interrogação (?), como em:
    /// -   host.com/Caminho/Tal?arg1={parametro}...
    /// 
    /// O template de URI tem a forma: /Rota/{parametro}/...?chave={parametro}
    /// Cada parametro na forma "{parametro}" é extraído e estocado na coleção.
    /// </summary>
    /// <param name="template">O aplicador de template.</param>
    /// <param name="requestUri">A URI de requisição.</param>
    private HashMap ParseArguments(Template template, string requestUri)
    {
      var args = new HashMap();

      var tokens = requestUri.Split('?');
      var path = tokens.First();
      var queryString = ParseQueryString(string.Join("?", tokens.Skip(1)));

      var match = template.PathRegex.Match(path);
      for (int i = 0; i < match.Groups.Count - 1; i++)
      {
        var name = template.PathTargets[i];
        var value = match.Groups[i + 1].Value;
        args[name] = value;
      }

      for (int i = 0; i < template.QuerySources.Length; i++)
      { 
        var sourceName = template.QuerySources[i];
        var targetName = template.QueryTargets[i];
        var value = queryString[sourceName];
        args[targetName] = value;
      }

      return args;
    }

    /// <summary>
    /// Interpreta os argumentos da URI de requisição.
    /// </summary>
    /// <param name="queryString">Os argumentos de requisição.</param>
    /// <returns>O argumentos obtidos.</returns>
    private HashMap<string> ParseQueryString(string queryString)
    {
      if (queryString.StartsWith("?"))
      {
        queryString = queryString.Substring(1);
      }

      var args = new HashMap<string>();
      var tokens = queryString.Split('&');
      foreach (var token in tokens)
      {
        if (token.Contains('='))
        {
          var split = token.Split('=');
          var name = split.First();
          var value = string.Join("=", split.Skip(1));
          args[name] = value;
        }
        else
        {
          args[token] = "1";
        }
      }
      return args;
    }
  }
}