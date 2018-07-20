using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Paper.Media.Design;
using Paper.Media.Design.Extensions;
using Toolset.Reflection;

namespace Media.Design.Extensions.Papers.Rendering
{
  static class RenderOfData
  {
    public static void Render(IPaper paper, Entity entity, PaperContext ctx)
    {
      // Os dados foram renderizados anteriormente e estocados no cache
      var data = ctx.Cache.Get<DataWrapper>(CacheKeys.Data);
      if (data == null)
        return;

      AddData(paper, entity, ctx, data);
      AddDataHeaders(paper, entity, ctx, data);
      AddDataLinks(paper, entity, ctx, data);
    }

    /// <summary>
    /// Renderizando dados e cabeçalhos básicos
    /// </summary>
    private static void AddData(IPaper paper, Entity entity, PaperContext ctx, DataWrapper data)
    {
      foreach (var key in data.EnumerateKeys())
      {
        var value = data.GetValue(key);
        entity.AddProperty(key, value);

        var header = data.GetHeader(key);
        entity.AddDataHeader(header);
      }
    }

    /// <summary>
    /// Renderizando personalizações nos cabeçalhos
    /// </summary>
    private static void AddDataHeaders(IPaper paper, Entity entity, PaperContext ctx, DataWrapper data)
    {
      var headers = paper._Call<IEnumerable<HeaderInfo>>("GetDataHeaders", data.DataSource);
      if (headers != null)
      {
        entity.AddDataHeaders(headers);
      }
    }

    /// <summary>
    /// Renderizando links
    /// </summary>
    private static void AddDataLinks(IPaper paper, Entity entity, PaperContext ctx, DataWrapper data)
    {
      var linkRenderers = paper._Call<IEnumerable<ILink>>("GetDataLinks", data.DataSource);
      if (linkRenderers != null)
      {
        foreach (var linkRenderer in linkRenderers)
        {
          var link = linkRenderer.RenderLink(ctx);
          if (link != null)
          {
            link.AddRel(RelNames.DataLink);
            link.Rel.Remove(RelNames.Link);
            entity.AddLink(link);
          }
        }
      }
    }
  }
}