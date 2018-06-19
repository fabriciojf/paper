using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Paper.Media.Design;
using Paper.Media.Rendering.Utilities;
using Toolset;
using Toolset.Reflection;
using static Paper.Media.Design.Sort;

namespace Paper.Media.Rendering.Queries
{
  internal static class QueryPathBuilder
  {
    public static string BuildPath(
      RenderContext ctx, object query, string pathTemplate,
      IEnumerable<KeyValuePair<string, object>> pathArgs = null,
      IEnumerable<KeyValuePair<string, object>> filterArgs = null)
    {
      if (!pathTemplate.StartsWith("/"))
      {
        pathTemplate = $"/{pathTemplate}";
      }

      if (pathArgs == null)
      {
        pathArgs = SatisfyArgs(query, ctx.PathArgs.Keys);
      }

      if (filterArgs == null)
      {
        var filter = query.Get("Filter");
        if (filter != null)
        {
          var type = filter.GetType();
          var filterArgNames = type.GetProperties().Select(p => p.Name).ToArray();
          filterArgs = SatisfyArgs(query, filterArgNames);
        }
        else
        {
          filterArgs = new KeyValuePair<string, object>[0];
        }
      }

      var pagination = query.Get<Pagination>("Pagination");
      if (pagination != null)
      {
        filterArgs =
          new[] {
            new KeyValuePair<string, object>("offset", pagination.Offset),
            new KeyValuePair<string, object>("limit", pagination.Limit)
          }.Concat(filterArgs);
      }

      var sort = query.Get<Sort>("Sort");
      if (sort != null)
      {
        var fields = (
          from field in sort.SortedFields
          let key = field.FieldName.ChangeCase(TextCase.CamelCase)
          select (field.Order == Order.Descending) ? $"{key}:desc" : key
        ).ToArray();

        filterArgs = filterArgs.Append(new KeyValuePair<string, object>("sort", fields));
      }

      var root = GetUriPrefix(ctx.HttpContext.Request);
      var path = UriUtil.BuildPath(pathTemplate, pathArgs);
      var queryString = UriUtil.ToQueryString(filterArgs);
      path = $"{root}{path}{queryString}";
      return path;
    }

    public static string GetUriPrefix(HttpRequest request)
    {
      string uri = "";

      if (request.Scheme != null)
        uri = string.Concat(uri, request.Scheme, "://");

      if (request.Host.HasValue)
        uri = string.Concat(uri, request.Host.ToUriComponent());

      uri = string.Concat(
        uri,
        request.PathBase.ToUriComponent()
      );

      return uri;
    }

    public static KeyValuePair<string, object>[] SatisfyArgs(object query, IEnumerable<string> argNames)
    {
      var args = new KeyValuePair<string, object>[argNames.Count()];
      for (var i = 0; i < argNames.Count(); i++)
      {
        string argName = argNames.ElementAt(i);
        object argValue = query.Get(argName) ?? query.Get("Filter")?.Get(argName);
        args[i] = KeyValuePair.Create(argName, argValue);
      }
      return args;
    }
  }
}