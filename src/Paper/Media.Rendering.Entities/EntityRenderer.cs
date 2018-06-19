using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Paper.Media;
using System.Collections.Specialized;
using Toolset;
using Paper.Media.Design;
using Toolset.Collections;
using System.Collections;
using Paper.Media.Design.Queries;
using Microsoft.Extensions.DependencyInjection;
using Paper.Media.Rendering.Utilities;

namespace Paper.Media.Rendering.Entities
{
  [PaperRenderer(ContractName)]
  public class EntityRenderer : IPaperRenderer
  {
    public const string ContractName = "Entity";

    public Type PaperType { get; set; }

    public string PathTemplate { get; set; }

    private IServiceProvider injector;

    public EntityRenderer(IServiceProvider injector)
    {
      this.injector = injector;
    }

    public Ret<Entity> RenderEntity(HttpContext httpContext, string path)
    {
      var entity = (Entity)injector.CreateInstance(PaperType);

      ApplyDefaultClass(entity, httpContext);
      ApplyDefaultTitle(entity, httpContext);
      ApplyLinkTemplate(entity, httpContext);
      ApplySelfLink(entity, httpContext);

      return entity;
    }

    private void ApplyDefaultClass(Entity entity, HttpContext httpContext)
    {
      var entityClass = Conventions.MakeClassName(entity);
      if (entityClass != null)
      {
        if (entity.Class == null)
        {
          entity.Class = new NameCollection();
        }
        entity.Class.Add(entityClass);
      }
    }

    private void ApplyDefaultTitle(Entity entity, HttpContext httpContext)
    {
      if (entity.Title == null)
      {
        entity.Title =
          entity.GetType().Name
            .Replace("Entity", "")
            .ChangeCase(TextCase.ProperCase);
      }
    }

    private void ApplyLinkTemplate(Entity entity, HttpContext httpContext)
    {
      var linkTemplates =
        from e in EnumerateEntities(entity)
        where e.Links != null
        from link in e.Links
        where link.Href?.Contains("{") == true
        select link;

      foreach (var link in linkTemplates)
      {
        link.Href = UriUtil.ApplyUriTemplate(link.Href, httpContext);
      }
    }

    private void ApplySelfLink(Entity entity, HttpContext httpContext)
    {
      if (entity.Links == null)
        entity.Links = new LinkCollection();

      var hasSelf = entity.Links.Any(link =>
        link.Rel?.Contains(KnownRelations.Self) == true
      );
      if (!hasSelf)
      {
        entity.Links.AddSelf(httpContext.Request.GetRequestUri());
      }
    }

    private IEnumerable<Entity> EnumerateEntities(Entity entity)
    {
      yield return entity;
      if (entity.Entities != null)
      {
        foreach (var subEntity in entity.Entities.SelectMany(EnumerateEntities))
        {
          yield return subEntity;
        }
      }
    }

    public IEnumerable<KeyValuePair<string, object>> GetAllArgs(HttpContext httpContext)
    {
      return GetQueryArgs(httpContext).Concat(GetPathArgs(httpContext));
    }

    public IDictionary<string, object> GetQueryArgs(HttpContext httpContext)
    {
      var cache = httpContext.GetCache();
      var args = cache["QueryArgs"] as IDictionary<string, object>;
      if (args == null)
      {
        var queryString = httpContext.Request.QueryString.Value;
        cache["QueryArgs"] = args = UriUtil.ParseQueryString(queryString);
      }
      return args;
    }

    public IDictionary<string, object> GetPathArgs(HttpContext httpContext)
    {
      var cache = httpContext.GetCache();
      var args = cache["PathArgs"] as IDictionary<string, object>;
      if (args == null)
      {
        cache["PathArgs"] = args = UriUtil.ParsePath(PathTemplate, httpContext.Request.Path);
      }
      return args;
    }
  }
}
