using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Paper.Media.Design;
using Paper.Media.Design.Extensions;
using Toolset.Reflection;

namespace Paper.Media.Papers.Rendering
{
  static class RenderOfRows
  {
    public static void Render(IPaper paper, Entity entity, PaperContext ctx)
    {
      // Os dados foram renderizados anteriormente e estocados no cache
      var rows = ctx.Cache.Get<DataWrapperEnumerable>(CacheKeys.Rows);
      if (rows == null)
        return;

      AddRowsAndLinks(paper, entity, ctx, rows);
      AddRowHeaders(paper, entity, ctx, rows);
    }

    private static void AddRowsAndLinks(IPaper paper, Entity entity, PaperContext ctx, DataWrapperEnumerable rows)
    {
      var keys = rows.EnumerateKeys().ToArray();
      foreach (DataWrapper row in rows)
      {
        var rowEntity = new Entity();
        rowEntity.AddRel(Rel.Row);

        foreach (var key in keys)
        {
          var value = row.GetValue(key);
          rowEntity.AddProperty(key, value);
        }

        var linkRenderers = paper._Call<IEnumerable<ILink>>("GetRowLinks", row.DataSource);
        if (linkRenderers != null)
        {
          foreach (var linkRenderer in linkRenderers)
          {
            var link = linkRenderer.RenderLink(ctx);
            if (link != null)
            {
              link.AddRel(RelNames.RowLink);
              link.Rel.Remove(RelNames.Link);
              rowEntity.AddLink(link);
            }
          }
        }

        entity.AddEntity(rowEntity);
      }
    }

    private static void AddRowHeaders(IPaper paper, Entity entity, PaperContext ctx, DataWrapperEnumerable rows)
    {
      var keys = rows.EnumerateKeys().ToArray();

      foreach (var key in keys)
      {
        var header = rows.GetHeader(key);
        entity.AddRowHeader(header);
      }

      var headers = paper._Call<IEnumerable<HeaderInfo>>("GetRowHeaders", rows.DataSource);
      if (headers != null)
      {
        entity.AddRowHeaders(headers);
      }
    }
  }
}