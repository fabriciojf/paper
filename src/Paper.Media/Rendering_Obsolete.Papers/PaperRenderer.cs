using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media.Design;
using Toolset;
using Toolset.Collections;
using Toolset.Reflection;

namespace Paper.Media.Rendering_Obsolete.Papers
{
  public class PaperRenderer
  {
    private readonly IInjector injector;

    public PaperRenderer(IInjector injector)
    {
      this.injector = injector;
    }

    public Ret<Entity> RenderEntity(PaperContext context)
    {
      var entity = new Entity();

      if (context.Paper is Entity)
      {
        CloneEntity(entity, (Entity)context.Paper, context);
      }
      else if (context.Paper is IPaper)
      {
        RenderPaper(entity, (IPaper)context.Paper, context);
      }
      else
      {
        return Ret.Fail("Tipo não suportado pelo renderizador do Paper.Media: " + context.Paper?.GetType().FullName);
      }

      entity.AddClass(context.Paper.GetType());
      entity.AddLinkSelf(context.RequestUri);
      entity.ResolveLinks(context.RequestUri);

      return entity;
    }

    private void CloneEntity(Entity targetEntity, Entity sourceEntity, PaperContext context)
    {
      targetEntity._CopyFrom(sourceEntity);
    }

    private void RenderPaper(Entity entity, IPaper paper, PaperContext context)
    {
      //
      // Fase 2: Repassando parametros
      //
      RenderOfBasics.SetArgs(paper, context.PathArgs);
      RenderOfPage.SetArgs(paper, context);
      RenderOfSort.SetArgs(paper, context);
      RenderOfFilter.SetArgs(paper, context);

      //
      // Fase 3: Consultando dados
      //
      RenderOfPage.PreCache(paper, context);
      RenderOfData.CacheData(paper, context.Cache);
      RenderOfRows.CacheData(paper, context.Cache);
      RenderOfCards.CacheData(paper, context.Cache);
      RenderOfPage.PostCache(paper, context);

      //
      // Fase 4: Renderizando entidade
      //
      RenderOfBasics.Render(paper, entity, context);
      RenderOfData.Render(paper, entity, context);
      RenderOfRows.Render(paper, entity, context);
      RenderOfPage.Render(paper, entity, context);
      RenderOfSort.Render(paper, entity, context);
      RenderOfFilter.Render(paper, entity, context);
      RenderOfCards.Render(paper, entity, context);
    }
  }
}
