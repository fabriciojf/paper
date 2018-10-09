using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Toolset;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
using Toolset.Xml;

namespace Inkeeper.RestApi
{
  public class RestMiddleware
  {
    class Spec
    {
      public string Method { get; set; }
      public Regex PathRegex { get; set; }
      public string[] PathTargets { get; set; }
      public string[] QuerySources { get; set; }
      public string[] QueryTargets { get; set; }
      public MethodInfo Callee { get; set; }
    }

    class Parameter
    {
      public ParameterInfo Info { get; set; }
      public object Value { get; set; }

      public override string ToString()
      {
        return Info?.Name;
      }
    }
    
    private readonly string route;
    private readonly Type serviceType;
    private readonly RequestDelegate next;

    private readonly List<Spec> routes;

    public RestMiddleware(RequestDelegate next, string route, Type serviceType)
    {
      this.next = next;
      this.route = route;
      this.serviceType = serviceType;

      this.routes = new List<Spec>();

      var specs =
        from method in serviceType.GetMethods()
        from attr in method.GetCustomAttributes(true).OfType<WebMethodAttribute>()
        select new { method, attr };
      foreach (var spec in specs)
      {
        MatchCollection matches;

        var methodRoute = spec.attr.GetRouteForMethod(spec.method);
        var tokens = methodRoute.Split('?');
        var path = tokens.FirstOrDefault();
        var queryString = tokens.Skip(1).FirstOrDefault() ?? "";

        var pattern = Regex.Escape(Regex.Replace(path, @"\{[^{}]*\}", "§"));
        var pathPattern = $"^{pattern.Replace("§", @"([^/?]+)")}$";
        var pathArgNamesPattern = @"\{([^/?]+)\}$";
        matches = Regex.Matches(path, pathArgNamesPattern);
        var pathArgNames = (
          from m in matches
          select m.Groups[1].Value
        ).ToArray();

        matches = Regex.Matches(queryString, @"([^?&=]+)=\{([^?&=]+)\}");
        var queryArgNames = (
          from m in matches
          select new { source = m.Groups[1].Value, target = m.Groups[2].Value }
        ).ToArray();

        this.routes.Add(new Spec
        {
          Callee = spec.method,
          Method = spec.attr.Method,
          PathRegex = new Regex(pathPattern),
          PathTargets = pathArgNames,
          QuerySources = queryArgNames.Select(x => x.source).ToArray(),
          QueryTargets = queryArgNames.Select(x => x.target).ToArray()
        });
      }
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider provider)
    {
      var route = context.Request.Path;
      var spec = routes.FirstOrDefault(x => x.PathRegex.IsMatch(route));

      if (spec == null || !spec.Method.EqualsIgnoreCase(context.Request.Method))
      {
        await next(context);
        return;
      }

      var instance = ActivatorUtilities.CreateInstance(provider, serviceType);
      var parameters = CreateParameters(spec.Callee.GetParameters(), provider, context).ToArray();
      SetParsedParameters(spec, parameters, context.Request.Path, context.Request.Query);

      var calleeArgs = parameters.Select(x => x.Value).ToArray();
      var result = spec.Callee.Invoke(instance, calleeArgs);

      await WriteResultAsync(context, result);
    }

    /// <summary>
    /// Formata a resposta e envia para o chamador de forma assíncrona.
    /// </summary>
    /// <param name="context">O contexto HTTP.</param>
    /// <param name="data">O dado a ser enviado.</param>
    private async Task WriteResultAsync(HttpContext context, object data)
    {
      context.Response.ContentType = "application/xml; charset=UTF-8";

      if (data == null)
      {
        return;
      }

      if (data is Stream stream)
      {
        using (stream)
        {
          await stream.CopyToAsync(context.Response.Body);
          return;
        }
      }

      if (data is string text)
      {
        await context.Response.WriteAsync(text);
        return;
      }

      using (stream = data.ToXmlStream())
      {
        await stream.CopyToAsync(context.Response.Body);
        return;
      }
    }

    /// <summary>
    /// Constrói a lista de valores para os argumentos resolvendo tipos
    /// a partir dos objetos em cache e da lista provida pelo chamador.
    /// </summary>
    /// <param name="parameters">Lista dos argumentos.</param>
    /// <param name="provider">Provedor de serviços injetáveis.</param>
    /// <param name="providedArgs">Objetos providos pelo chamador.</param>
    /// <returns>Lista de valores criados para os argumentos.</returns>
    private IEnumerable<Parameter> CreateParameters(ParameterInfo[] parameters, IServiceProvider provider, params object[] providedArgs)
    {
      var availables = new List<object>(providedArgs);
      foreach (var parameter in parameters)
      {
        var type = parameter.ParameterType;

        object provided;

        //
        // Tipos exatos
        //

        provided = availables.FirstOrDefault(x => x.GetType() == type);
        if (provided != null)
        {
          availables.Remove(provided);
          yield return new Parameter
          {
            Info = parameter,
            Value = provided
          };
          continue;
        }

        provided = availables.FirstOrDefault(x => type.IsAssignableFrom(x.GetType()));
        if (provided != null)
        {
          availables.Remove(provided);
          yield return new Parameter
          {
            Info = parameter,
            Value = provided
          };
          continue;
        }

        var resolvedType = provider.GetService(type);
        if (resolvedType != null)
        {
          yield return new Parameter
          {
            Info = parameter,
            Value = resolvedType
          };
          continue;
        }

        yield return new Parameter
        {
          Info = parameter,
          Value = type.IsValueType ? Activator.CreateInstance(type) : null
        };
      }

      // Se o consumo dos parâmetros for obrigatório este código por ser usado:
      // if (availables.Any())
      // {
      //   throw new Exception("Não existe construtor que satisfaça todos os argumentos indicados.");
      // }
    }

    /// <summary>
    /// Extrai parâmetros da URI e define na lista de parâmetros indicada.
    /// </summary>
    /// <param name="spec">A especificação da rota.</param>
    /// <param name="parameters">A lista de parâmetros destino.</param>
    /// <param name="path">O caminho da URI.</param>
    /// <param name="query">Os argumentos da URI.</param>
    private void SetParsedParameters(Spec spec, Parameter[] parameters, string path, IQueryCollection query)
    {
      if (spec.PathTargets != null)
      {
        var match = spec.PathRegex.Match(path);
        for (int i = 0; i < match.Groups.Count - 1; i++)
        {
          var argName = spec.PathTargets[i];

          var parameter = parameters.FirstOrDefault(x => x.Info.Name.EqualsIgnoreCase(argName));
          if (parameter == null)
          {
            throw new NullReferenceException("Argumento não declarado na URI: " + argName);
          }

          var value = match.Groups[i + 1].Value;
          parameter.Value = Change.To(value, parameter.Info.ParameterType);
        }
      }

      if (spec.QuerySources != null && spec.QueryTargets != null)
      {
        for (int i = 0; i < spec.QuerySources.Length; i++)
        {
          var sourceArg = spec.QuerySources[i];
          var targetArg = spec.QueryTargets[i];

          var parameter = parameters.FirstOrDefault(
            x => targetArg.EqualsIgnoreCase(x.Info.Name)
          );

          if (parameter == null)
          {
            throw new NullReferenceException("Argumento não declarado na URI: " + sourceArg);
          }

          var value = query[sourceArg];

          parameter.Value = Change.To(value, parameter.Info.ParameterType);
        }
      }
    }
  }
}