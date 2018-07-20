using System;
using System.Linq;
using Paper.Media.Design;
using Paper.Media.Design.Extensions;
using Toolset;
using Toolset.Reflection;

namespace Paper.Media.Papers.Rendering
{
  static class RenderOfRowsPagination
  {
    public static void SetArgs(IPaper paper, PaperContext ctx)
    {
      if (!paper._Has("RowPagination"))
        return;

      var pagination = paper._Get<Pagination>("RowPagination");
      if (pagination == null)
      {
        if (!paper._CanWrite("RowPagination"))
          return;

        pagination = pagination._SetNew<Pagination>("RowPagination");
      }

      pagination.CopyFromUri(ctx.RequestUri);

      ctx.Cache.Set(CacheKeys.RowsPagination, pagination);
    }

    /// <summary>
    /// Forçando a consulta de um item a mais para detectar mais dados
    /// disponiveis no lado do servidor
    /// </summary>
    public static void PreCacheRows(IPaper paper, PaperContext ctx)
    {
      var pagination = ctx.Cache.Get<Pagination>(CacheKeys.RowsPagination);
      if (pagination == null)
        return;

      pagination.SetLimitOrPageSize(pagination.Limit + 1);
    }

    /// <summary>
    /// Detectando o registro excedente.
    /// Se existir, então, temos mais registros no lado do servidor.
    /// </summary>
    public static void PosCacheRows(IPaper paper, PaperContext ctx)
    {
      var pagination = ctx.Cache.Get<Pagination>(CacheKeys.RowsPagination);
      if (pagination == null)
        return;

      var overSize = pagination.Limit;
      var hasMoreRows = false;

      // Voltando o tamanho da página para o original.
      pagination.SetLimitOrPageSize(pagination.Limit - 1);

      var rows = ctx.Cache.Get<DataWrapperEnumerable>(CacheKeys.Rows);
      if (rows != null)
      {
        hasMoreRows = (rows.Count == overSize);
        if (hasMoreRows)
        {
          // Reduzindo a quantidade de registros que deve ser lidos
          // para provocar o descarte do excedente.
          rows.Count--;
        }
      }

      ctx.Cache.Set(CacheKeys.HasMoreRows, hasMoreRows);
    }

    public static void Render(IPaper paper, Entity entity, PaperContext ctx)
    {
      var pagination = ctx.Cache.Get<Pagination>(CacheKeys.RowsPagination);
      var hasMoreRows = ctx.Cache.Get<bool>(CacheKeys.HasMoreRows);

      if (pagination == null)
        return;

      var hasFirst = pagination.Page > 2;
      var hasPrev = pagination.Page > 1;
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