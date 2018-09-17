using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Paper.Media.Design;
using Toolset;
using Toolset.Reflection;

namespace Paper.Media.Rendering
{
  public class PaperSpec
  {
    public Type Type { get; private set; }
    public string Route { get; private set; }
    public string UriTemplate { get; private set; }

    private PaperSpec()
    {
    }

    public static PaperSpec GetSpec<T>()
    {
      return GetSpec(typeof(T));
    }

    public static PaperSpec GetSpec(Type paperType)
    {
      var spec = new PaperSpec();
      spec.Type = paperType;
      spec.Route = GetRoute(paperType);
      spec.UriTemplate = GetUriTemplate(spec.Route, paperType);
      return spec;
    }

    private static string GetRoute(Type paperType)
    {
      var attr = paperType.GetCustomAttributes(true).OfType<PaperAttribute>().FirstOrDefault();
      var route = attr?.Route;
      if (route == null)
      {
        route = paperType.FullName
          .ReplacePattern("(Paper|Entity)$", "")
          .Replace(".", "/")
          .Replace("+", ".")
          .ReplacePattern(@"[^\w\d/.]", "_");
      }
      // Sanitizando a rota
      route = "/" + string.Join("/", route.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
      return route;
    }

    private static string GetUriTemplate(string route, Type paperType)
    {
      // Futura implementação: Os argumentos, como "{id}", serão inferidos.
      //      // Chaves multi-valor são representados no padrão "{key1}:{key2}"
      //      // conforme especificado por Design.Rendering.UriTemplate.
      //      var keys =
      //        from name in paperType._GetPropertyNames()
      //        select $"{{{name.ChangeCase(TextCase.CamelCase)}}}";
      //      var suffix = string.Join(":", keys);
      //      return (suffix.Length > 0) ? $"{route}/{suffix}" : route;

      // Implementação atual: Os argumentos, como "{id}", precisam ser explicitamente
      // definidos na rota.
      return route;
    }
  }
}