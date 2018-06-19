using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Paper.Media;
using Toolset;
using Paper.Media.Design;
using Toolset.Collections;
using System.Collections;
using Paper.Media.Design.Queries;
using Microsoft.Extensions.DependencyInjection;
using Paper.Media.Rendering.Utilities;
using System.Net;
using Toolset.Reflection;
using System.Data;
using Paper.Media.Design.Widgets;
using System.Reflection;

namespace Paper.Media.Rendering.Queries
{
  [PaperRenderer(ContractName)]
  public class QueryRenderer : IPaperRenderer
  {
    public const string ContractName = "Query";

    public Type PaperType { get; set; }

    public string PathTemplate { get; set; }

    private readonly IServiceProvider injector;
    private readonly IPaperRendererRegistry registry;

    private static readonly MethodInfo[] SetChain;
    private static readonly MethodInfo[] RenderingChain;

    static QueryRenderer()
    {
      var chain = (
        from type in typeof(QueryRenderer).Assembly.GetTypes()
        where type.Namespace == typeof(QueryRenderer).Namespace
        where type.Name.StartsWith("RenderOf")
        from method in type.GetMethods()
        where typeof(RenderContext) == method.GetParameters().FirstOrDefault()?.ParameterType
        let name = method.Name
        let step = name.StartsWith("Set") ? 0 : !name.StartsWith("Pos") ? 1 : 2
        orderby step
        select new { method, step }
      ).ToArray();

      SetChain = chain.Where(x => x.step == 0).Select(x => x.method).ToArray();
      RenderingChain = chain.Where(x => x.step > 0).Select(x => x.method).ToArray();
    }

    public QueryRenderer(IServiceProvider injector, IPaperRendererRegistry registry)
    {
      this.injector = injector;
      this.registry = registry;
    }

    public Ret<Entity> RenderEntity(HttpContext httpContext, string path)
    {
      if (httpContext.Request.Method != "GET")
        return Ret.Fail(HttpStatusCode.MethodNotAllowed);

      var query = injector.CreateInstance(PaperType);
      try
      {
        var entity = new Entity();

        RenderEntity(httpContext, query, entity);

        return entity;
      }
      catch (Exception ex)
      {
        return ex;
      }
      finally
      {
        (query as IDisposable)?.Dispose();
      }
    }

    private void RenderEntity(HttpContext httpContext, object query, Entity entity)
    {
      var context = new RenderContext
      {
        PaperRendererRegistry = registry,
        Injector = injector,
        HttpContext = httpContext,
        PathTemplate = PathTemplate,
        Entity = entity,
        Query = query,
        PathArgs = UriUtil.GetPathArgs(httpContext, PathTemplate),
        QueryArgs = UriUtil.GetQueryArgs(httpContext)
      };

      var args = new object[] { context };

      // Atribuindo parametros do objeto
      foreach (var method in SetChain)
      {
        method.Invoke(null, args);
      }

      // Capturando dados
      var data = query.Call("GetData");
      var rows = query.Call("GetRows");
      if (data is IEnumerable) data = ((IEnumerable)data).Cast<object>().ToArray();
      if (rows is IEnumerable) rows = ((IEnumerable)rows).Cast<object>().ToArray();
      context.Data = data;
      context.Rows = rows;

      // Renderizando e pós-renderizando o objeto
      foreach (var method in RenderingChain)
      {
        method.Invoke(null, args);
      }
    }
  }
}
