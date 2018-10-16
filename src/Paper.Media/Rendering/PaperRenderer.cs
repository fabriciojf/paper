using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media.Design;
using Paper.Media.Routing;
using Toolset;
using Toolset.Reflection;

namespace Paper.Media.Rendering
{
  /// <summary>
  /// Utilitário de rederização de rotas do Paper.
  /// O utilitário deve interpretar os parâmetros da rota e produzir uma instância 
  /// de Entity como resposta.
  /// </summary>
  public class PaperRenderer : IRenderer
  {
    /// <summary>
    /// Renderiza a rota produzindo a instância de Entity com as propriedades de hipermídia.
    /// </summary>
    /// <param name="paper">A instância do Paper com as definições de rederização da rota.</param>
    /// <param name="context">O contexto de rederização do Paper.</param>
    /// <returns>A instância de Entity com as propriedades de hipermídia.</returns>
    public Ret<Entity> RenderEntity(IPaper paper, IContext context)
    {
      var targetEntity = new Entity();
      
      if (paper is Entity sourceEntity)
      {
        targetEntity._CopyFrom(sourceEntity);
      }
      else
      {
        RenderPaper(paper, context, targetEntity);
      }

      targetEntity.AddClass(paper.GetType());
      targetEntity.AddLinkSelf(context.RequestUri);
      targetEntity.ResolveLinks(context.RequestUri);

      return targetEntity;
    }

    /// <summary>
    /// Renderiza uma entidade a partir de um IPaper.
    /// </summary>
    /// <param name="paper">A instância do Paper</param>
    /// <param name="context">O contexto de renderização.</param>
    /// <param name="targetEntity">A entidade em construção.</param>
    private void RenderPaper(IPaper paper, IContext context, Entity targetEntity)
    {
      //
      // Fase 2: Repassando parametros
      //
      RenderOfBasics.SetArgs(paper, context);
      RenderOfPage.SetArgs(paper, context);
      RenderOfSort.SetArgs(paper, context);
      RenderOfFilter.SetArgs(paper, context);

      //
      // Fase 3: Consultando dados
      //
      RenderOfPage.PreCache(paper, context);
      RenderOfData.CacheData(paper, context);
      RenderOfRows.CacheData(paper, context);
      RenderOfCards.CacheData(paper, context);
      RenderOfPage.PostCache(paper, context);

      //
      // Fase 4: Renderizando entidade
      //
      RenderOfBasics.Render(paper, context, targetEntity);
      RenderOfData.Render(paper, context, targetEntity);
      RenderOfRows.Render(paper, context, targetEntity);
      RenderOfPage.Render(paper, context, targetEntity);
      RenderOfSort.Render(paper, context, targetEntity);
      RenderOfFilter.Render(paper, context, targetEntity);
      RenderOfCards.Render(paper, context, targetEntity);
    }
  }
}