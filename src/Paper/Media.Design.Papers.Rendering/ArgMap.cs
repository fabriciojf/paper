using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Toolset;
using Toolset.Collections;
using Toolset.Reflection;

namespace Paper.Media.Design.Papers.Rendering
{
  public class ArgMap : IEnumerable<KeyValuePair<string, object>>
  {
    private readonly Map<string, object> items = new Map<string, object>();

    public ICollection<string> Keys => items.Keys;

    public object this[string key] => Get(key);

    public object Get(string key)
    {
      object value = this;
      foreach (var token in key.Split('.'))
      {
        if (value == null)
          break;

        if (value is ArgMap)
          value = ((ArgMap)value).items[token];
        else
          value = value?._Get(token);
      }
      return value;
    }

    public T Get<T>(string key)
    {
      var value = Get(key);
      var convertedValue = Change.To<T>(value);
      return convertedValue;
    }

    public bool Contains(string key)
    {
      if (items.ContainsKey(key))
        return true;

      object value = this;
      foreach (var token in key.Split('.'))
      {
        if (value == null)
          break;

        if (value is ArgMap)
          value = ((ArgMap)value).items[token];
        else
          value = value?._Get(token);
      }
      return value != null;
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
      return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return items.GetEnumerator();
    }

    /// <summary>
    /// Extrai argumentos da URI indicada.
    /// </summary>
    /// <param name="uri">A URI a ser analisada.</param>
    /// <param name="uriTemplate">
    /// Um template opcional para extração do caminho da URI.
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
    /// </param>
    /// <param name="pathPrefix">
    /// Um prefixo opcional para o caminho analisado.
    /// -   O prefixo é acrescentado no início do template.
    /// -   Como parte do template, todas as normas de extração de argumentos do template
    ///     se aplicam também ao prefixo.
    /// -   O curinga "*" pode ser usado para indicar qualquer texto na posição.
    /// 
    /// Exemplo:
    /// -   Considerando a URI:
    ///     -   http://host.com/web/api/1/usuarios/17/posts
    /// -   O caminho analisado será:
    ///     -   /web/api/1/usuarios/17/posts
    /// -   Se aplicado o template:
    ///     -   /usuarios/{id}/posts
    /// -   Nada será encontrado, porque o template não corresponde ao caminho analisado.
    /// -   Porém, se aplicado o prefixo:
    ///     -   /*/api/1
    /// -   O caminho analisado passará a ser:
    ///     -   /usuarios/17/posts
    /// -   O id do usuário será encontrado, porque agora o template corresponde ao caminho analisado.
    /// </param>
    /// <returns></returns>
    public static ArgMap ParseFromUri(string uri, string uriTemplate = null, string pathPrefix = "/")
    {
      var tokens = uri.Split('?');

      var path = new Route(tokens.First()).MakeRelative("/");
      var queryString = uri.Split('?').Skip(1).LastOrDefault();

      var map = new ArgMap();

      ParseQueryString(map, queryString);
      ParsePath(map, path, pathPrefix, uriTemplate);

      return map;
    }

    private static void ParsePath(ArgMap map, string path, string pathPrefix, string uriTemplate)
    {
      if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(uriTemplate))
        return;

      pathPrefix = pathPrefix.Replace(@"*", @"[^/]+");

      while (pathPrefix.EndsWith("/"))
        pathPrefix = pathPrefix.Substring(0, pathPrefix.Length - 1);

      if (!pathPrefix.StartsWith("/"))
        pathPrefix = "/" + pathPrefix;

      if (!uriTemplate.StartsWith("/"))
        uriTemplate = "/" + uriTemplate;

      var pattern = $"^{pathPrefix}{uriTemplate}";

      var options = RegexOptions.IgnoreCase;

      var keyPattern = new Regex(@"\{([^{}]+)\}", options);
      var uriPattern = new Regex(keyPattern.Replace(pattern, @"([\w\d._]+)"), options);

      var keyMatches = keyPattern.Matches(pattern);
      var uriMatch = uriPattern.Match(path);
      for (var i = 0; i < keyMatches.Count; i++)
      {
        Match keyMatch = keyMatches[i];

        var key = keyMatch.Groups[1].Value;
        var value = uriMatch.Success ? uriMatch.Groups[i + 1].Value : null;

        ParseArg(map, key, value);
      }
    }

    private static void ParseQueryString(ArgMap map, string queryString)
    {
      if (string.IsNullOrEmpty(queryString))
        return;

      var entries =
        from token in queryString.Split('&')
        let parts = token.Split('=')
        let key = parts.First()
        let value = string.Join('=', parts.Skip(1))
        where !string.IsNullOrEmpty(value)
        select new { key, value };

      foreach (var entry in entries)
      {
        ParseArg(map, entry.key, entry.value);
      }
    }

    private static void ParseArg(ArgMap map, string key, string value)
    {
      var isList = key.EndsWith("[]");
      if (isList)
      {
        key = key.Substring(0, key.Length - 2);
      }

      var isMin = key.EndsWithIgnoreCase(".min");
      var isMax = key.EndsWithIgnoreCase(".max");
      var isRange = isMin || isMax;
      if (isRange)
      {
        key = key.Substring(0, key.Length - 4);
      }

      var tokens = key.Split('.');
      var parents = tokens.Take(tokens.Length - 1);
      var current = tokens.Last();

      foreach (var parent in parents)
      {
        if (!(map.items[parent] is ArgMap))
        {
          map.items[parent] = new ArgMap();
        }
        map = (ArgMap)map.items[parent];
      }

      if (isRange)
      {
        var range = map.items[current] as Range;
        var min = isMin ? value : range?.Min;
        var max = isMax ? value : range?.Max;
        map.items[current] = new Range(min, max);
      }
      else if (isList)
      {
        var list = map.items[current] as List<object>;
        if (list == null)
        {
          map.items[current] = (list = new List<object>());
        }
        if (value != null)
        {
          list.Add(value);
        }
      }
      else
      {
        map.items[current] = value;
      }
    }
  }
}