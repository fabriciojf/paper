
using System.Data;
using Toolset;
using Toolset.Reflection;
using Paper.Media.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using Paper.Media.Design.Papers;
using Media.Utilities.Types;

namespace Paper.Media.Rendering.Papers
{
  static class RenderOfBlueprint
  {
    /// <summary>
    /// Repassa os argumentos indicados para as propriedades na instância de IPaper.
    /// </summary>
    /// <param name="paper">A instância de IPaper que será modificada.</param>
    /// <param name="args">A coleção dos argumentos atribuídos.</param>
    public static void SetArgs(IPaper paper, ArgMap args)
    {
    }

    /// <summary>
    /// Renderiza as característica do Paper.
    /// </summary>
    /// <param name="paper">O Paper com as características de renderização.</param>
    /// <param name="entity">A entidade que está sendo renderizada com base no Paper.</param>
    /// <param name="ctx">O contexto de renderização.</param>
    public static void Render(IPaper paper, Entity entity, PaperContext ctx)
    {
      entity.AddClass(Class.Blueprint);

      AddBlueprint(paper, entity, ctx);
      AddLinks(paper, entity, ctx);
    }

    /// <summary>
    /// Adiciona as propriedades de blueprint.
    /// </summary>
    /// <param name="paper">O Paper com as características de renderização.</param>
    /// <param name="entity">A entidade que está sendo renderizada com base no Paper.</param>
    /// <param name="ctx">O contexto de renderização.</param>
    private static void AddBlueprint(IPaper paper, Entity entity, PaperContext ctx)
    {
      if (!paper._Has("GetBlueprint"))
        return;

      var blueprint = paper._Call<Blueprint>("GetBlueprint");
      if (blueprint == null)
        return;

      entity.AddProperties(blueprint);
    }

    /// <summary>
    /// Renderizando links
    /// </summary>
    /// <param name="paper">O Paper com as características de renderização.</param>
    /// <param name="entity">A entidade que está sendo renderizada com base no Paper.</param>
    /// <param name="ctx">O contexto de renderização.</param>
    private static void AddLinks(IPaper paper, Entity entity, PaperContext ctx)
    {
      var indexLink = paper._Call<ILink>("GetIndex");
      if (indexLink != null)
      {
        var link = indexLink.RenderLink(ctx);
        if (link != null)
        {
          link.AddRel(RelNames.Index);
          link.Rel.Remove(RelNames.Link);
          entity.AddLink(link);
        }
      }
    }
  }
}