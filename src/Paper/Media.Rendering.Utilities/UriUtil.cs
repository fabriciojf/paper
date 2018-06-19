using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Toolset;
using Toolset.Collections;
using Toolset.Reflection;

namespace Paper.Media.Rendering.Utilities
{
  public static class UriUtil
  {
    public const string Input = "in";
    public const string Ouput = "out";
    public const string AlternateOuput = "f";
    public const string Help = "help";
    public const string Sort = "sort";
    public const string Limit = "limit";
    public const string Offset = "offset";

    public static readonly string[] AllReservedArgs =
      { Input, Ouput, AlternateOuput, Help, Sort, Limit, Offset };

    #region Uri Template

    public static string ApplyUriTemplate(string href, HttpContext httpContext)
    {
      if (href?.Contains("{") == true)
      {
        var req = httpContext.Request;

        if (href.Contains("{Api}"))
        {
          href = href.Replace("{Api}", "{Scheme}://{Host}{PathBase}");
          href += "{QueryModifiers}";
        }

        href = href
          .Replace("{Scheme}", req.Scheme)
          .Replace("{Host}", req.Host.Value)
          .Replace("{PathBase}", req.PathBase.Value)
          .Replace("{Path}", req.Path.Value)
          .Replace("{QueryString}", req.QueryString.Value)
          ;

        if (href.Contains("{QueryModifiers}"))
        {
          var modifierNames = new[] { "f" };
          var modifiers = (
            from name in modifierNames
            where req.Query.ContainsKey(name)
            select name + "=" + req.Query[name]
          ).ToArray();

          var queryString =
            modifiers.Any()
              ? (href.Contains("?") ? "&" : "?") + string.Join("&", modifiers)
              : null;

          href = href.Replace("{QueryModifiers}", queryString);
        }
      }
      return href;
    }

    #endregion

    #region Path Args

    public static string[] GetArgNames(string templatePath)
    {
      var keys = (
        from m in Regex.Matches(templatePath, @"\{([^{]+)\}").Cast<Match>()
        select m.Groups[1].Value
      ).ToArray();
      return keys;
    }

    public static IDictionary<string, object> GetPathArgs(HttpContext httpContext, string pathTemplate)
    {
      var cache = httpContext.GetCache();
      var args = cache["PathArgs"] as IDictionary<string, object>;
      if (args == null)
      {
        cache["PathArgs"] = args = ParsePath(pathTemplate, httpContext.Request.Path);
      }
      return args;
    }

    public static IDictionary<string, object> ParsePath(string templatePath, string requestPath)
    {
      var args = new HashMap();

      requestPath = requestPath.Split('?').First();

      var keys = GetArgNames(templatePath);
      var regex = new Regex(Regex.Replace(templatePath, @"\{[^{]+\}", "(.*)"));
      var match = regex.Match(requestPath);
      if (match.Success)
      {
        var keyEnumerator = keys.Cast<string>().GetEnumerator();
        var groupEnumerator = match.Groups.Cast<Group>().Skip(1).GetEnumerator();
        while (keyEnumerator.MoveNext() && groupEnumerator.MoveNext())
        {
          var key = keyEnumerator.Current;
          var text = groupEnumerator.Current.Value;
          var value = CreateValue(text);
          args[key] = value;
        }
      }
      else
      {
        keys.ForEach(key => args[key] = null);
      }
      return args;
    }

    public static string BuildPath(string templatePath, IEnumerable<KeyValuePair<string, object>> args)
    {
      var path = templatePath;
      foreach (var arg in args)
      {
        var key = $"{{{arg.Key}}}";
        var value = CreateText(arg.Value);
        path = path.Replace(key, value);
      }
      return path;
    }

    #endregion

    #region Query String

    public static IDictionary<string, object> GetQueryArgs(HttpContext httpContext)
    {
      var cache = httpContext.GetCache();
      var args = cache["QueryArgs"] as IDictionary<string, object>;
      if (args == null)
      {
        var queryString = httpContext.Request.QueryString.Value;
        cache["QueryArgs"] = args = ParseQueryString(queryString);
      }
      return args;
    }

    public static IDictionary<string, object> ParseQueryString(string queryString)
    {
      var cache = new HashMap();

      if (queryString == null)
        return cache;

      queryString = queryString.Split('?').Last();
      var tokens = queryString.Split('&');
      foreach (var token in tokens)
      {
        var index = token.IndexOf('=');

        var key = (index >= 0) ? token.Substring(0, index) : token;
        var text = (index >= 0) ? token.Substring(index + 1) : "true";

        var value = CreateValue(text);

        //  Arrays:
        //  -   Arrays sao criados com repeticao da chave.
        //      Por exemplo, a string:
        //          "?id=10" forma o inteiro: id=10
        //  -   Mas a string:
        //          "?id=10&id=20" forma o array id=[10,20]
        //  -   Para forçar um array a chave pode ser sufixada com "[]".
        //      Por exemplo, a string:
        //          "id[]=10" forma o array id=[10]
        //  Ranges:
        //  -   Ranges são intervalos em três formas possíveis:
        //      -   De X a Y, na forma: { Min=X, Max=Y }
        //      -   De X em diante, na forma: { Min=X }
        //      -   Até Y, na forma: { Max=Y }
        //  -   Na URI são indicados como, respectivamente:
        //      -   ?tal.min=X&tal.min=Y
        //      -   ?tal.min=X
        //      -   ?tal.min=Y
        bool isArray = key.Contains("[]") || cache.ContainsKey(key);
        bool isRange = key.Contains(".min") || key.Contains(".max");

        if (isArray)
        {
          key = key.Replace("[]", "");
          var list = cache[key] as object[] ?? new[] { cache[key] };
          value = list.Append(value).NonNull().ToArray();
        }
        else if (isRange)
        {
          var isMin = key.Contains(".min");
          key = key.Replace(".min", "").Replace(".max", "");
          var range = cache[key] as Range ?? new Range();
          if (isMin)
          {
            range.Min = value;
          }
          else
          {
            range.Max = value;
          }
          value = range;
        }

        cache[key] = value;
      }

      var args = new HashMap(cache.Count);
      foreach (var item in cache)
      {
        var key = item.Key.ChangeCase(TextCase.CamelCase);
        args[key] = item.Value;
      }
      return args;
    }

    public static string ToQueryString(IEnumerable<KeyValuePair<string, object>> args)
    {
      if (args == null)
        return "";

      var keys = args.Select(x => x.Key);
      return ToQueryString(keys, key =>
        args.Where(e => e.Key == key).Select(e => e.Value).FirstOrDefault()
      );
    }

    public static string ToQueryString(IDictionary args)
    {
      if (args == null)
        return "";

      var keys = args.Keys.Cast<string>();
      return ToQueryString(keys, key => args[key]);
    }

    public static string ToQueryString(object graph)
    {
      if (graph == null)
        return "";

      var keys = graph.GetType().GetProperties().Select(x => x.Name);
      return ToQueryString(keys, key => graph.Get(key));
    }

    private static string ToQueryString(IEnumerable<string> keys, Func<string, object> valueSelector)
    {
      var items = Enumerable.Empty<string>();
      foreach (string key in keys)
      {
        var value = valueSelector.Invoke(key);
        var tokens = CreateTokens(key, value);
        items = items.Concat(tokens);
      }
      var queryString = string.Join("&", items);
      return (queryString == "") ? "" : $"?{queryString}";
    }

    private static IEnumerable<string> CreateTokens(string key, object value)
    {
      if (value == null)
        yield break;

      key = key.ChangeCase(TextCase.CamelCase);

      var isArray = !(value is string) && typeof(IEnumerable).IsAssignableFrom(value.GetType());
      var isRange = value.Has("Min") || value.Has("Max");

      if (isArray)
      {
        var items = ((IEnumerable)value).Cast<object>();
        var texts = items.Select(CreateText).NonEmpty();
        foreach (var text in texts)
        {
          yield return $"{key}[]={text}";
        }
        yield break;
      }
      else if (isRange)
      {
        var min = value.Get("Min");
        var max = value.Get("Max");
        if (min != null)
        {
          var text = CreateText(min);
          yield return $"{key}.min={text}";
        }
        if (max != null)
        {
          var text = CreateText(max);
          yield return $"{key}.max={text}";
        }
      }
      else
      {
        var text = CreateText(value);
        if (!string.IsNullOrEmpty(text))
        {
          yield return $"{key}={text}";
        }
      }
    }

    public class Range
    {
      public object Min { get; set; }
      public object Max { get; set; }
    }

    #endregion

    #region Castings

    public static object CreateValue(string text)
    {
      if (string.IsNullOrEmpty(text))
        return null;

      if (text.EqualsIgnoreCase("true") || text.EqualsIgnoreCase("false"))
        return bool.Parse(text.ToLower());

      if (Regex.IsMatch(text, "^-?[0-9]+$"))
        return int.Parse(text);

      if (Regex.IsMatch(text, "^-?[0-9]+[.,][0-9]+$"))
        return decimal.Parse(text.Replace(",", "."), CultureInfo.InvariantCulture);

      if (Regex.IsMatch(text, "^[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}.*")
       || Regex.IsMatch(text, "^[0-9]{4}-[0-9]{2}-[0-9]{2}$"))
        return DateTime.Parse(text);

      if (Regex.IsMatch(text, "^([0-9]+[.])?[0-9]{2}:[0-9]{2}.*"))
        return TimeSpan.Parse(text);

      return text;
    }

    public static string CreateText(object value)
    {
      if (value?.Has("Value") == true)
        value = value.Get("Value");

      if (value == null)
        return null;

      if (value is string)
        return (string)value;

      if (value is bool)
        return value.Equals(true) ? "true" : "false";

      if (value is DateTime)
        return ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:sszzz");

      if (value is IFormattable)
        return ((IFormattable)value).ToString(null, CultureInfo.InvariantCulture);

      var text = value.ToString();
      return (text != "") ? text : null;
    }

    #endregion

  }
}