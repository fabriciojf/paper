using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Paper.Media.Design;
using Toolset;
using Toolset.Collections;
using Toolset.Reflection;

namespace Paper.Media.Design.Papers.Rendering
{
  public class PaperRenderer
  {
    private readonly IServiceProvider serviceProvider;

    public PaperRenderer(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public Ret<Entity> RenderEntity(PaperContext ctx)
    {
      var entity = new Entity();

      if (ctx.Paper is Entity)
      {
        CloneEntity(entity, (Entity)ctx.Paper, ctx);
      }
      else if (ctx.Paper is IPaper)
      {
        RenderPaper(entity, (IPaper)ctx.Paper, ctx);
      }
      else
      {
        return Ret.Fail("Tipo não suportado pelo renderizador do Paper.Media: " + ctx.Paper?.GetType().FullName);
      }

      entity.AddClass(ctx.Paper.GetType());
      entity.AddLinkSelf(ctx.RequestUri);
      entity.ResolveLinks(ctx.RequestUri);

      return entity;
    }

    private void CloneEntity(Entity targetEntity, Entity sourceEntity, PaperContext ctx)
    {
      targetEntity._CopyFrom(sourceEntity);
    }

    private void RenderPaper(Entity entity, IPaper paper, PaperContext ctx)
    {
      //
      // Fase 2: Repassando parametros
      //
      RenderOfBasics.SetArgs(paper, ctx.QueryArgs);
      RenderOfBasics.SetArgs(paper, ctx.PathArgs);
      RenderOfRowsPage.SetArgs(paper, ctx);
      RenderOfRowsSort.SetArgs(paper, ctx);

      //
      // Fase 3: Consultando dados
      //
      RenderOfRowsPage.PreCacheRows(paper, ctx);
      RenderOfData.CacheData(paper, ctx.Cache);
      RenderOfRows.CacheData(paper, ctx.Cache);
      RenderOfRowsPage.PosCacheRows(paper, ctx);

      //
      // Fase 4: Renderizando entidade
      //
      RenderOfBasics.Render(paper, entity, ctx);
      RenderOfData.Render(paper, entity, ctx);
      RenderOfRows.Render(paper, entity, ctx);
      RenderOfRowsPage.Render(paper, entity, ctx);
      RenderOfRowsSort.Render(paper, entity, ctx);
    }
  }
}
