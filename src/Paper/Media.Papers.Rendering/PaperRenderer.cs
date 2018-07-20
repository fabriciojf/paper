using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Paper.Media.Design;
using Toolset.Collections;
using Toolset.Reflection;

namespace Media.Design.Extensions.Papers.Rendering
{
  public class PaperRenderer
  {
    private readonly IServiceProvider serviceProvider;

    public PaperRenderer(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public Entity RenderEntity(PaperContext ctx)
    {
      var entity = new Entity();

      //
      // Fase 2: Repassando parametros
      //
      SetArgs(ctx.Paper, ctx.QueryArgs);
      SetArgs(ctx.Paper, ctx.PathArgs);
      RenderOfRowsPage.SetArgs(ctx.Paper, ctx);
      RenderOfRowsSort.SetArgs(ctx.Paper, ctx);

      //
      // Fase 3: Consultando dados
      //
      RenderOfRowsPage.PreCacheRows(ctx.Paper, ctx);
      CacheData(ctx.Paper, ctx.Cache);
      CacheRows(ctx.Paper, ctx.Cache);
      RenderOfRowsPage.PosCacheRows(ctx.Paper, ctx);

      //
      // Fase 4: Renderizando entidade
      //
      RenderOfInfo.Render(ctx.Paper, entity, ctx);
      RenderOfData.Render(ctx.Paper, entity, ctx);
      RenderOfRows.Render(ctx.Paper, entity, ctx);
      RenderOfRowsPage.Render(ctx.Paper, entity, ctx);
      RenderOfRowsSort.Render(ctx.Paper, entity, ctx);

      //
      // Fase 5: Aplicando finalizações
      //
      entity.ResolveLinks(ctx.RequestUri);

      return entity;
    }

    /// <summary>
    /// Repassa os argumentos indicados para as propriedades na instância de IPaper.
    /// </summary>
    /// <param name="paper">A instância de IPaper que será modificada.</param>
    /// <param name="pathArgs">A coleção dos argumentos atribuídos.</param>
    private void SetArgs(IPaper paper, ArgCollection pathArgs)
    {
      foreach (var arg in pathArgs)
      {
        paper._TrySet(arg.Key, arg.Value);
      }
    }

    /// <summary>
    /// Realiza a consulta de dados specificados no Paper e estoca os dados
    /// obtidos no cache indicado.
    /// </summary>
    /// <param name="paper">A instância de IPaper que contém as consultas a dados.</param>
    /// <param name="cache">O cache para estocagem dos dados consultados.</param>
    private void CacheData(IPaper paper, EntryCollection cache)
    {
      if (paper._Has("GetData"))
      {
        var data = paper._Call("GetData");
        var dataWrapper = DataWrapper.Create(data);
        cache.Set(CacheKeys.Data, dataWrapper);
      }
    }

    /// <summary>
    /// Realiza a consulta de registros specificados no Paper e estoca os registros
    /// obtidos no cache indicado.
    /// </summary>
    /// <param name="paper">A instância de IPaper que contém as consultas a dados.</param>
    /// <param name="cache">O cache para estocagem dos registros consultados.</param>
    private void CacheRows(IPaper paper, EntryCollection cache)
    {
      if (paper._Has("GetRows"))
      {
        var data = paper._Call("GetRows");
        var dataWrapper = DataWrapperEnumerable.Create(data);
        cache.Set(CacheKeys.Rows, dataWrapper);
      }
    }
  }
}
