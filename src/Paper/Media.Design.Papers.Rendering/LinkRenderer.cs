using System.Linq;
using Paper.Media.Design;
using Paper.Media.Design.Fluid;
using Paper.Media.Design.Queries;
using Paper.Media.Rendering.Utilities;
using Toolset.Reflection;

namespace Paper.Media.Rendering.Queries
{
  static class LinkRenderer
  {
    public static void RenderLinks(RenderContext ctx)
    {
      if (!ctx.Query.HasMethod<Links>("GetLinks", typeof(object)))
        return;

      var links = ctx.Query.Call<Links>("GetLinks");
      if (links == null)
        return;

      if (ctx.Entity.Links == null)
      {
        ctx.Entity.Links = new LinkCollection();
      }

      foreach (var item in links.OfType<HrefLink>())
      {
        var link = CreateLink(item, ctx);
        if (link != null)
        {
          var hasSelf = (link.Rel?.Any(rel => rel == KnownRelations.Self) == true);
          if (!hasSelf)
          {
            link.AddClass("entityLink");
            link.AddRel("link");
          }

          if (link.Href != null)
          {
            ctx.Entity.Links.Add(link);
          }
        }
      }
      foreach (var item in links.OfType<QueryLink>())
      {
        var link = CreateLink(item, ctx);
        if (link != null)
        {
          var hasSelf = (link.Rel?.Any(rel => rel == KnownRelations.Self) == true);
          if (!hasSelf)
          {
            link.AddClass("entityLink");
            link.AddRel("link");
          }

          if (link.Href != null)
          {
            ctx.Entity.Links.Add(link);
          }
        }
      }
    }

    public static void RenderDataLinks(RenderContext ctx, object data)
    {
      if (!ctx.Query.HasMethod<Links>("GetDataLinks"))
        return;

      var links = ctx.Query.Call<Links>("GetDataLinks", data);
      if (links == null)
        return;

      if (ctx.Entity.Links == null)
      {
        ctx.Entity.Links = new LinkCollection();
      }

      foreach (var item in links.OfType<HrefLink>())
      {
        var link = CreateLink(item, ctx);
        if (link != null)
        {
          var hasSelf = (link.Rel?.Any(rel => rel == KnownRelations.Self) == true);
          if (!hasSelf)
          {
            link.AddClass("dataLink");
            link.AddRel("link");
          }

          if (link.Href != null)
          {
            ctx.Entity.Links.Add(link);
          }
        }
      }
      foreach (var item in links.OfType<QueryLink>())
      {
        var link = CreateLink(item, ctx);
        if (link != null)
        {
          var hasSelf = (link.Rel?.Any(rel => rel == KnownRelations.Self) == true);
          if (!hasSelf)
          {
            link.AddClass("dataLink");
            link.AddRel("link");
          }

          if (link.Href != null)
          {
            ctx.Entity.Links.Add(link);
          }
        }
      }
    }

    public static void RenderRowLinks(RenderContext ctx, Entity rowEntity, object row)
    {
      if (!ctx.Query.HasMethod<Links>("GetRowLinks"))
        return;

      var links = ctx.Query.Call<Links>("GetRowLinks", row);
      if (links == null)
        return;

      if (rowEntity.Links == null)
      {
        rowEntity.Links = new LinkCollection();
      }

      foreach (var item in links.OfType<HrefLink>())
      {
        var link = CreateLink(item, ctx);
        if (link != null)
        {
          var hasSelf = (link.Rel?.Any(rel => rel == KnownRelations.Self) == true);
          if (!hasSelf)
          {
            link.AddClass("rowLink");
            link.AddRel("link");
          }

          if (link.Href != null)
          {
            rowEntity.Links.Add(link);
          }
        }
      }

      foreach (var item in links.OfType<QueryLink>())
      {
        var link = CreateLink(item, ctx);
        if (link != null)
        {
          var hasSelf = (link.Rel?.Any(rel => rel == KnownRelations.Self) == true);
          if (!hasSelf)
          {
            link.AddClass("rowLink");
            link.AddRel("link");
          }

          if (link.Href != null)
          {
            rowEntity.Links.Add(link);
          }
        }
      }
    }

    private static Link CreateLink(HrefLink source, RenderContext ctx)
    {
      var link = new Link();
      link.CopyFrom(source);
      link.Href = UriUtil.ApplyUriTemplate(link.Href, ctx.HttpContext);
      return link;
    }

    private static Link CreateLink(QueryLink source, RenderContext ctx)
    {
      var linkedQuery = ctx.Injector.CreateInstance(source.QueryType);
      source.Setter?.Invoke(linkedQuery);
      
      var targets =
        from renderer
          in ctx.PaperRendererRegistry.FindPaperRenderers(source.QueryType)
        where renderer.PaperRendererType == typeof(QueryRenderer)
        let argNames = UriUtil.GetArgNames(renderer.PathTemplate)
        orderby argNames.Length descending
        select new { renderer, argNames };

      foreach (var target in targets)
      {
        var pathTemplate = target.renderer.PathTemplate;
        var argNames = target.argNames;

        var pathArgs = QueryPathBuilder.SatisfyArgs(linkedQuery, argNames);

        var isAllArgsSatisfied = !pathArgs.Any(x => x.Value == null);
        if (isAllArgsSatisfied)
        {
          var href = QueryPathBuilder.BuildPath(ctx, linkedQuery, pathTemplate, pathArgs, null);
          var link = new Link().CopyFrom(source);
          link.Href = href;
          return link;
        }
      }

      return null;
    }
  }
}