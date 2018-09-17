using System;
using System.Linq;
using Media.Utilities.Types;
using Paper.Media.Design;
using Toolset;
using Toolset.Reflection;

namespace Paper.Media.Rendering.Papers
{
  static class RenderOfPage
  {
    public static void SetArgs(IPaper paper, PaperContext ctx)
    {
      if (!paper._Has("Page"))
        return;

      var page = paper._Get<Page>("Page");
      if (page == null)
      {
        if (!paper._CanWrite("Page"))
          return;

        page = page._SetNew<Page>("Page");
      }

      page.CopyFromUri(ctx.RequestUri);

      ctx.Cache.Set(CacheKeys.Page, page);
    }

    /// <summary>
    /// Forçando a consulta de um item a mais para detectar mais dados
    /// disponiveis no lado do servidor
    /// </summary>
    public static void PreCache(IPaper paper, PaperContext ctx)
    {
      var page = ctx.Cache.Get<Page>(CacheKeys.Page);
      if (page == null)
        return;

      page.SetLimitOrSize(page.Limit + 1);
    }

    /// <summary>
    /// Detectando o registro excedente.
    /// Se existir, então, temos mais registros no lado do servidor.
    /// </summary>
    public static void PostCache(IPaper paper, PaperContext ctx)
    {
      var hasMoreData = false;

      var page = ctx.Cache.Get<Page>(CacheKeys.Page);
      if (page == null)
        return;

      var overSize = page.Limit;
      // Voltando o tamanho da página para o original.
      page.SetLimitOrSize(page.Limit - 1);

      var rows = ctx.Cache.Get<DataWrapperEnumerable>(CacheKeys.Rows);
      if (rows != null)
      {
        hasMoreData = (rows.Count == overSize);
        if (hasMoreData)
        {
          // Reduzindo a quantidade de registros que deve ser lidos
          // para provocar o descarte do excedente.
          rows.Count--;
        }
      }

      ctx.Cache.Set(CacheKeys.HasMoreData, hasMoreData);
    }

    public static void Render(IPaper paper, Entity entity, PaperContext ctx)
    {
      var pagination = ctx.Cache.Get<Page>(CacheKeys.Page);
      var hasMoreRows = ctx.Cache.Get<bool>(CacheKeys.HasMoreData);

      if (pagination == null)
        return;

      var hasFirst = pagination.Number > 1;
      var hasPrev = pagination.Number > 1;
      var hasNext = hasMoreRows;

      var route = new Route(ctx.RequestUri);

      if (hasFirst)
      {
        var page = pagination.FirstPage();
        var href = page.CopyToUri(ctx.RequestUri);
        entity.AddLink(href, "Início", Rel.First);
      }

      if (hasPrev)
      {
        var page = pagination.PreviousPage();
        var href = page.CopyToUri(ctx.RequestUri);
        entity.AddLink(href, "Anterior", Rel.Previous);
      }

      if (hasNext)
      {
        var page = pagination.NextPage();
        var href = page.CopyToUri(ctx.RequestUri);
        entity.AddLink(href, "Próxima", Rel.Next);
      }
    }
  }
}