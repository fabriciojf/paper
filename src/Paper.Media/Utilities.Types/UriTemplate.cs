using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Toolset.Reflection;

namespace Media.Utilities.Types
{
  /// <summary>
  /// Utilitário de processamento de template de URI
  /// 
  /// Template
  /// --------
  /// O template é um padrão de extração e construção de URI.
  /// A comparação é insensível a caso.
  /// 
  /// Na forma:
  /// -   {argumento}
  /// Exemplo:
  /// -   /Empresas/{idEmpresa}/Usuarios/{idUsuario}
  /// 
  /// Construções especiais são suportadas:
  /// 
  /// -   {argumento[]}
  ///         Para construção de arrays.
  ///         Exemplo:
  ///         -   /Usuarios/{id[]}/Posts
  ///         Uso:
  ///         -   /Usuarios/1,2,3,4,5/Posts
  ///         
  /// -   {argumento.min} e {argumento.max}
  ///         Para construção de períodos
  ///         Exemplo:
  ///         -   /Usuarios/{id.min}:{id.max}/Posts
  ///         Uso:
  ///         -   /Usuarios/5:15/Posts
  ///         
  /// -   {argumento.propriedade}
  ///         Para construção de objetos.
  ///         Exemplo:
  ///         -   /Usuarios/{post.idUsuario}/Posts/{post.idPost}
  ///         Uso:
  ///         -   /Usuarios/17/Posts/35
  /// </summary>
  public class UriTemplate
  {
    public string Template { get; }

    private Regex keyPattern;
    private Regex uriPattern;

    private readonly List<string> tokens = new List<string>();
    private readonly ArgMap args = new ArgMap();

    public UriTemplate(string template)
    {
      this.Template = template;
      this.keyPattern = new Regex(@"\{([^{}]+)\}", RegexOptions.IgnoreCase);
      this.uriPattern = new Regex(keyPattern.Replace(template, @"([\w\d._]+)"), RegexOptions.IgnoreCase);

      ParseTemplate();
    }

    private void ParseTemplate()
    {
      var matches = keyPattern.Matches(Template);
      foreach (Match match in matches)
      {
        var key = match.Groups[1].Value;
        tokens.Add(key);
        args[key] = null;
      }
    }

    public void SetArg(string key, object value)
    {
      if (!args.Contains(key))
        throw new Exception($"A chave '{key}' não está mapeada no template: {this.Template}");

      args[key] = value;
    }

    public void SetArgsFrom(IEnumerable<KeyValuePair<string, object>> items)
    {
      foreach (var item in items)
      {
        if (args.Contains(item.Key))
        {
          args[item.Key] = item.Value;
        }
      }
    }

    public void SetArgsFromGraph(object graph)
    {
      foreach (var key in args.Keys.ToArray())
      {
        if (graph._Has(key))
        {
          var value = graph._Get(key);
          args[key] = value;
        }
      }
    }

    public void SetArgsFromUri(string uri)
    {
      var tokens = uri.Split('?');
      var path = tokens.First();
      var queryString = tokens.Skip(1).LastOrDefault();
      SetArgsFromPath(path);
      SetArgsFromQueryString(queryString);
    }

    private void SetArgsFromPath(string path)
    {
      if (path == null)
        return;

      var uriMatch = uriPattern.Match(path);
      if (!uriMatch.Success)
        return;

      int index = 1;
      foreach (var key in args.Keys.ToArray())
      {
        if (index >= uriMatch.Groups.Count)
          return;

        var value = uriMatch.Groups[index++].Value;
        args[key] = value;
      }
    }

    private void SetArgsFromQueryString(string queryString)
    {
      if (string.IsNullOrEmpty(queryString))
        return;

      var entries =
        from token in queryString.Split('&')
        let parts = token.Split('=')
        let key = parts.First()
        let value = string.Join("=", parts.Skip(1))
        where !string.IsNullOrEmpty(value)
        select new { key, value };

      foreach (var entry in entries)
      {
        args[entry.key] = entry.value;
      }
    }

    public ArgMap CreateArgs() => new ArgMap(args);

    public string CreateUri()
    {
      var uri = this.Template;
      foreach (var token in tokens)
      {
        var value = args[token];

        if (value == null)
          throw new Exception($"O parâmetro {token} não foi definido. Não é possível resolver o template de URI: {Template}");

        uri = uri.Replace($"{{{token}}}", value.ToString());
      }
      return uri;
    }

    public override string ToString()
    {
      var uri = this.Template;
      foreach (var token in tokens)
      {
        var key = $"{{{token}}}";
        var value = args[token];
        uri = uri.Replace($"{{{token}}}", $"{{{token}={value ?? "null"}}}");
      }
      return $"{{{uri}}}";
    }
  }
}
